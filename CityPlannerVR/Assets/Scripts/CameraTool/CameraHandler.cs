using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraHandler : MonoBehaviour {

    public GameObject normalCamera;
    public GameObject videoCamera;
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

		
    }
	//-------------------------------------------------------------------------------------------------------------------------------------

	//Function that keeps track of if cameras need to be active or not (used for the subscription)
	void IsActive(){
		//Aktivoi ja deaktivoi kamerat sitä mukaan kun CameraTool valitaan tai otetaan pois
		//if(kamera on aktiivinen){
		//	ActivateCameraTool(); 
		//}
		//else{
		//	DeactivateCameraTool();
		//}

		//Subscribaa ehkä input manageriin tässä (switchCameras vaikka)
	}

	//-------------------------------------------------------------------------------------------------------------------------------------
	//Is called when the cameraTool is switched on
	public void ActivateCameraTool()
	{
		NormalCameraModeActive = true;
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
	public void SwitchCameras(){
		//If normal camera is active disable it and enable videoCamera
		if (NormalCameraModeActive == true)
		{
			NormalCameraModeActive = false;
			VideoCameraModeActive = true;
		} 

		//Otherwise enable normal camera and disable video camera
		else 
		{
			NormalCameraModeActive = true;
			VideoCameraModeActive = false;
		}
	}
	//-------------------------------------------------------------------------------------------------------------------------------------
}
