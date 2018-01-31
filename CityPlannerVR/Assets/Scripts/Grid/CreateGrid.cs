using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGrid : MonoBehaviour {

	private GridTile[,] tiles;
	public int gridSizeX;
	public int gridSizeZ;

	[SerializeField]
	private int cellSize;

	public Material gridMaterial;

	// Use this for initialization
	void Start () {

		tiles = new GridTile[gridSizeX, gridSizeZ];

		for (int x = 0; x < gridSizeX; x += cellSize) {
			for (int z = 0; z < gridSizeZ; z += cellSize) {

				tiles [x, z] = new GridTile (new GameObject (), cellSize);
				tiles [x, z].tileObject.name = "Grid point (" + x + "," + z + ")";

				//Set the tiles to be children of this object
				tiles [x, z].tileObject.transform.parent = transform;
				tiles [x, z].tileObject.transform.localPosition = new Vector3(x, transform.position.y, z);
				//This must be done because of all the scaling done in the scene
				tiles [x, z].tileObject.transform.localScale = new Vector3(1, 1, 1);
				
				tiles [x, z].line.material = gridMaterial;

				DrawGrid (tiles[x, z]);
			}
		}
	}

	void DrawGrid(GridTile tile){
		tile.line.SetPosition(0, new Vector3(CalculatePos(tile, 1), transform.position.y, CalculatePos(tile, 1)));
		tile.line.SetPosition(1, new Vector3(CalculatePos(tile, 1), transform.position.y, CalculatePos(tile, -1)));
		tile.line.SetPosition(2, new Vector3(CalculatePos(tile, -1), transform.position.y, CalculatePos(tile, -1)));
		tile.line.SetPosition(3, new Vector3(CalculatePos(tile, -1), transform.position.y, CalculatePos(tile, 1)));
	}

	//Defines the size and position of the lines in the grid
	//Multplier is 1 or -1
	float CalculatePos(GridTile tile, int multiplier)
	{
		//We want to use the tiles own size just in case the cellSize of this object is changed when it should not b
		float pos = (float)tile.CellSize * multiplier / 2;
		return pos;
	}

	public GridTile GetTileAt(int x, int z){
	
		if (tiles [x, z] == null) {
			Debug.LogError ("Tile " + x + "," + z + " was null");
		}

		return tiles [x, z];
	}
}
