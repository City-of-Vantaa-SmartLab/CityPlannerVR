using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the activation and deactivation of the Screenshot camera and the video camera
/// </summary>

public class CameraHandler : MonoBehaviour {

	private enum CameraMode {ScreenshotCamera, VideoCamera, PathCamera}
	private CameraMode cameraMode = CameraMode.ScreenshotCamera;

    public GameObject normalCamera;
    public GameObject videoCameraObject;
	public GameObject pathCameraPoint;

    ScreenshotCamera screenshotCamera;
    VideoCamera videoCamera;
	CameraPathHandler pathCameraHandler;

    //Needed to disable the teleport temporarily so it won't interfere with the camera controls
    Valve.VR.InteractionSystem.Teleport teleport;

    ToolManager toolManager;
    int handNumber;

	//-------------------------------------------------------------------------------------------------------------------------------------
	private bool normalCameraModeActive = false;
	public bool NormalCameraModeActive {
		get {
			return normalCameraModeActive;
		}
		private set{ 
			normalCameraModeActive = value;
			normalCamera.SetActive (normalCameraModeActive);
		}
	}
    //-------------------------------------------------------------------------------------------------------------------------------------
    private bool videoCameraModeActive = false;
    public bool VideoCameraModeActive
    {
        get
        {
            return videoCameraModeActive;
        }
        private set
        {
            videoCameraModeActive = value;
            videoCameraObject.SetActive (videoCameraModeActive);
        }
    }
    //-------------------------------------------------------------------------------------------------------------------------------------
	private bool pathPointModeActive = false;
	public bool PathPointModeActive
	{
		get
		{ 
			return pathPointModeActive;
		}
		private set
		{ 
			pathPointModeActive = value;
			pathCameraPoint.SetActive (pathPointModeActive);
		}
	}

    void Awake()
    {
		NormalCameraModeActive = false;
		VideoCameraModeActive = false;
		PathPointModeActive = false;

        toolManager = GetComponent<ToolManager>();
        toolManager.AnnounceToolChanged += ActivateCameraTool;
        handNumber = toolManager.myHandNumber;

        screenshotCamera = normalCamera.GetComponent<ScreenshotCamera>();
        videoCamera = videoCameraObject.GetComponent<VideoCamera>();
        pathCameraHandler = pathCameraPoint.GetComponent<CameraPathHandler>();

        teleport = GameObject.Find("Teleporting").GetComponent<Valve.VR.InteractionSystem.Teleport>();

    }
	//-------------------------------------------------------------------------------------------------------------------------------------
	//Is called when the cameraTool is switched on
	public void ActivateCameraTool(uint deviceIndex, ToolManager.ToolType tool)
	{
        //If screenshot camera is selected
		if (tool == ToolManager.ToolType.Camera) {
			//When camera is activated we give it the number of the hand that activated it
			screenshotCamera.myHandNumber = handNumber;
			NormalCameraModeActive = true;
			VideoCameraModeActive = false;
			PathPointModeActive = false;

			teleport.disableTeleport = true;
		}

        //if video camera is selected
        else if (tool == ToolManager.ToolType.VideoCamera) {
			videoCamera.myHandNumber = handNumber;
			VideoCameraModeActive = true;
			NormalCameraModeActive = false;
			PathPointModeActive = false;

			teleport.disableTeleport = true;
		} 
		//if path camera is selected
		else if (tool == ToolManager.ToolType.PathCamera) {
			pathCameraHandler.myHandNumber = handNumber;
			PathPointModeActive = true;
			NormalCameraModeActive = false;
			VideoCameraModeActive = false;
		}
        //if neither camera is selected
        else
        {
            DeactivateCameraTool();
        }
	}
	//-------------------------------------------------------------------------------------------------------------------------------------
	//Is called when the cameraTool is switched off
	public void DeactivateCameraTool()
	{
        if (toolManager.myHandNumber == screenshotCamera.myHandNumber)
        {
            NormalCameraModeActive = false;
        }

        VideoCameraModeActive = false;
        teleport.disableTeleport = false;
		PathPointModeActive = false;
    }
}
