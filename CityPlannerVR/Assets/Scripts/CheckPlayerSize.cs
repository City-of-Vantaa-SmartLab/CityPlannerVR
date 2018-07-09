using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks if player is normal size or shrinked down on the table for other scripts
/// </summary>

public class CheckPlayerSize : MonoBehaviour {

    [Tooltip("ScaleToSmallObject")]
    public GameObject scalePlayerToSmall;
    [Tooltip("ScaleToBigObjects")]
    public GameObject scalePlayerToLarge;
    public Camera vrCamera;
    [Tooltip("NearClipping plane when player is small")]
    public float clippingPlaneSmall;
    [Tooltip("NearClipping plane when player is big")]
    public float clippingPlaneBig;
    public GameObject player;
    public GameObject vrcameraObject;

    /// <summary>
    /// Players scale when small
    /// </summary>
    Vector3 smallScale;
    /// <summary>
    /// Players scale when big
    /// </summary>
    Vector3 largeScale;

    private void Start()
    {
        vrcameraObject = GameObject.Find("VRCamera (eye)");
        vrCamera = vrcameraObject.GetComponent<Camera>();
    }
    ///<summary></summary>
    ///are we small
    ///</summary>
    bool isInPedesrianMode = false;
    ///<summary></summary>
    ///are we small (read only)
    ///</summary>
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

    /// <summary>
    /// Check if player is small or big
    /// </summary>
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
