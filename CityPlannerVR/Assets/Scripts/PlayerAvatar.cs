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

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Debug.Log("MirrorNetworkedVRNode::OnStartServer: Object " + this.gameObject.name + ":" + this.gameObject.GetInstanceID().ToString() + " coming active!");
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
        Debug.Log("asd");

        left.GetComponent<HandPositionSetter>().TargetSetHand(connectionToClient, VRNode.LeftHand);
        right.GetComponent<HandPositionSetter>().TargetSetHand(connectionToClient, VRNode.RightHand);
    }


    IEnumerator TrackHeadCoroutine()
    {
        Debug.Log("MirrorNetworkedVRNode::TrackHeadCoroutine: Start headtracking!");
        while (true)
        {
            transform.rotation = InputTracking.GetLocalRotation(node);
            transform.position = InputTracking.GetLocalPosition(node);
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
