﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RockVR.Video;
using RockVR.Vive;
using RockVR.Vive.Demo;

public class VideoCamera : MonoBehaviour {

	InputListener inputListener;
    public CameraProSetUpCtrl cameraProSetUpCtrl;

	public ControllerState controllerState = ControllerState.Normal;
	private CameraState cameraState = CameraState.Normal;

	//This value is got from the cameraHandler that activates this object
	public int myHandNumber;
	private uint myDeviceIndex;

    public GameObject videoCameraScreen;

    int index = 0;
    //All the fixed points where the screenshot camera can be (first 2 are in players hands)
    public GameObject[] points;

    void Awake(){

		inputListener = GameObject.Find("Player").GetComponent<InputListener>();
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
            videoCameraScreen.transform.localPosition = new Vector3(0.25f, 0, 0);
        }
        //Right hand
        else
        {
            videoCameraScreen.transform.localPosition = new Vector3(-0.25f, 0, 0);
        }


        cameraProSetUpCtrl.EnableCamera();
		Subscribe();
	}

	private void OnDisable()
	{
        Unsubscribe();
        cameraProSetUpCtrl.DisableCamera();
	}

	private void Subscribe()
	{
		if (inputListener)
		{
			inputListener.TriggerClicked += StartAndStopVideo;

            inputListener.PadClicked += ChangePoint;

            if (myHandNumber == 1)
				inputListener.Hand1DeviceFound += HandleMyIndexFound;
			if (myHandNumber == 2)
				inputListener.Hand2DeviceFound += HandleMyIndexFound;
		}
		else
		{
			Debug.LogError("Did not find inputlistener!");
		}
	}

	private void Unsubscribe()
	{
		if (inputListener)
		{
			inputListener.TriggerClicked -= StartAndStopVideo;

            inputListener.PadClicked -= ChangePoint;

            if (myHandNumber == 1)
				inputListener.Hand1DeviceFound -= HandleMyIndexFound;
			if (myHandNumber == 2)
				inputListener.Hand2DeviceFound -= HandleMyIndexFound;
		}
		else
		{
			Debug.LogError("Did not find inputlistener!");
		}
	}

	private void HandleMyIndexFound(uint deviceIndex)
	{
		myDeviceIndex = deviceIndex;
	}


	private void StartAndStopVideo(object sender, ClickedEventArgs e)
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

    //----------------------------------------------------------------------------------------------------------------------------------------//

    void ChangePoint(object sender, ClickedEventArgs e)
    {

        if ((sender.ToString().Equals("Hand1 (SteamVR_TrackedController)") && myHandNumber == 1) || (sender.ToString().Equals("Hand2 (SteamVR_TrackedController)") && myHandNumber == 2))
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
