using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Determines all the basic function for the start and end measurement points
/// </summary>
public class MeasurementPoint : MonoBehaviour {

    GameObject grid;
	CreateGrid createGrid;
    Pathfinding pathfinding;
    GridTile tile;
	public GridTile Tile {
		get {
			return tile;
		}
	}

	IsAttachedToHand attached;

    public GameObject otherPoint;
    MeasurementPoint other;

    void Start()
	{
        grid = GameObject.FindGameObjectWithTag("GridParent");

        createGrid = grid.GetComponent<CreateGrid> ();
        pathfinding = grid.GetComponent<Pathfinding>();

		attached = GetComponent<IsAttachedToHand> ();

        SnapPosition();

		if (attached != null) {
			attached.OnSnapToGrid += CheckIfSnapping;
		}
	}

	void CheckIfSnapping(){
		if (attached != null) {
			Debug.Log ("IsHolding a Measuremen point: " + attached.IsHolding);
			if (!attached.IsHolding) {
				transform.parent = grid.transform;
				SnapPosition ();
			}
		}
	}

	private void SnapPosition()
	{
		RaycastHit hit;

		Vector3 Start = transform.position;

		Ray ray = new Ray(Start, Vector3.down);
        

		if (Physics.Raycast(ray, out hit, 5f, 1 << LayerMask.NameToLayer("GridLayer")))
		{
			//Get the tile we hit
			tile = createGrid.GetTileAt (hit.collider.transform.localPosition.x, hit.collider.transform.localPosition.z);
            transform.localPosition = new Vector3(tile.tileObject.transform.localPosition.x, tile.tileObject.transform.localPosition.y * 2f, tile.tileObject.transform.localPosition.z);
            transform.localRotation = Quaternion.identity;

            other = otherPoint.GetComponent<MeasurementPoint>();
            if (other.Tile != null)
            {
                //Gets distance between the tile this object is on and the tile the other object is on
                float dist = MeasureDistance.CalculateDistance(tile, other.Tile);
                pathfinding.FindPath(Tile, other.Tile);
                Debug.Log("Distance without pathfinding is " + dist);
            }
		}
	}
}
