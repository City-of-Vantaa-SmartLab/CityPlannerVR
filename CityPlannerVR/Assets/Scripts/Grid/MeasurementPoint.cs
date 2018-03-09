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
        private set
        {
            tile = value;
        }
		get {
			return tile;
		}
	}

	IsAttachedToHand attached;

	//[Tooltip("The other one of the two measurementpoints (EndPoint for StartPoint and other way around)")]
    [HideInInspector]
    public GameObject otherPoint;
    MeasurementPoint other;

    void Start()
	{
		grid = ObjectContainer.grid;

        createGrid = grid.GetComponent<CreateGrid> ();
        pathfinding = grid.GetComponent<Pathfinding>();

		attached = GetComponent<IsAttachedToHand> ();

		if (gameObject.name == "MeasurementStartPoint") {
			otherPoint = GameObject.FindGameObjectWithTag("EndPoint");
		} else if (gameObject.name == "MeasurementEndPoint") {
			otherPoint = GameObject.FindGameObjectWithTag("StartPoint");
		} else {
			Debug.LogError ("There is a typo somewhere");
		}

		SnapPosition();

		if (attached != null) {
			attached.OnSnapToGrid += CheckIfSnapping;
		}
	}

	void CheckIfSnapping(){
		if (attached != null) {
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
			Tile = createGrid.GetTileAt (hit.collider.transform.localPosition.x, hit.collider.transform.localPosition.z);
            transform.localPosition = new Vector3(Tile.tileObject.transform.localPosition.x, Tile.tileObject.transform.localPosition.y * 2f, Tile.tileObject.transform.localPosition.z);
            transform.localRotation = Quaternion.identity;

            other = otherPoint.GetComponent<MeasurementPoint>();
            if (other.Tile != null)
            {
                //Gets distance between the tile this object is on and the tile the other object is on
                float dist = MeasureDistance.CalculateDistance(tile, other.Tile);
                pathfinding.FindPath(Tile.tileObject.transform.localPosition, other.Tile.tileObject.transform.localPosition);
                Debug.Log("Distance without pathfinding is " + dist);
            }
		}
	}
}
