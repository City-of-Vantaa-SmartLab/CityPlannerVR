using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : IHeapItem<GridTile> {

    public enum GridState { Empty, Full };

	GridState state = GridState.Empty;

	public GridState State {
		get {
			return state;
		}
		set {
			state = value;
		}
	}

    //The gameObject that represents this tile and has all the components
    public GameObject tileObject;
    public GameObject containedObject;
    public BoxCollider collider;
    //Is used to indicate to the player if object can be placed here
    public BoxCollider trigger;
    IndicateGridState indicator;

    public float xPos;
	public float zPos;

    //Pathfinding stuff
	public float gCost;
	public float hCost;
    public float fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
    int heapIndex;

    public GridTile parent;
    HighlightSelection highLight;

	private GridTileStateCheck checkGridState;

	public GridTile(GameObject tileObject, float cellSize, float x, float z)
    {
        this.tileObject = tileObject;
		xPos = x;
		zPos = z;

        //All the components that are added to this object
		collider = tileObject.AddComponent<BoxCollider> ();
        trigger = tileObject.AddComponent<BoxCollider> ();
		checkGridState = tileObject.AddComponent<GridTileStateCheck> ();
		checkGridState.tile = this;
        highLight = tileObject.AddComponent<HighlightSelection>();
        indicator = tileObject.AddComponent<IndicateGridState>();

        InitializeTileObject (cellSize);
    }

	//All the grids will have these values and they are not changed
	private void InitializeTileObject(float cellSize){
		tileObject.tag = "Grid";
		tileObject.layer = LayerMask.NameToLayer("GridLayer");

		collider.size = new Vector3(cellSize, 0, cellSize);
		//This is hardcoded value. It makes sure the collider is in right place 
		collider.center = new Vector3 (0, 0, 0);

        //trigger height should be a bit taller than the tallest object on the grid (to avoid confusion with users)
        float triggerHeight = 20f;

        trigger.isTrigger = true;
        trigger.size = new Vector3(cellSize, triggerHeight, cellSize);
        trigger.center = new Vector3(0, triggerHeight * 0.5f, 0);
    }

    //For the pathfinding
    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(GridTile nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
