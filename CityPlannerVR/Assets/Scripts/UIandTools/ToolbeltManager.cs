using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Locks toolbelt in place when looked at. Also fades the toolbelt when it is not needed.
/// </summary>

public class ToolbeltManager : MonoBehaviour {

    public int lockAngle;
    //public bool faded;
    public bool lockWhenLookingUp = true;
    private bool locked;
    private bool resetPosition = false;
    
    private Transform playerCameraTransform;
    private Transform originalParentTransform;
    private Vector3 originalPos;
    private Quaternion originalRot;



    // Use this for initialization
    void Start () {
        Initialize();
    }
	
	// Update is called once per frame
	void Update () {
        if (lockWhenLookingUp)
        {
            if (playerCameraTransform)
            {
                Vector3 temp;
                temp = playerCameraTransform.TransformDirection(Vector3.forward);
                temp = Quaternion.AngleAxis(-lockAngle, Vector3.right) * temp; //rotates the temp vector downwards
                if (Vector3.Dot(temp, Vector3.up) > 0)  //projects temp to y axis
                    locked = true;
                else
                    locked = false;
            }

            if (locked && !resetPosition)
            {
                //transform.parent = transform.root; //will move with player, but not rotate
                transform.parent = transform.root.parent; //will stay in place
                resetPosition = true;
            }
            else if (!locked && resetPosition)
            {
                transform.parent = originalParentTransform;
                transform.localPosition = originalPos;
                transform.localRotation = originalRot;
                resetPosition = false;
            }
        }
	}

    private void Initialize()
    {
        if (!playerCameraTransform)
        {
            GameObject player = GameObject.Find("Player");
            SteamVR_Camera camera = player.GetComponentInChildren<SteamVR_Camera>();
            if (camera)
                playerCameraTransform = camera.transform;
            else
                Debug.Log("ToolbeltManager initalization failed!");
        }
        if (lockAngle == 0)
        {
            lockAngle = 20;
        }

        if (!originalParentTransform)
            originalParentTransform = transform.parent;
        originalPos = transform.localPosition;
        originalRot = transform.localRotation;
    }

}
