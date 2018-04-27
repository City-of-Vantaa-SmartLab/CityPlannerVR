using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPathHandler : MonoBehaviour {

	//The gameobject to be instantiated
    public GameObject pathPoint;

	public GameObject videoCamera;

	public bool cameraStart = false;

#region private variables

	enum Tool {Add, Move, Remove};
	Tool tool;

	InputMaster inputMaster;
	XRLineRenderer line;

	//The instantiated gameobject
	GameObject point;

	//Put all the points on the path here
	List<GameObject> pathPoints;

	int pathPointIndex = 0;

	GameObject selectedPoint;
	bool holdTrigger = false;


	float cameraSpeed = 0.5f;

#endregion

	void Start(){

        inputMaster = GameObject.Find("Player").GetComponent<InputMaster> ();
		line = GameObject.Find ("CameraPathLineDrawer").GetComponent<XRLineRenderer>();

		pathPoints = new List<GameObject>();
		InitializePathLine ();
		tool = Tool.Add;

		videoCamera.SetActive (false);

		//Subscribe ();
	}

	void Subscribe(){

        inputMaster.TriggerClicked += TriggerPressed;
        inputMaster.TriggerUnclicked += TriggerReleased;

        inputMaster.TriggerClicked += InstantiatePathPoint;
        inputMaster.TriggerClicked += ActivatePointMoving;
        inputMaster.TriggerClicked += RemovePoint;

        inputMaster.TriggerClicked += jokuKamera;
	}

	//--------------------------------------------------------------------------------------------------------------------------------
	//													INSTANTIATE POINT
	//--------------------------------------------------------------------------------------------------------------------------------

	private void InstantiatePathPoint(object sender, ClickedEventArgs e)
    {
		if (tool == Tool.Add) {
			point = Instantiate (pathPoint, transform.position, transform.rotation) as GameObject;
			pathPoints.Add (point);

			DrawLineBetweenPoints ();
		}
    }

	private void DrawLineBetweenPoints(){

		line.SetVertexCount (pathPointIndex + 1);
		line.SetPosition (pathPointIndex, pathPoints[pathPointIndex].transform.position);
		pathPointIndex++;
		
	}

	private void InitializePathLine(){
		line.SetVertexCount (0);
		pathPointIndex = 0;
	}

	//--------------------------------------------------------------------------------------------------------------------------------
	//													MOVE POINT
	//--------------------------------------------------------------------------------------------------------------------------------
	private void ActivatePointMoving(object sender, ClickedEventArgs e){
		if (tool == Tool.Move) {
			StartCoroutine (MovePoint ());
		}
	}

	private IEnumerator MovePoint(){

		if (selectedPoint != null) {

			while (holdTrigger) {
				selectedPoint.transform.position = transform.position;
				selectedPoint.transform.rotation = transform.rotation;

				int index = pathPoints.IndexOf (selectedPoint);
				line.SetPosition (index, selectedPoint.transform.position);

				yield return null;
			}
		}

		else {
			yield break;
		}
	}

	private void TriggerPressed(object sender, ClickedEventArgs e){
		holdTrigger = true;
	}

	private void TriggerReleased(object sender, ClickedEventArgs e){
		holdTrigger = false;
	}

	//--------------------------------------------------------------------------------------------------------------------------------
	//													REMOVE POINT
	//--------------------------------------------------------------------------------------------------------------------------------

	private void RemovePoint(object sender, ClickedEventArgs e){
		if (tool == Tool.Remove) {
			if (selectedPoint != null) {

				pathPoints.Remove (selectedPoint);

				GameObject.Destroy (selectedPoint);

				ReDrawPath ();

				selectedPoint = null;
				tool = Tool.Add;
			}
		}
	}

	private void ReDrawPath(){

		InitializePathLine ();
		line.SetVertexCount (pathPoints.Count);
		pathPointIndex = pathPoints.Count;

		for (int i = 0; i < pathPoints.Count; i++) {
			line.SetPosition(i, pathPoints[i].transform.position);
		}
	}

	//--------------------------------------------------------------------------------------------------------------------------------
	//													CAMERA
	//--------------------------------------------------------------------------------------------------------------------------------

	private void jokuKamera(object sender, ClickedEventArgs e){
        if (cameraStart)
        {
            videoCamera.SetActive(true);
            videoCamera.transform.position = pathPoints[0].transform.position;
            videoCamera.transform.rotation = pathPoints[0].transform.rotation;

            StartCoroutine (MoveCamera ());
        }
	}

	private IEnumerator MoveCamera(){
		//Camera starts at 0 and its first target is 1
		int targetIndex = 1;

        //while the position of the videoCamera is not the same as the last points position we want to move the camera
        while (videoCamera.transform.position != pathPoints[pathPoints.Count - 1].transform.position)
        {
            Transform targetPoint = pathPoints[targetIndex].transform;

            videoCamera.transform.position = Vector3.MoveTowards(videoCamera.transform.position, targetPoint.position, Time.deltaTime * cameraSpeed);
			videoCamera.transform.rotation = Quaternion.RotateTowards (videoCamera.transform.rotation, targetPoint.rotation, Time.deltaTime * 100f);

            if (videoCamera.transform.position == targetPoint.position)
            {
                targetIndex++;
            }

            yield return null;
        }

		yield break;
	}
		
	//--------------------------------------------------------------------------------------------------------------------------------

	void OnCollisionEnter(Collision other){
		if (other.gameObject.tag == "CameraPathPoint") {
			selectedPoint = other.gameObject;

			tool = Tool.Move;
		}
	}

	void OnCollisionExit(Collision other){
		if (other.gameObject.tag == "CameraPathPoint") {
			selectedPoint = null;

			tool = Tool.Add;
		}
	}
}
