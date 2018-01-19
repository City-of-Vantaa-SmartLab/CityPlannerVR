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

    [SyncVar(hook = "HookScaleHands")]
    public Vector3 objScale;

    [TargetRpc]
    public void TargetSetHand(NetworkConnection target, UnityEngine.XR.XRNode node)
    {
        // Get gameobject handling player VR stuff
        playerVR = GameObject.FindGameObjectWithTag("Player");

        StartCoroutine(TrackNodeCoroutine(node));
        //Debug.Log("HandPositionSetter::TargetSetHand: Hand set");
    }

    IEnumerator TrackNodeCoroutine(UnityEngine.XR.XRNode node)
    {
        while (true)
        {
            transform.rotation = UnityEngine.XR.InputTracking.GetLocalRotation(node);

            //Scaling the playerVR (Player) down causes some funky stuff with the position calculations. These checks are needed to counter that.
            //(the check values are hardcoded for now. Solution can be changed if there is time).

            //Check if we are in god mode (big)
            if (playerVR.transform.localScale == new Vector3(1, 1, 1))
            {
                transform.position = playerVR.transform.position + UnityEngine.XR.InputTracking.GetLocalPosition(node);
            }

            //Check if we are in pedestrian mode (small)
            else if (playerVR.transform.localScale == new Vector3(0.025f, 0.025f, 0.025f))
            {
                //                                                                                                       all the axes are same for scale, so no matter which one is used. (If they're not, something is wrong and it should be fixed)
                transform.position = playerVR.transform.position + UnityEngine.XR.InputTracking.GetLocalPosition(node) * playerVR.transform.localScale.x;
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
