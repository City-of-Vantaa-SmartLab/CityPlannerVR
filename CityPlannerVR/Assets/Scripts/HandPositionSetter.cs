using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.VR;
using Valve.VR.InteractionSystem;

/// <summary>
/// Description: Sets the transform of player avatar hands correctly
/// </summary>


public class HandPositionSetter : NetworkBehaviour
{
    private GameObject playerVR;
    private CheckPlayerSize playerSize;

    private Hand hand1;
    private Hand hand2;

    [SyncVar(hook = "HookScaleHands")]
    public Vector3 objScale;

    int buildingLayer = 9;
    int finalMask;

    [TargetRpc]
    public void TargetSetHand(NetworkConnection target, UnityEngine.XR.XRNode node)
    {
        // Get gameobject handling player VR stuff
        playerVR = GameObject.FindGameObjectWithTag("Player");
        playerSize = playerVR.GetComponent<CheckPlayerSize>();

        hand1 = GameObject.Find("Player/SteamVRObjects/Hand1").GetComponent<Hand>(); ;
        hand2 = GameObject.Find("Player/SteamVRObjects/Hand2").GetComponent<Hand>();

        StartCoroutine(TrackNodeCoroutine(node));
        //Debug.Log("HandPositionSetter::TargetSetHand: Hand set");

        int layerMask = 1 << buildingLayer;
        finalMask = ~layerMask;
    }

    IEnumerator TrackNodeCoroutine(UnityEngine.XR.XRNode node)
    {
        while (true)
        {
            transform.rotation = UnityEngine.XR.InputTracking.GetLocalRotation(node);

            //Scaling the playerVR (Player) down causes some funky stuff with the position calculations. These checks are needed to counter that.
            //(the check values are hardcoded for now. Solution can be changed if there is time).


             //Check if we are in pedestrian mode (small)
            if(playerSize.isSmall)
            {
                //                                                                                                       all the axes are same for scale, so no matter which one is used. (If they're not, something is wrong and it should be fixed)
                transform.position = playerVR.transform.position + UnityEngine.XR.InputTracking.GetLocalPosition(node) * playerVR.transform.localScale.x;
                hand1.hoverLayerMask = finalMask;
                hand2.hoverLayerMask = finalMask;
            }

            //Check if we are in god mode (big)
            else
            {
                transform.position = playerVR.transform.position + UnityEngine.XR.InputTracking.GetLocalPosition(node);
                hand1.hoverLayerMask = -1;
                hand2.hoverLayerMask = -1;

            }

            CmdScaleHands(playerVR.transform.localScale * 0.07f);

            yield return null;
        }
    }

    //Does the hand scaling on the server
    [Command]
    void CmdScaleHands(Vector3 scale)
    {
        objScale = scale;
        transform.localScale = objScale;
    }

    //Is used for the syncVar to work correctly
    void HookScaleHands(Vector3 scale)
    {
        objScale = scale;
        transform.localScale = objScale;
    }
}
