using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Moves the spectator camera.
/// </summary>
public class MoveSpectatorCamera : MonoBehaviour {

	//----------Movement--------------------------

	float speed = 10f;
	Vector3 cameraPosition;

	float wallXmin = -35f;
	float wallXmax = 20f;

	float wallYmin = 0f;
	float wallYmax = 4f;

	float wallZmin = -17f;
	float wallZmax = 16f;

	//--------------------------------------------

	//----------Rotation--------------------------
    float sensitivity = 15f;

    float minY = -60f;
    float maxY = 60f;

    float rotationX;
    float rotationY;
	//--------------------------------------------

	//----------Reset-----------------------------
	Vector3 defaultPosition = new Vector3(-19, 4, 0);
	Vector3 defaultRotation = new Vector3(0, 90, 0);
	//--------------------------------------------

	void Start(){
		cameraPosition = defaultPosition;
	}
    void Update() {

        RotateCamera();
        MoveCamera ();
		ResetCamera ();
    }

	/// <summary>
	/// Moves the spectator camera.
	/// </summary>
    void MoveCamera()
    {
        if (Input.GetKey(KeyCode.W))
        {
			transform.Translate(Vector3.forward * speed * Time.deltaTime);
			//RestrictCamera ();
        }
        if (Input.GetKey(KeyCode.S))
        {
			transform.Translate(-Vector3.forward * speed * Time.deltaTime);
			//RestrictCamera ();
        }
        if (Input.GetKey(KeyCode.A))
        {
			transform.Translate(Vector3.left * speed * Time.deltaTime);
			//RestrictCamera ();
        }
        if (Input.GetKey(KeyCode.D))
        {
			transform.Translate(-Vector3.left * speed * Time.deltaTime);
			//RestrictCamera ();
        }
    }

	/// <summary>
	/// Prevents the camera to go through walls
	/// </summary>
	void RestrictCamera(){
		if ((transform.position.x < wallXmin) || (transform.position.x > wallXmax)) {
			transform.position = cameraPosition;
		}
		else if ((transform.position.y < wallYmin) || (transform.position.y > wallYmax)) {
			transform.position = cameraPosition;
		}
		else if ((transform.position.z < wallZmin) || (transform.position.z > wallZmax)) {
			transform.position = cameraPosition;
		}
		else {
			cameraPosition = transform.position;
		}
	}

	/// <summary>
	/// Rotates the spectator camera.
	/// </summary>
	void RotateCamera()
	{
		rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
		rotationY += Input.GetAxis("Mouse Y") * sensitivity;
		rotationY = Mathf.Clamp(rotationY, minY, maxY);

		transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
	}

	/// <summary>
	/// Resets the camera position and rotation.
	/// </summary>
	void ResetCamera(){
		if (Input.GetKey (KeyCode.R)) {
			transform.position = defaultPosition;
			transform.eulerAngles = defaultRotation;
		}
	}
}
