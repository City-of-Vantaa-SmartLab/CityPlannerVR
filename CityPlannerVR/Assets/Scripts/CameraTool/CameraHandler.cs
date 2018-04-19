using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraHandler : MonoBehaviour {

    public GameObject normalCamera;
    public GameObject videoCameraObject;

    ScreenshotCamera screenshotCamera;
    VideoCamera videoCamera;

    
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
    void Awake()
    {
		NormalCameraModeActive = false;
		VideoCameraModeActive = false;

        toolManager = GetComponent<ToolManager>();
        toolManager.OnToolChange += ActivateCameraTool;
        handNumber = toolManager.myHandNumber;

        screenshotCamera = normalCamera.GetComponent<ScreenshotCamera>();
        videoCamera = videoCameraObject.GetComponent<VideoCamera>();

    }
	//-------------------------------------------------------------------------------------------------------------------------------------
	//Is called when the cameraTool is switched on
	public void ActivateCameraTool(uint deviceIndex, ToolManager.ToolType tool)
	{
        //If camera is selected
        if(tool == ToolManager.ToolType.Camera)
        {
            //When camera is activated we give it the number of the hand that activated it
            screenshotCamera.myHandNumber = handNumber;
            NormalCameraModeActive = true;
            VideoCameraModeActive = false;
        }

        else if (tool == ToolManager.ToolType.VideoCamera)
        {
            videoCamera.myHandNumber = handNumber;
            VideoCameraModeActive = true;
            NormalCameraModeActive = false;
        }
        //if camera is not selected
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
            VideoCameraModeActive = false;
        }
	}
	//-------------------------------------------------------------------------------------------------------------------------------------
	//Switches between normal camera and video camera
	//public void SwitchCameras(){
	//	//If normal camera is active disable it and enable videoCamera
	//	if (NormalCameraModeActive == true)
	//	{
	//		NormalCameraModeActive = false;
	//		VideoCameraModeActive = true;
	//	} 

	//	//Otherwise enable normal camera and disable video camera
	//	else 
	//	{
	//		NormalCameraModeActive = true;
	//		VideoCameraModeActive = false;
	//	}
	//}
    //-------------------------------------------------------------------------------------------------------------------------------------
}
