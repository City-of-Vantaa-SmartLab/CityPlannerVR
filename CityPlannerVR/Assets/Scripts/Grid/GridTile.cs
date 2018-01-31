using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile {

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

	private int cellSize;

	public int CellSize {
		get {
			return cellSize;
		}
	}

    public GameObject tileObject;
    public BoxCollider collider;
    public LineRenderer line;

	private GridTileStateCheck checkGridState;

	public GridTile(GameObject tileObject, int cellSize)
    {
        this.tileObject = tileObject;
		this.cellSize = cellSize;

		collider = tileObject.AddComponent<BoxCollider> ();
		line = tileObject.AddComponent<LineRenderer> ();
		checkGridState = tileObject.AddComponent<GridTileStateCheck> ();
		checkGridState.tile = this;

		InitializeTileObject ();
		InitializeLineRenderer ();
    }

	//All the grids will have these values and they are not changed
	private void InitializeTileObject(){
		tileObject.tag = "Grid";
		tileObject.layer = LayerMask.NameToLayer("GridLayer");

		collider.size = new Vector3(cellSize, 0, cellSize);
		//This is hardcoded value. It makes sure the collider is in right place 
		collider.center = new Vector3 (0, 0.7f, 0);
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
}
