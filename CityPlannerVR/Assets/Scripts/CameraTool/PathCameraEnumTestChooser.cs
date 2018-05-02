using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCameraEnumTestChooser : MonoBehaviour {

	public PathVideoCamera pathVideoCamera;

	public GameObject pathCamera;
	public GameObject pathCameraScreen;

	void OnTriggerEnter(Collider other){

		if (gameObject.name == "Add") {
			pathVideoCamera.tool = PathVideoCamera.Tool.Add;
			pathCamera.SetActive (false);
			pathCameraScreen.SetActive (false);
		} 

		else if (gameObject.name == "Move") {
			pathVideoCamera.tool = PathVideoCamera.Tool.Move;
			pathCamera.SetActive (false);
			pathCameraScreen.SetActive (false);
		} 

		else if (gameObject.name == "Remove") {
			pathVideoCamera.tool = PathVideoCamera.Tool.Remove;
			pathCamera.SetActive (false);
			pathCameraScreen.SetActive (false);
		} 

		else if (gameObject.name == "Capture") {
			pathVideoCamera.tool = PathVideoCamera.Tool.Capture;
		}
		Debug.Log("Tool at the moment: " + pathVideoCamera.tool);
	}
}
