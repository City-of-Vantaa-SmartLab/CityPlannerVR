using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSelection : MonoBehaviour {

	private LaserPointer laser;
	private XRLineRenderer line;
	private InputMaster inputMaster;

    private string areaTag = "Untagged";
	private static int index;

	GameObject areaPoint;
	public static List<GameObject> areaPoints;

	private void Start(){
		laser = gameObject.transform.parent.GetComponentInChildren<LaserPointer> ();
		line = GetComponent<XRLineRenderer> ();
		inputMaster = GetComponentInParent<InputMaster> ();

        areaPoints = new List<GameObject>();

		index = 0;
	}

    public void ActivateCreatePoint(LaserPointer laser, GameObject target)
    {
        if(target.tag == areaTag)
        {
            CreatePoint();
        }
    }


    private void CreatePoint(){
		areaPoint = GameObject.CreatePrimitive (PrimitiveType.Sphere);
        areaPoint.name = "SelectionPoint";
		areaPoint.transform.position = laser.hitPoint;
        areaPoint.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
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
