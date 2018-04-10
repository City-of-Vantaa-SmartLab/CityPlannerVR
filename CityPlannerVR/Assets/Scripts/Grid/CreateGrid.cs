using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates the grid tiles and puts them in order
/// 
/// Tutorial: https://www.youtube.com/watch?v=3Dw5d7PlcTM (other videos in the series were also used)
/// </summary>
public class CreateGrid : MonoBehaviour {

	private GridTile[,] tiles;

    ProceduralGrid grid;

    public int originalGridSizeX;
	public int originalGridSizeZ;

    int gridSizeX;
    int gridSizeZ;

	private float cellSize = 2f;

    //This is here for now because tutorial said so. Used if we want to move something in our grid
	public List<GridTile> path;

	public float CellSize {
		get {
			return cellSize;
		}
		set {
			cellSize = value;
            DeleteGridTiles();
			CreateGridTiles ();
		}
	}

    private void Awake()
    {
        grid = GetComponentInChildren<ProceduralGrid>();
    }

    // Use this for initialization
    void Start () {

		CreateGridTiles ();
	}

    //Pathfinding uses this
    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeZ;
        }
    }

    void DeleteGridTiles()
    {
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeZ; j++)
            {
                Destroy(tiles[i, j].tileObject);
            }
        }
    }

	void CreateGridTiles(){

        gridSizeX = originalGridSizeX;
        gridSizeZ = originalGridSizeZ;

        //This will make sure that the grid is the same size as the table is, when cellSize is changed
        gridSizeX = Mathf.FloorToInt(gridSizeX / CellSize);
        gridSizeZ = Mathf.FloorToInt(gridSizeZ / CellSize);

        //Gives the ProceduralGrid important information about how to draw the grid
        grid.CellSize = CellSize;
        grid.GridSize_x = gridSizeX;
        grid.GridSize_z = gridSizeZ;

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
			}
		}

        grid.MakeProceduralGrid();
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

    //Gets a tile according to its position in the world from the array and returns it
	public GridTile GetTileAt(float x, float z){

        int xIndex = Mathf.FloorToInt(x / CellSize);
        int zIndex = Mathf.FloorToInt(z / CellSize);

        if (tiles [xIndex, zIndex] == null) {
			Debug.LogError ("Tile " + x + "," + z + " was null");
		}

		return tiles [xIndex, zIndex];
	}
}
