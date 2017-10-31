using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.VR;

public class HandPositionSetter : NetworkBehaviour
{
    IEnumerator TrackNodeCoroutine(VRNode node)
    {
        while (true)
        {
            transform.rotation = InputTracking.GetLocalRotation(node);
            transform.position = InputTracking.GetLocalPosition(node);
            yield return null;
        }
    }

    [TargetRpc]
    public void TargetSetHand(NetworkConnection target, VRNode node)
    {
        StartCoroutine(TrackNodeCoroutine(node));
        Debug.Log("RpcSetHand");
    }
}
