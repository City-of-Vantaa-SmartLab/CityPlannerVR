using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Networking;

public class PlayerAvatar : NetworkBehaviour
{
    public UnityEngine.XR.XRNode node = UnityEngine.XR.XRNode.Head;
    public GameObject handPositionSetterPrefab;
    public GameObject otherPrefab;

    private GameObject playerVR;
    private GameObject playerHead;
    private GameObject playerBody;

    private Vector3 playerBodyScaleFactor;

    private GameObject left;
    private GameObject right;

    //used to determine the height of the table
    private GameObject cityTeleportArea;
	//Used to call the scalePlayer function from Scale object to scale player big again
	private ScaleObject scalePlayer;

    [SyncVar(hook = "ScaleChange")]
    public Vector3 objScale;

    private CheckPlayerSize playerSize;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        //Debug.Log("PlayerAvatar::OnStartLocalPlayer: Object " + this.gameObject.name + ":" + this.gameObject.GetInstanceID().ToString() + " coming active!");

        // Get gameobject handling player VR stuff
        playerVR = GameObject.FindGameObjectWithTag("Player");

        // Get player head and body gameobjects
        playerHead = transform.GetChild(0).gameObject;
        playerBody = transform.GetChild(1).gameObject;

        playerBodyScaleFactor = playerBody.transform.localScale;

        //use this, if using the detailed version of the Tikkuraitti model
        //cityTeleportArea = GameObject.Find("Environment/NewTikkuraittiModel/TeleportAreaCity");

        //use this if using the simplified version of the Tikkuraitti model
        cityTeleportArea = GameObject.Find("Environment/TikkuraittiModel_simple/TeleportAreaCity");

        playerSize = playerVR.GetComponent<CheckPlayerSize>();

		scalePlayer = GameObject.Find ("ScaleToBigObject").GetComponent<ScaleObject>();

        StartCoroutine(TrackHeadCoroutine());
        StartCoroutine(MakeSureSetHand());   
    }

    IEnumerator MakeSureSetHand()
    {
        // Vive controllers might take a while to become active at
        // startup so it's nice to wait for a second before 
        // attempting to do something with them.
        yield return new WaitForSeconds(1f);
        CmdSpawnHands();
    }

    [Command]
    private void CmdSpawnHands()
    {
        left = Instantiate(handPositionSetterPrefab);
        right = Instantiate(handPositionSetterPrefab);

        NetworkServer.SpawnWithClientAuthority(left, connectionToClient);
        NetworkServer.SpawnWithClientAuthority(right, connectionToClient);

        // Tell client that these are its hands and it should keep track of them
        left.GetComponent<HandPositionSetter>().TargetSetHand(connectionToClient, UnityEngine.XR.XRNode.LeftHand);
        right.GetComponent<HandPositionSetter>().TargetSetHand(connectionToClient, UnityEngine.XR.XRNode.RightHand);
    }

    IEnumerator TrackHeadCoroutine()
    {
        //Debug.Log("PlayerAvatar::TrackHeadCoroutine: Starting avatar tracking!");
        while (true)
        {
            //Vector3 nodePos = playerVR.transform.position + UnityEngine.XR.InputTracking.GetLocalPosition(node);
            Vector3 nodePos = Camera.main.transform.position; // This still works when player gets scaled down/up, above does not
            Quaternion nodeRot = UnityEngine.XR.InputTracking.GetLocalRotation(node);

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

    [Command]
    public void CmdUpdateScale(Vector3 newScale)
    {
        //Debug.Log("PlayerAvatar::CmdUpdateScale: Scaling to " + newScale.z.ToString());
        objScale = newScale;
        transform.localScale = objScale;
    }

    public void ScaleChange(Vector3 newScaleValue)
    {
        //Debug.Log("PlayerAvatar::ScaleChange: SyncVar scale updated to " + newScaleValue + " (" + gameObject.name + ")");
        // Most likely unnecessary, but just to make absolutely sure all variables do get updated
        objScale = newScaleValue;
        transform.localScale = objScale;
    }

    [Command]
    public void CmdSetAuth(NetworkInstanceId objectId, NetworkIdentity player)
    {
        var iObject = NetworkServer.FindLocalObject(objectId);
        var networkIdentity = iObject.GetComponent<NetworkIdentity>();
        var otherOwner = networkIdentity.clientAuthorityOwner;

        if (otherOwner == player.connectionToClient)
        {
            return;
        }
        else
        {
            if (otherOwner != null)
            {
                networkIdentity.RemoveClientAuthority(otherOwner);
            }
            networkIdentity.AssignClientAuthority(player.connectionToClient);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------

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

    //----------------------------------------------------------------------------------------------------------------------------

    //Checks if player tried to jump down from the table
    void CheckPlayerPosition()
    {
        Debug.Log("CheckPlayerPosition");
        //if we are on pedestrian mode (small)
        if (playerSize.isSmall)
        {
            Debug.Log("player is small");
            TrackPlayerPosition();

            if (playerVR.transform.position.y < cityTeleportArea.transform.position.y)
            {
                Debug.Log("positions_list[0]: " + positions_list[0] + ", positions_list[1]: " + positions_list[1]);
                //playerVR.transform.position = positions_list[0];
				playerVR.transform.position = positions_list[1];
				scalePlayer.ScalePlayer();
            }
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------

    public void RemoveFromPositions_list()
    {
        positions_list.RemoveAt(0);
    }

#endregion
}
