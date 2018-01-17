using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.VR;

/// <summary>
/// Description: Sets the transform of player avatar hands correctly
/// TODO: Objects only scales correctly for local player.
/// </summary>


public class HandPositionSetter : NetworkBehaviour
{
    private GameObject playerVR;

    IEnumerator TrackNodeCoroutine(UnityEngine.XR.XRNode node)
    {
        while (true)
        {
            //        transform.rotation = UnityEngine.XR.InputTracking.GetLocalRotation(node);
            //         TODO: Position is incorrect when scaling player avatar
            //        transform.position = playerVR.transform.position + UnityEngine.XR.InputTracking.GetLocalPosition(node);
            //         TODO: Unity does not seem to automatically network object scale.
            //Have to do it "manually". Network the scaling like in player avatar / ScaleObject.cs.
            //Basically instead of client changing the object scale, tell the server to change the object scale.
            //        transform.localScale = playerVR.transform.localScale * 0.07f;


            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            

            yield return null;
        }
    }

    [TargetRpc]
    public void TargetSetHand(NetworkConnection target, UnityEngine.XR.XRNode node)
    {
        // Get gameobject handling player VR stuff
        playerVR = GameObject.FindGameObjectWithTag("Player");

        StartCoroutine(TrackNodeCoroutine(node));
        //Debug.Log("HandPositionSetter::TargetSetHand: Hand set");
    }
}
