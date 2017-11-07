using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Networking;

public class PlayerAvatar : NetworkBehaviour
{
    public VRNode node = VRNode.Head;
    public GameObject handPositionSetterPrefab;
    public GameObject otherPrefab;

    private GameObject playerVR;
    private GameObject playerHead;
    private GameObject playerBody;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Debug.Log("MirrorNetworkedVRNode::OnStartServer: Object " + this.gameObject.name + ":" + this.gameObject.GetInstanceID().ToString() + " coming active!");

        // Get gameobject handling player VR stuff
        playerVR = GameObject.FindGameObjectWithTag("Player");

        // Get player head and body gameobjects
        playerHead = transform.GetChild(0).gameObject;
        playerBody = transform.GetChild(1).gameObject;

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
        left.GetComponent<HandPositionSetter>().TargetSetHand(connectionToClient, VRNode.LeftHand);
        right.GetComponent<HandPositionSetter>().TargetSetHand(connectionToClient, VRNode.RightHand);
    }

    IEnumerator TrackHeadCoroutine()
    {
        Debug.Log("MirrorNetworkedVRNode::TrackHeadCoroutine: Starting avatar tracking!");
        while (true)
        {
            Vector3 nodePos = playerVR.transform.position + InputTracking.GetLocalPosition(node);
            Quaternion nodeRot = InputTracking.GetLocalRotation(node);

            playerHead.transform.rotation = nodeRot;
            playerHead.transform.position = nodePos;

            Vector3 newBodyRot = new Vector3(0, nodeRot.eulerAngles.y, 0);
            playerBody.transform.rotation = Quaternion.Euler(newBodyRot);
            // Body position is lower than head position
            playerBody.transform.position = new Vector3(nodePos.x, nodePos.y - 0.8f, nodePos.z);
            
            yield return null;
        }
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
