using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Networking;

public class PlayerAvatar : NetworkBehaviour
{
    public VRNode node = VRNode.LeftHand;
    public GameObject handPositionSetterPrefab;
    public GameObject otherPrefab;

    private GameObject playerHead;
    private GameObject playerBody;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Debug.Log("MirrorNetworkedVRNode::OnStartServer: Object " + this.gameObject.name + ":" + this.gameObject.GetInstanceID().ToString() + " coming active!");

        // Get player head and body gameobjects
        playerHead = transform.GetChild(0).gameObject;
        playerBody = transform.GetChild(1).gameObject;

        StartCoroutine(TrackHeadCoroutine());
        StartCoroutine(MakeSureSetHand());   
    }

    IEnumerator MakeSureSetHand()
    {
        CmdSpawn();
        //if (NetworkServer.active)
        //{
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

        left.GetComponent<HandPositionSetter>().TargetSetHand(connectionToClient, VRNode.LeftHand);
        right.GetComponent<HandPositionSetter>().TargetSetHand(connectionToClient, VRNode.RightHand);
    }


    IEnumerator TrackHeadCoroutine()
    {
        Debug.Log("MirrorNetworkedVRNode::TrackHeadCoroutine: Start avatar tracking!");
        while (true)
        {
            Vector3 nodePos = InputTracking.GetLocalPosition(node);
            Quaternion nodeRot = InputTracking.GetLocalRotation(node);

            playerHead.transform.rotation = nodeRot;
            playerHead.transform.position = nodePos;

            Vector3 bodyNewRot = new Vector3(0, nodeRot.eulerAngles.y, 0);

            playerBody.transform.rotation = Quaternion.Euler(bodyNewRot);
            // Body position is lower than head position
            playerBody.transform.position = new Vector3(nodePos.x, nodePos.y - 0.75f, nodePos.z);
            
            yield return null;
        }
    }

    [Command]
    void CmdSpawn()
    {
        var go = Instantiate(
           otherPrefab,
           transform.position + new Vector3(0, 1, 0),
           Quaternion.identity);

        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    }
    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdSpawn();
        }
    }
}
