using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSelection : MonoBehaviour {

	public int handNumber;

	private LaserPointer laser;
	private XRLineRenderer line;
	private InputMaster inputMaster;

	private string tag = "Untagged";

	private static int index;

	GameObject areaPoint;
	static List<GameObject> areaPoints;

	private void Start(){
		laser = gameObject.transform.parent.GetComponentInChildren<LaserPointer> ();
		line = GetComponent<XRLineRenderer> ();
		inputMaster = GetComponentInParent<InputMaster> ();

		index = 0;

		inputMaster.TriggerClicked += TriggerPressed;
	}

	private void TriggerPressed(object sender, ClickedEventArgs e){
		if (laser.target.tag == tag && e.controllerIndex == handNumber) {
			CreatePoint ();
		}
	}

	private void CreatePoint(){
		areaPoint = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		areaPoint.transform.position = laser.hitPoint;
		areaPoints.Add (areaPoint);

		DrawLineBetweenPoints ();
	}

	private void DrawLineBetweenPoints(){

		line.SetVertexCount (index + 1);
		line.SetPosition (index, areaPoint.transform.position);
		index++;
	}

	private void ScalePoints(){
		//Scale points and line when player shrinks down and grows up
	}
}
