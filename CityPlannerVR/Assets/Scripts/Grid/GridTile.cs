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
			triggerScript.GetGridState (state);
		}
	}

    public GameObject tileObject;
    public BoxCollider collider;
    public BoxCollider trigger;
    public LineRenderer line;

	public float xPos;
	public float zPos;

	public float gCost;
	public float hCost;
	public GridTile parent;

	TriggerScript triggerScript;

    int heapIndex;


	public float fCost{
		get{ 
			return gCost + hCost;
		}
	}

	private GridTileStateCheck checkGridState;

	public GridTile(GameObject tileObject, float cellSize, float x, float z)
    {
        this.tileObject = tileObject;
		xPos = x;
		zPos = z;

		collider = tileObject.AddComponent<BoxCollider> ();
        trigger = tileObject.AddComponent<BoxCollider>();
		line = tileObject.AddComponent<LineRenderer> ();
		checkGridState = tileObject.AddComponent<GridTileStateCheck> ();
		checkGridState.tile = this;

		triggerScript = tileObject.AddComponent<TriggerScript>();

        InitializeTileObject (cellSize);
		InitializeLineRenderer ();
    }

	//All the grids will have these values and they are not changed
	private void InitializeTileObject(float cellSize){
		tileObject.tag = "Grid";
		tileObject.layer = LayerMask.NameToLayer("GridLayer");

		collider.size = new Vector3(cellSize, 0, cellSize);
		//This is hardcoded value. It makes sure the collider is in right place 
		collider.center = new Vector3 (0, 0.7f, 0);

        trigger.size = new Vector3(cellSize, 1, cellSize);
        trigger.center = new Vector3(0, 1, 0);
        trigger.isTrigger = true;
	}

	private void InitializeLineRenderer(){
		//Defines how many points we have to draw the line through
		line.positionCount = 4;
		//Don't know if I have to use them both, but I'm using them just in case
		line.startWidth = 0.01f;
		line.endWidth = 0.01f;
		//Used so the grid scales correctly on the table
		line.useWorldSpace = false;
		//We don't need (or want) shadows
		line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		//Draws a line from last point to the first (so we get a square)
		line.loop = true;
	}

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
