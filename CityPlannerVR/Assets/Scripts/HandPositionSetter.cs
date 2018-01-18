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

    private Vector3 objScale;
    

    IEnumerator TrackNodeCoroutine(UnityEngine.XR.XRNode node)
    {
        while (true)
        {
            //--------------------------------------------------------------------------------------------------------------------------------------

            transform.rotation = UnityEngine.XR.InputTracking.GetLocalRotation(node);

            if (playerVR.transform.localScale == new Vector3(1, 1, 1))
            {
                transform.position = playerVR.transform.position + UnityEngine.XR.InputTracking.GetLocalPosition(node);
            }

            else if (playerVR.transform.localScale == new Vector3(0.025f, 0.025f, 0.025f))
            {
                transform.position = playerVR.transform.position + UnityEngine.XR.InputTracking.GetLocalPosition(node) * 0.025f;
            }

            //--------------------------------------------------------------------------------------------------------------------------------------

            //TESTAA TÄTÄ JA TOISTA

            //--------------------------------------------------------------------------------------------------------------------------------------
            transform.localScale = playerVR.transform.localScale * 0.07f;
            if (isLocalPlayer)
            {
                CmdScaleHands(transform.localScale);
            }
            //--------------------------------------------------------------------------------------------------------------------------------------
            //if (isLocalPlayer)
            //{
            //    CmdScaleHands(playerVR.transform.localScale * 0.07f);
            //}
            //--------------------------------------------------------------------------------------------------------------------------------------

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

    [Command]
    public void CmdScaleHands(Vector3 newScale)
    {
        objScale = newScale;
        transform.localScale = objScale;
    }
}
