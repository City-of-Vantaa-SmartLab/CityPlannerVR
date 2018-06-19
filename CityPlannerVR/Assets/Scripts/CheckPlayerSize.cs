using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks if player is normal size or shrinked down on the table for other scripts
/// </summary>

public class CheckPlayerSize : MonoBehaviour {

    public GameObject scalePlayerToSmall;
    public GameObject scalePlayerToLarge;
    public Camera vrCamera;
    public float clippingPlaneSmall;
    public float clippingPlaneBig;
    public GameObject player;
    public GameObject vrcameraObject;

    Vector3 smallScale;
    Vector3 largeScale;

    private void Start()
    {
        vrcameraObject = GameObject.Find("VRCamera (eye)");
        vrCamera = vrcameraObject.GetComponent<Camera>();
    }
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
		//														the first element is players scale (or should be)
        smallScale = scalePlayerToSmall.GetComponent<ScaleObject>().newScales[0];
		largeScale = scalePlayerToLarge.GetComponent<ScaleObject>().newScales[0];
    }

    void CalculateMode()
    {
        if (transform.localScale == smallScale)
        {
            isInPedesrianMode = true;
            vrCamera.nearClipPlane = clippingPlaneSmall;


        }
        else if (transform.localScale == largeScale)
        {
            isInPedesrianMode = false;
            vrCamera.nearClipPlane = clippingPlaneBig;
        }
        //Just in case
        else
        {
            Debug.LogError("There is something wrong in CheckPlayerSize");
        }
    }
}
