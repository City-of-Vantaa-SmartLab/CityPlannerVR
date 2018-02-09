using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGrid : MonoBehaviour {

	private GridTile[,] tiles;

	public int gridSizeX;
	public int gridSizeZ;

	private float cellSize = 5f;

	public List<GridTile> path;

	public float CellSize {
		get {
			return cellSize;
		}
		set {
			cellSize = value;
			//CreateGridTiles ();
		}
	}

	public Material gridMaterial;

	// Use this for initialization
	void Start () {

		CreateGridTiles ();
	}

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeZ;
        }
    }

	void CreateGridTiles(){

        //This will make sure that the grid is the same size as the table is when cellSize is changed
        gridSizeX = Mathf.FloorToInt(gridSizeX / CellSize);
        gridSizeZ = Mathf.FloorToInt(gridSizeZ / CellSize);

        tiles = new GridTile[gridSizeX, gridSizeZ];

		for (int x = 0; x < gridSizeX; x++) {
			for (int z = 0; z < gridSizeZ; z++) {

				tiles [x, z] = new GridTile (new GameObject (), cellSize, x * CellSize, z * CellSize);
				tiles [x, z].tileObject.name = "Grid point (" + x * CellSize + "," + z * CellSize + ")";

				//Set the tiles to be children of this object
				tiles [x, z].tileObject.transform.parent = transform;
				tiles [x, z].tileObject.transform.localPosition = new Vector3(x * CellSize, transform.position.y, z * CellSize);
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


	public List<GridTile> GetNeighbours(GridTile tile){
		List<GridTile> neighbours = new List<GridTile> ();

		for (int x = -1; x <= 1; x++) {
			for (int z = -1; z <= 1; z++) {
				if (x == 0 && z == 0) {
					continue;
				}

				int checkX = Mathf.FloorToInt(tile.xPos/CellSize + x);
				int checkZ = Mathf.FloorToInt(tile.zPos/CellSize + z);

				if (checkX >= 0 && checkX < gridSizeX && checkZ >= 0 && checkZ < gridSizeZ) {
					neighbours.Add (tiles[checkX, checkZ]);
				}
			}
		}
		return neighbours;
	}

	//Defines the size and position of the lines in the grid
	//Multplier is 1 or -1
	float CalculatePos(GridTile tile, int multiplier)
	{
		//We want to use the tiles own size just in case the cellSize of this object is changed when it should not be
		float pos = CellSize * multiplier / 2;
		return pos;
	}

	public GridTile GetTileAt(float x, float z){

        int xIndex = Mathf.FloorToInt(x / CellSize);
        int zIndex = Mathf.FloorToInt(z / CellSize);

        Debug.Log("xIndex = " + xIndex);
        Debug.Log("zIndex = " + zIndex);

        if (tiles [xIndex, zIndex] == null) {
			Debug.LogError ("Tile " + x + "," + z + " was null");
		}

		return tiles [xIndex, zIndex];
	}

    //Just for pathfinding debugging
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSizeX, 1, gridSizeZ));

        if (tiles != null)
        {
            foreach (GridTile n in tiles)
            {
                Gizmos.color = (n.State == GridTile.GridState.Empty) ? Color.white : Color.red;
                if (path != null)
                    if (path.Contains(n))
                        Gizmos.color = Color.black;
                Gizmos.DrawCube(new Vector3(n.xPos, 1, n.zPos)*0.025f, Vector3.one * CellSize * 0.025f);
            }
        }
    }
}
