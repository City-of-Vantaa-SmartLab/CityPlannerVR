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
    public BoxCollider indicatorTrigger;
    public XRLineRenderer line;

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
        indicatorTrigger = tileObject.AddComponent<BoxCollider>();
		line = tileObject.AddComponent<XRLineRenderer> ();
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

        //This will trigger when the building is put on the table
        trigger.size = new Vector3(cellSize, 1, cellSize);
        trigger.center = new Vector3(0, 1, 0);
        trigger.isTrigger = true;

        //This will trigger and change the color of the building, to indicate, if the building will be put on another building
        indicatorTrigger.size = new Vector3(cellSize, 7, cellSize);
        indicatorTrigger.center = new Vector3(0, 4, 0);
        indicatorTrigger.isTrigger = true;
    }

	private void InitializeLineRenderer(){
		//Defines how many points we have to draw the line through
		line.SetVertexCount(4);
		line.SetTotalWidth(0.1f);
		//Used so the grid scales correctly on the table
		//line.m_UseWorldSpace = false;
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
