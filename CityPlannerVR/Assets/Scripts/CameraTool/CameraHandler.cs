using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraHandler : MonoBehaviour {

    public GameObject normalCamera;
    public GameObject videoCamera;

    ToolManager toolManager;

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
	public bool VideoCameraModeActive {
		get {
			return videoCameraModeActive;
		}
		private set{ 
			videoCameraModeActive = value;
			videoCamera.SetActive (videoCameraModeActive);
		}
	}
	//-------------------------------------------------------------------------------------------------------------------------------------
    void Awake()
    {
		NormalCameraModeActive = false;
		VideoCameraModeActive = false;

        toolManager = GetComponent<ToolManager>();
        toolManager.OnToolChange += ActivateCameraTool;
		
    }
	//-------------------------------------------------------------------------------------------------------------------------------------
	//Is called when the cameraTool is switched on
	public void ActivateCameraTool(uint deviceIndex, ToolManager.ToolType tool)
	{
        //If camera is selected
        if(tool == ToolManager.ToolType.Camera)
        {
            NormalCameraModeActive = true;
        }

        else if(tool == ToolManager.ToolType.VideoCamera)
        {
            VideoCameraModeActive = true;
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
		NormalCameraModeActive = false;
		VideoCameraModeActive = false;
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
