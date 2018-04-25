using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPathHandler : MonoBehaviour {

    public GameObject pathPoint;

#region private variables

	enum Tool {Add, Move, Remove};
	Tool tool;

	InputListener inputListener;
	XRLineRenderer line;

	GameObject point;

	//Put all the points on the path here
	List<GameObject> pathPoints;

	int i = 0;

#endregion

	void Start(){
		
		inputListener = GameObject.Find("Player").GetComponent<InputListener> ();
		line = GameObject.Find ("CameraPathLineDrawer").GetComponent<XRLineRenderer>();

		pathPoints = new List<GameObject>();
		InitializePathLine ();
		tool = Tool.Add;

		Subscribe ();
	}

	void Subscribe(){
	
		inputListener.TriggerClicked += InstantiatePathPoint;
	}


	private void InstantiatePathPoint(object sender, ClickedEventArgs e)
    {
		if (tool == Tool.Add) {
			point = Instantiate (pathPoint, transform.position, transform.rotation) as GameObject;
			pathPoints.Add (point);

			DrawLineBetweenPoints ();
		}
    }

	private void DrawLineBetweenPoints(){

		line.SetVertexCount (i + 1);
		line.SetPosition (i, pathPoints[i].transform.position);
		i++;
		
	}

	private void InitializePathLine(){
		line.SetVertexCount (0);
	}

	private void MovePoint(){
		
	}

	private void RemovePoint(){
		
	}
}
