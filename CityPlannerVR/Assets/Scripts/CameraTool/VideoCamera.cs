using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RockVR.Video;
using RockVR.Vive;
using RockVR.Vive.Demo;

public class VideoCamera : MonoBehaviour {

	InputListener inputListener;

	Valve.VR.InteractionSystem.Teleport teleport;

	public ControllerState controllerState = ControllerState.Normal;
	private CameraState cameraState = CameraState.Normal;
	public GameObject applicationMenuButton;

	//This value is got from the cameraHandler that activates this object
	public int myHandNumber;
	private uint myDeviceIndex;

	void Awake(){

		teleport = GameObject.Find("Teleporting").GetComponent<Valve.VR.InteractionSystem.Teleport>();

		inputListener = GameObject.Find("Player").GetComponent<InputListener>();
	}

	private void OnEnable()
	{
		Subscribe();
	}

	private void OnDisable()
	{
		Unsubscribe();
	}

	private void Subscribe()
	{
		if (inputListener)
		{
			teleport.disableTeleport = true;

			inputListener.TriggerClicked += OnPressApplicationMenuDown;

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
			teleport.disableTeleport = false;

			inputListener.TriggerClicked -= OnPressApplicationMenuDown;

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


	private void OnPressApplicationMenuDown(object sender, ClickedEventArgs e)
	{
		Debug.Log ("jotain alkoi");
		if (cameraState == CameraState.Normal)
		{
            Debug.Log("Kamera on normaali");
			if (VideoCaptureProCtrl.instance.status == VideoCaptureProCtrl.StatusType.NOT_START ||
				VideoCaptureProCtrl.instance.status == VideoCaptureProCtrl.StatusType.FINISH)
			{
				VideoCaptureCtrl.instance.StartCapture();
				applicationMenuButton.SetActive(false);
                Debug.Log("jotain");
			}
			else if (VideoCaptureProCtrl.instance.status == VideoCaptureProCtrl.StatusType.STARTED)
			{
				VideoCaptureProCtrl.instance.StopCapture();
                Debug.Log("jotain muuta");
			}
			else if (VideoCaptureProCtrl.instance.status == VideoCaptureProCtrl.StatusType.STOPPED)
			{
                Debug.Log("jotain lisää");
				return;
			}
		}
		Debug.Log ("jotain loppui");
	}
}
