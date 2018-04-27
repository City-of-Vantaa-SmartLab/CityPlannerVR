using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

public class PhotonPlayerAvatar : Photon.PunBehaviour
{
	#region Public Variables

	[Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
	public static GameObject LocalPlayerInstance;

	public UnityEngine.XR.XRNode node = UnityEngine.XR.XRNode.Head;

	public Material localPlayerBodyMaterial;

	#endregion

	#region Private Variables

	private GameObject playerVR;
	private GameObject playerHead;
	private GameObject playerBody;

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

    //Values come from the layer list
    int buildingLayer = 9;
    int measurePointLayer = 11;
    int finalMask;

	ExitGames.Client.Photon.Hashtable playerCustomProperties = new ExitGames.Client.Photon.Hashtable();

    #endregion

    void Awake()
	{
		// Keep track of the localPlayer instance to prevent instantiation when levels are synchronized
		if ( photonView.isMine)
		{
			PhotonPlayerAvatar.LocalPlayerInstance = this.gameObject;
		}
		// We flag as don't destroy on load so that instance survives level synchronization, 
		// thus giving a seamless experience when levels load.
		DontDestroyOnLoad(this.gameObject);
	}

	void Start()
	{
		Debug.Log ("PlayerAvatar created");

		// Get gameobject handling player VR stuff
		playerVR = GameObject.FindGameObjectWithTag("Player");

		// Get player head and body gameobjects
		playerHead = transform.GetChild(0).gameObject;
		playerBody = transform.GetChild(1).gameObject;

		leftHand = transform.GetChild(2).gameObject;
		rightHand = transform.GetChild(3).gameObject;

        hand1 = GameObject.Find("Player/SteamVRObjects/Hand1").GetComponent<Hand>();
        hand2 = GameObject.Find("Player/SteamVRObjects/Hand2").GetComponent<Hand>();

        hand1Manager = hand1.gameObject.GetComponent<ToolManager>();
        hand2Manager = hand2.gameObject.GetComponent<ToolManager>();

        if (this.gameObject.GetComponent<PhotonView> ().isMine) {
			playerBody.GetComponent<MeshRenderer> ().material = localPlayerBodyMaterial;
		}
		playerBodyScaleFactor = playerBody.transform.localScale;

		//use this if using the simplified version of the Tikkuraitti model
		cityTeleportArea = GameObject.Find("Environment/TikkuraittiModel_simple/TeleportAreaCity");

		playerSize = playerVR.GetComponent<CheckPlayerSize>();

        int buildingLayerMask = 1 << buildingLayer;
        int measureLayerMask = 1 << measurePointLayer;
        finalMask = ~(buildingLayerMask | measureLayerMask);

        if (this.gameObject.GetComponent<PhotonView> ().isMine) {
			StartCoroutine (TrackHeadCoroutine ());
			StartCoroutine (TrackHandNodeCoroutine (UnityEngine.XR.XRNode.LeftHand, leftHand));  //Left hand
			StartCoroutine (TrackHandNodeCoroutine (UnityEngine.XR.XRNode.RightHand, rightHand));  //Right hand
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
		while (true)
		{
			hand.transform.rotation = UnityEngine.XR.InputTracking.GetLocalRotation(node);

			if (playerSize.isSmall)
			{
				//                                                                                                       all the axes are same for scale, so no matter which one is used. (If they're not, something is wrong and it should be fixed)
				hand.transform.position = playerVR.transform.position + UnityEngine.XR.InputTracking.GetLocalPosition(node) * playerVR.transform.localScale.x;
				//Now player won't be able to pick up building or other stuff we don't want when they are shrinked down on the table
				hand1.hoverLayerMask = finalMask;
				hand2.hoverLayerMask = finalMask;
			}

			//Check if we are in god mode (big)
			else
			{
				hand.transform.position = playerVR.transform.position + UnityEngine.XR.InputTracking.GetLocalPosition(node) * playerVR.transform.localScale.x;
				//Player is normal sized again and must be able to move everything again
                if(hand1Manager.Tool == ToolManager.ToolType.Empty)
                {
                    hand1.hoverLayerMask = -1;
                }
                if (hand2Manager.Tool == ToolManager.ToolType.Empty)
                {
                    hand2.hoverLayerMask = -1;
                }
                
			}

			//CmdScaleHands(playerVR.transform.localScale * 0.07f);

			yield return null;
		}
	}

	/*IEnumerator TrackHands()
	{
		Debug.Log ("PlayerAvatar::TrackHands: Starting hand tracking");
		while (true)
		{
			if (this.leftHand != null) {
				Vector3 leftPos = GameObject.Find ("Hand1").GetComponent<Valve.VR.InteractionSystem.Hand> ().transform.localPosition;
				Quaternion leftRot = GameObject.Find ("Hand1").GetComponent<Valve.VR.InteractionSystem.Hand> ().transform.localRotation;

				leftHand.transform.localPosition = leftPos;
				leftHand.transform.rotation = leftRot;
				SteamVR vr = SteamVR.instance;
				if ( vr != null )
				{
					var pose = new Valve.VR.TrackedDevicePose_t();
					var gamePose = new Valve.VR.TrackedDevicePose_t();
					var err = vr.compositor.GetLastPoseForTrackedDeviceIndex( SteamVR_Controller.Input(3).index, ref pose, ref gamePose );
					if ( err == Valve.VR.EVRCompositorError.None )
					{
						var t = new SteamVR_Utils.RigidTransform( gamePose.mDeviceToAbsoluteTracking );
						leftHand.transform.localPosition = t.pos;
						leftHand.transform.localRotation = t.rot;
					}
				}
			}

			if (this.rightHand != null) {
				Vector3 rightPos = SteamVR_Controller.Input(4).transform.pos;
				Quaternion rightRot = SteamVR_Controller.Input(4).transform.rot;

				rightHand.transform.position = rightPos;
				rightHand.transform.rotation = rightRot;
			}	

			yield return null;
		}
	}*/
		
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
		bodyPos += new Vector3 (backVector.x * 0.3f, 0, backVector.z * 0.3f);
		return bodyPos;
	}

    //[PunRPC]
    public void UpdateScale(Vector3 newScale)
    {
        playerCustomProperties["Scale"] = newScale;

        PhotonNetwork.player.SetCustomProperties(playerCustomProperties);

        Vector3 scale = (Vector3)PhotonNetwork.player.CustomProperties ["Scale"];

        transform.localScale = scale;
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
		if(positions_list.Count == 0)
		{
			positions_list.Add(playerVR.transform.position);
		}
		//if there is 1 element in the list
		else if(positions_list.Count == 1)
		{
			//if our current position is not the same as the previous position
			//(We don't want to update the list, if we are standing still)
			if(positions_list[0] != playerVR.transform.position)
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
				playerVR.transform.position = positions_list[0];
			}
		}
	}
		
	public void RemoveFromPositions_list()
	{
		positions_list.RemoveAt(0);
	}

#endregion
}
