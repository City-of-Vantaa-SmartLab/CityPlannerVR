using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks if player is normal size or shrinked down on the table for other scripts
/// </summary>

public class CheckPlayerSize : MonoBehaviour {

    public GameObject scalePlayerToSmall;
    public GameObject scalePlayerToLarge;

    Vector3 smallScale;
    Vector3 largeScale;

    //are we small
    bool isInPedesrianMode = false;
    public bool isSmall
    {
        get
        {
            CalculateMode();
            return isInPedesrianMode;
        }
    }

    private void Awake()
    {
        smallScale = scalePlayerToSmall.GetComponent<ScaleObject>().NewScale;
        largeScale = scalePlayerToLarge.GetComponent<ScaleObject>().NewScale;
    }

    void CalculateMode()
    {
        if (transform.localScale == smallScale)
        {
            isInPedesrianMode = true;
        }
        else if (transform.localScale == largeScale)
        {
            isInPedesrianMode = false;
        }
        //Just in case
        else
        {
            Debug.LogError("There is something wrong in CheckPlayerSize");
        }
    }
}
