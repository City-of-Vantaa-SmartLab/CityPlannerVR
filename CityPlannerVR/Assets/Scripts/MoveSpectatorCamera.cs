using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO: liikkuminen niin, että eteenpäin on se mihin kamera katsoo
public class MoveSpectatorCamera : MonoBehaviour {

	//----------Movement--------------------------

	float speed = 10f;

	//--------------------------------------------

	//----------Rotation--------------------------
    float sensitivity = 15f;

    float minY = -60f;
    float maxY = 60f;

    float rotationX;
    float rotationY;
	//--------------------------------------------

    void Update() {

		MoveCamera ();
        RotateCamera();
    }

	/// <summary>
	/// Moves the spectator camera.
	/// </summary>
    void MoveCamera()
    {
		if (Input.GetKey (KeyCode.W)) {
			transform.localPosition += Vector3.forward * Time.deltaTime * speed ;
		}
		if (Input.GetKey (KeyCode.S)) {
			transform.localPosition += -Vector3.forward * speed * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.A)) {
			transform.localPosition += Vector3.left * speed * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.D)) {
			transform.localPosition += -Vector3.left * speed * Time.deltaTime;
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
}
