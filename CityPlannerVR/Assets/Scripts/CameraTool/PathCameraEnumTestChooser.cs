using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCameraEnumTestChooser : MonoBehaviour {

	public PathVideoCamera pathVideoCamera;

	float xPos;
	float yPos;
	float zPos;

	float newYPos;

	void Start(){
		xPos = transform.position.x;
		yPos = transform.position.y;
		zPos = transform.position.z;
		newYPos = yPos + 1f;
	}

	void OnCollisionEnter(Collision other){

		if (gameObject.name == "Add") {
			pathVideoCamera.tool = PathVideoCamera.Tool.Add;
		} else if (gameObject.name == "Move") {
			pathVideoCamera.tool = PathVideoCamera.Tool.Move;
		} else if (gameObject.name == "Remove") {
			pathVideoCamera.tool = PathVideoCamera.Tool.Remove;
		} else if (gameObject.name == "Capture") {
			pathVideoCamera.tool = PathVideoCamera.Tool.Capture;
		}
		Debug.Log("Tool at the moment: " + pathVideoCamera.tool);
	}
}
