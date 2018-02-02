using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasurementPoint : MonoBehaviour {

	CreateGrid cg;

	GridTile tile;

	public GridTile Tile {
		get {
			return tile;
		}
	}

	void Start()
	{
		cg = GameObject.FindGameObjectWithTag ("GridParent").GetComponent<CreateGrid> ();

		SnapPosition();

	}

	private void SnapPosition()
	{
		RaycastHit hit;

		Vector3 Start = transform.position;

		Ray ray = new Ray(Start, new Vector3(0, -1, 0));


		if (Physics.Raycast(ray, out hit, 1f, 1 << LayerMask.NameToLayer("GridLayer")))
		{
			//Get the tile we hit
			tile = cg.GetTileAt (Mathf.FloorToInt(hit.collider.transform.localPosition.x), Mathf.FloorToInt(hit.collider.transform.localPosition.z));

		}
	}


	//When we drop an object it will snap to the grid
	private void OnDetachedFromHand(Valve.VR.InteractionSystem.Hand hand)
	{
		SnapPosition();
	}
}
