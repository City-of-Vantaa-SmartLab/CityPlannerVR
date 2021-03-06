using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;
using System;

public class PhotonPlayerAvatar : Photon.PunBehaviour
{
    #region Public Variables

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;
    public bool initialized = false;

    public UnityEngine.XR.XRNode node = UnityEngine.XR.XRNode.Head;

    public Material localPlayerBodyMaterial;

    public List<Material> AvatarMaterials = new List<Material>();

	//Room edge value, order Ikkuna, Tutorial, !ikkuna, !tutorial
	public List<float> RoomLimits = new List<float> ();

    public ParticleSystem voiceIndicator;

    #endregion

    #region Private Variables

    private GameObject playerVR;
    private GameObject playerHead;
    private GameObject playerBody;
    public GameObject toolbelt;

    private GameObject leftHand;
    private GameObject rightHand;

    private Hand hand1;
    private Hand hand2;

    ToolManager hand1Manager;
    ToolManager hand2Manager;

    //used to determine the height of the table
    private GameObject cityTeleportArea;

    private Vector3 playerBodyScaleFactor;

    private CheckPlayerSize playerSize;

    //Used to call the scalePlayer function from Scale object to scale player big again
    private ScaleObject[] scalePlayer;

    private int currentAvatarMaterial = 0;

    //Values come from the layer list
    int buildingLayer = 9;
    int measurePointLayer = 11;
    int lockedPropsLayer = 19;
    int spawnSlotLayer = 21;
    int buttonLayer = 24;
    int normalMask;
    int finalMask;

    ExitGames.Client.Photon.Hashtable playerCustomProperties = new ExitGames.Client.Photon.Hashtable();

	private bool isInPlayArea = false;

    #endregion

    void Awake()
    {
        // Keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.isMine)
        {
            PhotonPlayerAvatar.LocalPlayerInstance = this.gameObject;
        }
        // We flag as don't destroy on load so that instance survives level synchronization, 
        // thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        Debug.Log("PlayerAvatar created");

        // Get gameobject handling player VR stuff
        playerVR = GameObject.FindGameObjectWithTag("Player");

        if (PhotonPlayerAvatar.LocalPlayerInstance.GetComponent<PhotonPlayerAvatar>().initialized == false)
        {
            Debug.Log("Initializing location");
            PhotonPlayerAvatar.LocalPlayerInstance.GetComponent<PhotonPlayerAvatar>().initialized = true;
            PhotonGameManager tmpManager = GameObject.Find("GameManager").GetComponent<PhotonGameManager>();
            Vector3 spawnLoc = new Vector3(tmpManager.playerSpawnPoints[tmpManager.connection.GetNumberOfClients()].x, playerVR.transform.position.y, tmpManager.playerSpawnPoints[tmpManager.connection.GetNumberOfClients()].z);
            playerVR.transform.position = spawnLoc;
        }
        // Get player head and body gameobjects
        playerHead = transform.GetChild(0).gameObject;
        playerBody = transform.GetChild(1).gameObject;


        leftHand = transform.GetChild(2).gameObject;
        rightHand = transform.GetChild(3).gameObject;

        hand1 = GameObject.Find("Player/SteamVRObjects/Hand1").GetComponent<Hand>();
        hand2 = GameObject.Find("Player/SteamVRObjects/Hand2").GetComponent<Hand>();

        hand1Manager = hand1.gameObject.GetComponent<ToolManager>();
        hand2Manager = hand2.gameObject.GetComponent<ToolManager>();

        if (this.gameObject.GetComponent<PhotonView>().isMine)
        {
            InstantiateToolbelt();
        }
        playerBodyScaleFactor = playerBody.transform.localScale;

        //use this if using the simplified version of the Tikkuraitti model
        cityTeleportArea = GameObject.Find("Environment/TikkuraittiModel_simple/TeleportAreaCity");

        playerSize = playerVR.GetComponent<CheckPlayerSize>();

        scalePlayer = GameObject.Find("ScaleToBigObject").GetComponents<ScaleObject>();

        ParticleSystem.MainModule main = voiceIndicator.main;
        main.startColor = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        int lockedPropsLayerMask = 1 << lockedPropsLayer;
        int buildingLayerMask = 1 << buildingLayer;
        int measureLayerMask = 1 << measurePointLayer;
        int spawnSlotLayerMask = 1 << spawnSlotLayer;
        int buttonLayerMask = 1 << buttonLayer;
        finalMask = ~(buildingLayerMask | measureLayerMask | lockedPropsLayerMask | spawnSlotLayerMask | buttonLayerMask);
        normalMask = ~(lockedPropsLayerMask | spawnSlotLayerMask | buttonLayerMask);

        if (this.gameObject.GetComponent<PhotonView>().isMine)
        {
            StartCoroutine(TrackHeadCoroutine());
            StartCoroutine(TrackHandNodeCoroutine(UnityEngine.XR.XRNode.LeftHand, leftHand));  //Left hand
            StartCoroutine(TrackHandNodeCoroutine(UnityEngine.XR.XRNode.RightHand, rightHand));  //Right hand
        }
    }

    IEnumerator TrackHeadCoroutine()
    {
        Debug.Log("PlayerAvatar::TrackHeadCoroutine: Starting avatar tracking!");
        while (true)
        {
            Quaternion nodeRot = UnityEngine.XR.InputTracking.GetLocalRotation(node);  //nodeRot.eulerAngles.normalized;

            //This caused a little visual bug when player was scaled down (the head was a bit off)
            //Vector3 nodePos = MoveBodyOutOfTheWayOfCamera(Camera.main.transform.position, -Camera.main.transform.forward); // This still works when player gets scaled down/up, above does not
            Vector3 nodePos = Camera.main.transform.position;

            //moves the "Avatar" gameobject which has head and body as children
            transform.position = nodePos;

            //Rotates the head of player
            playerHead.transform.localRotation = nodeRot;

            //Rotates the body of player
            Vector3 newBodyRot = new Vector3(0, nodeRot.eulerAngles.y, 0);
            playerBody.transform.localRotation = Quaternion.Euler(newBodyRot);

            //Checks if player tries to jump down from the table
            CheckPlayerPosition();

            yield return null;
        }
    }

    IEnumerator TrackHandNodeCoroutine(UnityEngine.XR.XRNode node, GameObject hand)
    {
        Quaternion handoffsetrot = Quaternion.Euler(-90, 0, 0);
        hand.transform.rotation = handoffsetrot * UnityEngine.XR.InputTracking.GetLocalRotation(node);
        while (true)
        {

            hand.transform.rotation = UnityEngine.XR.InputTracking.GetLocalRotation(node);

            if (playerSize.isSmall)
            {
                //all the axes are same for scale, so no matter which one is used. (If they're not, something is wrong and it should be fixed)
                hand.transform.position = playerVR.transform.position + UnityEngine.XR.InputTracking.GetLocalPosition(node) * playerVR.transform.localScale.x;
                //Now player won't be able to pick up building or other stuff we don't want when they are shrinked down on the table
                hand1.hoverLayerMask = finalMask;
                hand2.hoverLayerMask = finalMask;
            }

            //Check if we are in god mode (big)
            else
            {
				Vector3 tmpPos = playerVR.transform.position + UnityEngine.XR.InputTracking.GetLocalPosition(node) * playerVR.transform.localScale.x;
				if (tmpPos.z >= RoomLimits [0] || tmpPos.z <= RoomLimits [2]) {
					//Debug.LogWarning ("Outside");
					tmpPos.z = hand.transform.position.z;
					hand.transform.position = tmpPos;
				} else if (isInPlayArea && (tmpPos.x <= RoomLimits [1] || tmpPos.x >= RoomLimits [3])) {
					tmpPos.x = hand.transform.position.x;
					hand.transform.position = tmpPos;
				} else {
					hand.transform.position = tmpPos;
				}

                //hand.transform.position = playerVR.transform.position + UnityEngine.XR.InputTracking.GetLocalPosition(node) * playerVR.transform.localScale.x;
                //Player is normal sized again and must be able to move everything again
                if (hand1Manager.Tool == ToolManager.ToolType.Empty)
                {
                    hand1.hoverLayerMask = normalMask;
                }
                if (hand2Manager.Tool == ToolManager.ToolType.Empty)
                {
                    hand2.hoverLayerMask = normalMask;
                }

            }

            //CmdScaleHands(playerVR.transform.localScale * 0.07f);

            yield return null;
        }
    }

	public void CollisionHappened(Collider other)
	{
		if (isInPlayArea) {
			return;
		} else {
			if (other.CompareTag ("PlayAreaTrigger")) {
				Debug.LogWarning ("Player collided with play area!");
				isInPlayArea = true;
				if (this.gameObject.GetComponent<PhotonView> ().isMine) {
					playerBody.GetComponent<MeshRenderer> ().material = localPlayerBodyMaterial;
				}
				GameObject.Find ("GameManager").GetComponent<Logger> ().LogActionLine ("Move_inside_play_area", null);
			}
		}
	}

    private Vector3 MoveBodyOutOfTheWayOfCamera(Vector3 cameraPos, Vector3 backVector)
    {
        Vector3 bodyPos = cameraPos;
        /*if (backVector.x >= 0) {
			bodyPos.x += backVector.x * 0.3f < 0.3f ? 0.3f : backVector.x * 0.3f;
		} else {
			bodyPos.x += backVector.x * 0.3f > -0.3f ? -0.3f : backVector.x * 0.3f;
		}
		if (backVector.z >= 0) {
			bodyPos.z += backVector.z * 0.3f < 0.3f ? 0.3f : backVector.z * 0.3f;
		} else {
			bodyPos.z += backVector.z * 0.3f > -0.3f ? -0.3f : backVector.z * 0.3f;
		}*/
        bodyPos += new Vector3(backVector.x * 0.3f, 0, backVector.z * 0.3f);
        return bodyPos;
    }

    [PunRPC]
    public void UpdateScale(Vector3 newScale)
    {
        playerCustomProperties["Scale"] = newScale;

        PhotonNetwork.player.SetCustomProperties(playerCustomProperties);

        Vector3 scale = (Vector3)PhotonNetwork.player.CustomProperties["Scale"];

        transform.localScale = scale;
    }

    private void InstantiateToolbelt()
    {
        GameObject prefab = (GameObject)Resources.Load("Prefabs/ToolHolders/Toolbeltv3");
        toolbelt = Instantiate(prefab);

        Vector3 localPos = toolbelt.transform.localPosition;
        Vector3 globalScale = toolbelt.transform.localScale; //record prefabs true scale
        toolbelt.transform.parent = playerBody.transform;
        //toolbelt.transform.parent = transform;

        toolbelt.transform.localScale = Vector3.one; //required for lossy scale
        toolbelt.transform.localScale = new Vector3(globalScale.x / toolbelt.transform.lossyScale.x,
            globalScale.y / toolbelt.transform.lossyScale.y, globalScale.z / toolbelt.transform.lossyScale.z);
        toolbelt.transform.localPosition = localPos;
    }

    #region Stop player from themselves

    /// <summary>
    /// Stops player from jumping down the table if in pedestrian mode
    /// </summary>

    //Is used to store the last and current position of the player
    List<Vector3> positions_list = new List<Vector3>();

    //Keeps track of the last 2 positions player had
    public void TrackPlayerPosition()
    {
        //if the list is empty
        if (positions_list.Count == 0)
        {
            positions_list.Add(playerVR.transform.position);
        }
        //if there is 1 element in the list
        else if (positions_list.Count == 1)
        {
            //if our current position is not the same as the previous position
            //(We don't want to update the list, if we are standing still)
            if (positions_list[0] != playerVR.transform.position)
            {
                positions_list.Add(playerVR.transform.position);
            }
        }
        //if list is "full" (Count => 2) (we don't want to store all the position during runtime, just the current and previous)
        else
        {
            //if there is two or more positions, remove the first one (we don't need to know it anymore)
            RemoveFromPositions_list();
        }
    }

    //Checks if player tried to jump down from the table
    void CheckPlayerPosition()
    {
        //if we are on pedestrian mode (small)
        if (playerSize.isSmall)
        {
            TrackPlayerPosition();

            if (playerVR.transform.position.y < cityTeleportArea.transform.position.y)
            {
                //playerVR.transform.position = positions_list[0];
                for (int i = 0; i < scalePlayer.Length; i++)
                {
                    scalePlayer[i].ScalePlayer();
                }
            }
        }
    }

    public void RemoveFromPositions_list()
    {
        positions_list.RemoveAt(0);
    }

    #endregion

    [PunRPC]
    public void ChangeMaterialToAvatar()
    {
        currentAvatarMaterial++;
        if (currentAvatarMaterial >= AvatarMaterials.Count)
        {
            currentAvatarMaterial = 0;
        }

        Debug.Log("Changing Material");
        foreach (Transform child in this.gameObject.transform)
        {

            if (currentAvatarMaterial == 0)
            {
                //enable male avatar
                this.transform.Find("Head").GetComponent<Renderer>().enabled = true;
                this.transform.Find("Body").GetComponent<Renderer>().enabled = true;
                this.transform.Find("PhotonHandLeft").GetComponent<Renderer>().enabled = true;
                this.transform.Find("PhotonHandRight").GetComponent<Renderer>().enabled = true;

                //disable female avatar
                this.transform.Find("Head/Head_N").GetComponent<Renderer>().enabled = false;
                this.transform.Find("Body/Body_N").GetComponent<Renderer>().enabled = false;
                this.transform.Find("PhotonHandLeft/PhotonHandLeft_N").GetComponent<Renderer>().enabled = false;
                this.transform.Find("PhotonHandRight/PhotonHandRight_N").GetComponent<Renderer>().enabled = false;
                if (child.GetComponent<MeshRenderer>())
                    child.GetComponent<MeshRenderer>().material = AvatarMaterials[currentAvatarMaterial];
                else
                {
                    //Debug.Log("This object is null don't care about it");
                }
                foreach (Transform grandChild in child.gameObject.transform)
                {
                    if (grandChild.GetComponent<MeshRenderer>())
                        grandChild.GetComponent<MeshRenderer>().material = AvatarMaterials[currentAvatarMaterial];
                    else
                    {
                        //Debug.Log("This object is null don't care about it");
                    }
                }
                Debug.Log("Changing Material done");
            }
            else if (currentAvatarMaterial == 4)
            {
                //disable male avatar
                this.transform.Find("Head").GetComponent<Renderer>().enabled = false;
                this.transform.Find("Body").GetComponent<Renderer>().enabled = false;
                this.transform.Find("PhotonHandLeft").GetComponent<Renderer>().enabled = false;
                this.transform.Find("PhotonHandRight").GetComponent<Renderer>().enabled = false;

                //enable female avatar
                this.transform.Find("Head/Head_N").GetComponent<Renderer>().enabled = true;
                this.transform.Find("Body/Body_N").GetComponent<Renderer>().enabled = true;
                this.transform.Find("PhotonHandLeft/PhotonHandLeft_N").GetComponent<Renderer>().enabled = true;
                this.transform.Find("PhotonHandRight/PhotonHandRight_N").GetComponent<Renderer>().enabled = true;

                if (child.GetComponent<MeshRenderer>())
                    child.GetComponent<MeshRenderer>().material = AvatarMaterials[currentAvatarMaterial];
                else
                {
                    Debug.Log("This object is null don't care about it");
                }
                foreach (Transform grandChild in child.gameObject.transform)
                {
                    if (grandChild.GetComponent<MeshRenderer>())
                        grandChild.GetComponent<MeshRenderer>().material = AvatarMaterials[currentAvatarMaterial];
                    else
                        Debug.Log("This object is null don't care about it");
                }
                Debug.Log("Changing Material done");
            }
            else
            {
                if (child.GetComponent<MeshRenderer>())
                    child.GetComponent<MeshRenderer>().material = AvatarMaterials[currentAvatarMaterial];
                else
                {
                    Debug.Log("This object is null don't care about it");
                }
                foreach (Transform grandChild in child.gameObject.transform)
                {
                    if (grandChild.GetComponent<MeshRenderer>())
                        grandChild.GetComponent<MeshRenderer>().material = AvatarMaterials[currentAvatarMaterial];
                    else
                    {
                        Debug.Log("This object is null don't care about it");
                    }

                    Debug.Log("Changing Material done");
                }
            }

            //if (this.gameObject.GetComponent<PhotonView>().isMine)
            //{
            //    playerBody.GetComponent<MeshRenderer>().material = localPlayerBodyMaterial;
            //}
        }
    }
}
