using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.VR;

public class HandPositionSetter : NetworkBehaviour
{
    private GameObject playerVR;

    IEnumerator TrackNodeCoroutine(UnityEngine.XR.XRNode node)
    {
        while (true)
        {
            transform.rotation = UnityEngine.XR.InputTracking.GetLocalRotation(node);
            transform.position = playerVR.transform.position + UnityEngine.XR.InputTracking.GetLocalPosition(node);
            yield return null;
        }
    }

    [TargetRpc]
    public void TargetSetHand(NetworkConnection target, UnityEngine.XR.XRNode node)
    {
        // Get gameobject handling player VR stuff
        playerVR = GameObject.FindGameObjectWithTag("Player");

        StartCoroutine(TrackNodeCoroutine(node));
        Debug.Log("RpcSetHand");
    }
}
