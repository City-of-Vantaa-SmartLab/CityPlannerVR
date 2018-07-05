using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RockVR.Video;
using RockVR.Vive;
using RockVR.Vive.Demo;


/// <summary>
/// For taking video footage in game
/// </summary>
public class VideoCamera : MonoBehaviour {

	InputMaster inputMaster;
    public CameraProSetUpCtrl cameraProSetUpCtrl;

    /// <summary>
    /// The controller state of the video camera
    /// </summary>
	public ControllerState controllerState = ControllerState.Normal;
    /// <summary>
    /// The camera state of the video camera
    /// </summary>
	private CameraState cameraState = CameraState.Normal;

    /// <summary>
    /// This value is got from the cameraHandler that activates this object
    /// </summary>
	public int myHandNumber;

    /// <summary>
    /// The object which shows to the player what camera is seeing
    /// </summary>
    public GameObject videoCameraScreen;

    /// <summary>
    /// RenderTexture for showing to the player what the camera sees
    /// </summary>
    public RenderTexture screenTexture;
    /// <summary>
    /// The camera component of this object
    /// </summary>
    public Camera captureCamera;

    /// <summary>
    /// Index for the video camera point changing
    /// </summary>
    int index = 0;
    /// <summary>
    /// All the fixed points where the screenshot camera can be (first 2 are in players hands)
    /// </summary>
    public GameObject[] points;

    void Awake(){

		inputMaster = GameObject.Find("Player").GetComponent<InputMaster>();
        captureCamera.targetTexture = screenTexture;
	}

	private void OnEnable()
	{
        //                            myHandNumber is 1 or 2, but the place for them in the array are 0 and 1
        gameObject.transform.parent = points[myHandNumber - 1].transform;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;

        videoCameraScreen.transform.parent = points[myHandNumber - 1].transform;
        videoCameraScreen.transform.localRotation = Quaternion.identity;

        //Left hand
        if (myHandNumber == 1)
        {
            videoCameraScreen.transform.localPosition = new Vector3(0.15f, 0, 0);
        }
        //Right hand
        else
        {
            videoCameraScreen.transform.localPosition = new Vector3(-0.15f, 0, 0);
        }


        cameraProSetUpCtrl.EnableCamera();
		Subscribe();
	}

	private void OnDisable()
	{
        videoCameraScreen.SetActive(false);
        Unsubscribe();
        //cameraProSetUpCtrl.DisableCamera();
	}

    /// <summary>
    /// Subscribe to input masters events
    /// </summary>
	private void Subscribe()
	{
		if (inputMaster)
		{
			inputMaster.TriggerClicked += StartAndStopVideo;
            inputMaster.PadClicked += ChangePoint;
		}
		else
		{
			Debug.LogError("Did not find inputlistener!");
		}
	}

    /// <summary>
    /// Unsubscribe from input masters events
    /// </summary>
	private void Unsubscribe()
	{
		if (inputMaster)
		{
			inputMaster.TriggerClicked -= StartAndStopVideo;
            inputMaster.PadClicked -= ChangePoint;
		}
		else
		{
			Debug.LogError("Did not find inputlistener!");
		}
	}
    /// <summary>
    /// Start and stop capturing video
    /// </summary>
    /// <param name="sender">The controller which give the info</param>
    /// <param name="e">The info from the controller</param>
	private void StartAndStopVideo(object sender, ClickedEventArgs e)
	{
        if (e.controllerIndex == myHandNumber)
        {
            if (cameraState == CameraState.Normal)
            {
                if (VideoCaptureProCtrl.instance.status == VideoCaptureProCtrl.StatusType.NOT_START ||
                    VideoCaptureProCtrl.instance.status == VideoCaptureProCtrl.StatusType.FINISH)
                {
                    VideoCaptureProCtrl.instance.StartCapture();
                }
                else if (VideoCaptureProCtrl.instance.status == VideoCaptureProCtrl.StatusType.STARTED)
                {
                    VideoCaptureProCtrl.instance.StopCapture();
                }
                else if (VideoCaptureProCtrl.instance.status == VideoCaptureProCtrl.StatusType.STOPPED)
                {
                    return;
                }
            }
        }
	}

    //----------------------------------------------------------------------------------------------------------------------------------------//
    /// <summary>
    /// Change the point where the camera is 
    /// </summary>
    /// <param name="sender">The controller which give the info</param>
    /// <param name="e">The info from the controller</param>
    void ChangePoint(object sender, ClickedEventArgs e)
    {
        if (e.controllerIndex == myHandNumber)
        {
            if (e.padX > 0.7f)
            {
                ChangePointRight();
            }
            else if (e.padX < -0.7f)
            {
                ChangePointLeft();
            }
        }
    }

    /// <summary>
    /// Change the point where camera is to the right in the array
    /// </summary>
    void ChangePointRight()
    {
        if (index >= points.Length - 1)
        {
            //We have looped around
            index = 0;
        }
        else
        {
            index++;
        }

        Debug.Log("index is " + index);

        gameObject.transform.parent = points[index].transform;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// Change the point where camera is to the left in the array 
    /// </summary>
    void ChangePointLeft()
    {
        if (index <= 0)
        {
            //We have looped around
            index = points.Length - 1;
        }
        else
        {
            index--;
        }

        Debug.Log("index is " + index);

        gameObject.transform.parent = points[index].transform;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;
    }
}
