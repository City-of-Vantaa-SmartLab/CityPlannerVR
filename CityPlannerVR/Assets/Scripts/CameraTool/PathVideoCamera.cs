using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RockVR.Video;
using RockVR.Vive;
using RockVR.Vive.Demo;

public class PathVideoCamera : MonoBehaviour {

	//Put all the points on the path here
	[HideInInspector]
	public List<GameObject> pathPoints;

	InputMaster inputMaster;
	public CameraProSetUpCtrl cameraProSetUpCtrl;

	public ControllerState controllerState = ControllerState.Normal;
	private CameraState cameraState = CameraState.Normal;

	//This value is got from the cameraHandler that activates this object
	public int myHandNumber;
	private uint myDeviceIndex;

	public GameObject videoCameraScreen;

	public RenderTexture screenTexture;
	public Camera captureCamera;

	[HideInInspector]
	public enum Tool {Add, Move, Remove, Capture};
	[HideInInspector]
	public Tool tool;

	int index = 0;

	float cameraSpeed = 0.5f;

	void Awake(){

		inputMaster = GameObject.Find("Player").GetComponent<InputMaster>();
		captureCamera.targetTexture = screenTexture;
	}

	private void OnEnable()
	{
		//videoCameraScreen.transform.parent = points[myHandNumber - 1].transform;
		//videoCameraScreen.transform.localRotation = Quaternion.identity;

		//Left hand
		//if (myHandNumber == 1)
		//{
		//	videoCameraScreen.transform.localPosition = new Vector3(0.15f, 0, 0);
		//}
		//Right hand
		//else
		//{
		//	videoCameraScreen.transform.localPosition = new Vector3(-0.15f, 0, 0);
		//}


		cameraProSetUpCtrl.EnableCamera();
		//Subscribe();
	}

	private void OnDisable()
	{
		//Unsubscribe();
		//cameraProSetUpCtrl.DisableCamera();
	}

	private void Subscribe()
	{
		if (inputMaster)
		{
			
		}
		else
		{
			Debug.LogError("Did not find inputlistener!");
		}
	}

	private void Unsubscribe()
	{
		if (inputMaster)
		{
			
		}
		else
		{
			Debug.LogError("Did not find inputlistener!");
		}
	}

	private void StartAndStopVideo()
	{
		if (cameraState == CameraState.Normal)
		{
			if (VideoCaptureProCtrl.instance.status == VideoCaptureProCtrl.StatusType.NOT_START ||
				VideoCaptureProCtrl.instance.status == VideoCaptureProCtrl.StatusType.FINISH)
			{
				VideoCaptureProCtrl.instance.StartCapture();
				Debug.Log ("Capture started");
			}
			else if (VideoCaptureProCtrl.instance.status == VideoCaptureProCtrl.StatusType.STARTED)
			{
				VideoCaptureProCtrl.instance.StopCapture();
				Debug.Log ("Capture stopped");
			}
			else if (VideoCaptureProCtrl.instance.status == VideoCaptureProCtrl.StatusType.STOPPED)
			{
				return;
			}
		}
	}

	public void InitializeCamera(){

		if (tool == Tool.Capture) {
			gameObject.SetActive (true);
			transform.position = pathPoints [0].transform.position;
			transform.rotation = pathPoints [0].transform.rotation;

			//Start video capturing
			StartAndStopVideo ();
			StartCoroutine (MoveCamera ());
		}
		
	}

	private IEnumerator MoveCamera(){
		//Camera starts at 0 and its first target is 1
		int targetIndex = 1;

		//while the position of the videoCamera is not the same as the last points position we want to move the camera
		while (transform.position != pathPoints[pathPoints.Count - 1].transform.position)
		{
			Transform targetPoint = pathPoints[targetIndex].transform;

			transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, Time.deltaTime * cameraSpeed);
			transform.rotation = Quaternion.RotateTowards (transform.rotation, targetPoint.rotation, Time.deltaTime * 100f);

			if (transform.position == targetPoint.position)
			{
				targetIndex++;
			}

			yield return null;
		}

		//Stop video capturing
		StartAndStopVideo ();
		yield break;
	}
		

}
