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

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Debug.Log("PlayerAvatar::OnStartLocalPlayer: Object " + this.gameObject.name + ":" + this.gameObject.GetInstanceID().ToString() + " coming active!");

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
        //if (NetworkServer.active)
        //{
        //// Vive controllers might take a while to become active at
        //// startup so it's nice to wait for a second before 
        //// attempting to do something with them.
        yield return new WaitForSeconds(1f);
        CmdSpawnHands();
        //} else
        //{
        //    Debug.Log("Server inactive!");
        //}
    }

    [Command]
    private void CmdSpawnHands()
    {
        var left = Instantiate(handPositionSetterPrefab);
        var right = Instantiate(handPositionSetterPrefab);

        NetworkServer.SpawnWithClientAuthority(left, connectionToClient);
        NetworkServer.SpawnWithClientAuthority(right, connectionToClient);

        // Tell client that these are its hands and it should keep track of them
        left.GetComponent<HandPositionSetter>().TargetSetHand(connectionToClient, UnityEngine.XR.XRNode.LeftHand);
        right.GetComponent<HandPositionSetter>().TargetSetHand(connectionToClient, UnityEngine.XR.XRNode.RightHand);
    }

    IEnumerator TrackHeadCoroutine()
    {
        Debug.Log("PlayerAvatar::TrackHeadCoroutine: Starting avatar tracking!");
        while (true)
        {
            //Vector3 nodePos = playerVR.transform.position + UnityEngine.XR.InputTracking.GetLocalPosition(node);
            Vector3 nodePos = Camera.main.transform.position;
            Quaternion nodeRot = UnityEngine.XR.InputTracking.GetLocalRotation(node);

            playerHead.transform.rotation = nodeRot;
            playerHead.transform.position = nodePos;
            //playerHead.transform.localScale = playerVR.transform.localScale;

            Vector3 newBodyRot = new Vector3(0, nodeRot.eulerAngles.y, 0);
            playerBody.transform.rotation = Quaternion.Euler(newBodyRot);
            // Body position is lower than head position
            playerBody.transform.position = new Vector3(nodePos.x, nodePos.y - 0.8f * playerVR.transform.localScale.y, nodePos.z);
            //playerBody.transform.localScale = Vector3.Scale(playerVR.transform.localScale, playerBodyScaleFactor);

            yield return null;
        }
    }

    [Command]
    public void CmdUpdateScale(Vector3 newScale)
    {
        Debug.Log("PlayerAvatar::CmdUpdateScale: Scaling to " + newScale.z.ToString());
        transform.localScale = newScale;
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
