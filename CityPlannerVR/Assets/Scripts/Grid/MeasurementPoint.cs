using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Determines all the basic function for the start and end measurement points
/// </summary>
public class MeasurementPoint : MonoBehaviour {

    GameObject grid;
	CreateGrid cg;
    Pathfinding path;
    GridTile tile;
	public GridTile Tile {
		get {
			return tile;
		}
	}

    public GameObject otherPoint;
    MeasurementPoint other;

    void Start()
	{
        grid = GameObject.FindGameObjectWithTag("GridParent");

        cg = grid.GetComponent<CreateGrid> ();
        path = grid.GetComponent<Pathfinding>();

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
			tile = cg.GetTileAt (hit.collider.transform.localPosition.x, hit.collider.transform.localPosition.z);
            transform.localPosition = new Vector3(tile.tileObject.transform.localPosition.x, tile.tileObject.transform.localPosition.y * 2f, tile.tileObject.transform.localPosition.z);
            transform.localRotation = Quaternion.identity;

            other = otherPoint.GetComponent<MeasurementPoint>();
            if (other.Tile != null)
            {
                //Gets distance between the tile this object is on and the tile the other object is on
                float dist = MeasureDistance.CalculateDistance(tile, other.Tile);
                path.FindPath(Tile, other.Tile);
                Debug.Log("Distance without pathfinding is " + dist);
            }
		}
	}


	//When we drop an object it will snap to the grid
	private void OnDetachedFromHand(Valve.VR.InteractionSystem.Hand hand)
	{
        transform.parent = grid.transform;
		SnapPosition();
	}
}
