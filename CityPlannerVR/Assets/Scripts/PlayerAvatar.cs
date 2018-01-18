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

    //These two can be deleted
    //private GameObject left;
    //private GameObject right;

    [SyncVar(hook = "ScaleChange")]
    public Vector3 objScale;

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
        GameObject left = Instantiate(handPositionSetterPrefab);
        GameObject right = Instantiate(handPositionSetterPrefab);

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

            //Will not delete these yet, but if you can read this, these can be deleted
            //playerHead.transform.position = nodePos;
            //playerHead.transform.localScale = playerVR.transform.localScale;

            
            
            //Rotates the body of player
            Vector3 newBodyRot = new Vector3(0, nodeRot.eulerAngles.y, 0);
            playerBody.transform.localRotation = Quaternion.Euler(newBodyRot);

            //Will not delete these yet, but if you can read this, these can be deleted
            // Body position is lower than head position
            //playerBody.transform.position = new Vector3(nodePos.x, nodePos.y - 0.8f * playerVR.transform.localScale.y, nodePos.z);
            //playerBody.transform.localScale = Vector3.Scale(playerVR.transform.localScale, playerBodyScaleFactor);


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
}
