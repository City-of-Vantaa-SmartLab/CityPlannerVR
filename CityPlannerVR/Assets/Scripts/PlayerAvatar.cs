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

    private Component[] avatarMaterials;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Debug.Log("MirrorNetworkedVRNode::OnStartServer: Object " + this.gameObject.name + ":" + this.gameObject.GetInstanceID().ToString() + " coming active!");
        StartCoroutine(TrackHeadCoroutine());
        StartCoroutine(MakeSureSetHand());

        avatarMaterials = GetComponentsInChildren<Renderer>();
        if (avatarMaterials == null)
        {
            Debug.LogError("PlayerColorManager::Start: AvatarMaterials is null!");
        }
        GetRandomColor();
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

    private void GetRandomColor()

    {

        float h, s;

        h = Random.Range(0.0f, 1.0f);
        s = Random.Range(0.0f, 5.0f);

        if (s > 1.0f)
        {
            s = 1.0f;
        }

        foreach (Renderer renderer in avatarMaterials)
        {
            renderer.material.color = Color.HSVToRGB(h, s, 1);

        }

    }
}
