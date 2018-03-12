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
    public BoxCollider collider;
    //public XRLineRenderer line;

	public float xPos;
	public float zPos;

	public float gCost;
	public float hCost;
	public GridTile parent;

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
		//line = tileObject.AddComponent<XRLineRenderer> ();
		checkGridState = tileObject.AddComponent<GridTileStateCheck> ();
		checkGridState.tile = this;

        InitializeTileObject (cellSize);
		InitializeLineRenderer ();
    }

	//All the grids will have these values and they are not changed
	private void InitializeTileObject(float cellSize){
		tileObject.tag = "Grid";
		tileObject.layer = LayerMask.NameToLayer("GridLayer");

		collider.size = new Vector3(cellSize, 0, cellSize);
		//This is hardcoded value. It makes sure the collider is in right place 
		collider.center = new Vector3 (0, 0, 0);
    }

	private void InitializeLineRenderer(){
		//Defines how many points we have to draw the line through
		//line.SetVertexCount(4);
		//line.SetTotalWidth(0.1f);
		//Draws a line from last point to the first (so we get a square)
		//line.loop = true;
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
