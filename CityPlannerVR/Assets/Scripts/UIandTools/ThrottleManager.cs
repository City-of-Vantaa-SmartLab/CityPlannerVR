using System;
using UnityEngine;
using Valve.VR.InteractionSystem;

/// <summary>
/// 
/// </summary>

public class ThrottleManager : MonoBehaviour {

    public CircularDrive drive;
    public InputMaster inputMaster;
    public float driveAngle;
    public float normalAngle;
    public bool holding;

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        
        if (drive)
        {

            if (holding)
            {
                driveAngle = drive.outAngle;

            }
            else
            {
                ResetPosition();
                holding = true;
            }

        }

    }

    private void Initialize()
    {
        if (!drive)
            drive = gameObject.GetComponent<CircularDrive>();
        if (!drive)
            Debug.LogError("Circulardrive not found!");
        normalAngle = drive.startAngle;
    }

    public void ResetPosition()
    {
        driveAngle = normalAngle;
        drive.outAngle = normalAngle; //replace with lerp later



        float x = drive.transform.eulerAngles.x;
        float y = drive.transform.eulerAngles.y;
        float z = drive.transform.eulerAngles.z;

        //reset visuals as well
        switch (drive.axisOfRotation)
        {
            case CircularDrive.Axis_t.XAxis:
                drive.transform.eulerAngles = new Vector3(normalAngle, y, z);
                break;
            case CircularDrive.Axis_t.YAxis:
                drive.transform.eulerAngles = new Vector3(x, normalAngle, z);
                break;
            case CircularDrive.Axis_t.ZAxis:
                drive.transform.eulerAngles = new Vector3(x, y, normalAngle);
                break;
            default:
                break;
        }
    }

    //private void OnColliderEnter(Collider other)
    //{
    //    if (other.CompareTag("GameController"))  //a tag that is in hand!
    //    {
    //        holding = true;
    //    }
    //}

    //private void OnColliderExit(Collider other)
    //{
    //    if (other.CompareTag("GameController"))
    //    {
    //        ResetPosition();
    //        holding = false;
    //    }
    //}
}
