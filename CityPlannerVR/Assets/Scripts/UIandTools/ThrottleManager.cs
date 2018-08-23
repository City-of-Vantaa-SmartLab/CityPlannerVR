using UnityEngine;
using Valve.VR.InteractionSystem;

/// <summary>
/// 
/// </summary>

public class ThrottleManager : MonoBehaviour {

    public CircularDrive drive;
    public LinearMapping linearMapping;
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
        }
    }

    private void Initialize()
    {
        if (!drive)
            drive = gameObject.GetComponent<CircularDrive>();
        if (!drive)
            Debug.LogError("Circulardrive not found!");
        if (!linearMapping)
            linearMapping = gameObject.GetComponent<LinearMapping>();
        if (!drive)
            Debug.LogError("Linearmapping not found!");
        //normalAngle = drive.startAngle;
    }

    public void ResetPosition()
    {
        driveAngle = normalAngle;
        drive.outAngle = normalAngle;
        float x = drive.transform.eulerAngles.x;
        float y = drive.transform.eulerAngles.y;
        float z = drive.transform.eulerAngles.z;

        //reset visuals as well, replace with lerp later?
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

    private void OnTriggerEnter(Collider other) { 
        if (other.CompareTag("GameController"))  //a tag that is in hand!
        {
            holding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            ResetPosition();
            holding = false;
        }
    }
}
