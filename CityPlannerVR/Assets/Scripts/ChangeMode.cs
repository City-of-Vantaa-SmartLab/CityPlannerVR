using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks if we are in god mode or pedestrian mode and tells it to other scripts so they can work properly
/// </summary>

public class ChangeMode : MonoBehaviour {

    private bool isInPedestrianMode = false;

    private Vector3 playerSize;

    public void CheckPlayerMode()
    {
        playerSize = transform.localScale;

        if (playerSize == new Vector3(1, 1, 1))
        {
            //We are in god mode (big)
            isInPedestrianMode = false;
        }

        else if (playerSize == new Vector3(0.025f, 0.025f, 0.025f))
        {
            //We are in pedestrian mode (small)
            isInPedestrianMode = true;
        }
        else
        {
            //Because the values are hardcoded for now, if something is changed and forgotten to update here, fire an error
            //(Just in case)
            Debug.LogError("Player size in pedestrian or god mode has been changed. Update to 'ChangeMode.cs' the new size");
        }
    }

    public bool GetIsInPedestrianMode()
    {
        return isInPedestrianMode;
    }
}
