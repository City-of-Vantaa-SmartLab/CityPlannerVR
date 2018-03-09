using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGridLine : MonoBehaviour {

    XRLineRenderer line;
    CreateGrid createGrid;

    public Material gridMaterial;

	private float cellSize;
	private int gridSizeX;
	private int gridSizeZ;

	private float lineLenghtX;
	private float lineLenghtZ;

	private int maxLineCountX;
	private int maxLineCountZ;

	private int lineCountX;
	private int lineCountZ;

	//nämä voinee yhdistää
	private int coefficientX;
	private int coefficientZ;

	//nämä voinee yhdistää
	private const int x = 0;
	private const int z = 0;

	private float y;

    //How many points we need to draw the lines through
    private int pointCount;

	void Start()
    {
        line = gameObject.AddComponent<XRLineRenderer>();
		createGrid = GameObject.FindGameObjectWithTag ("GridParent").GetComponent<CreateGrid>();
        line.SetTotalWidth(0.1f);
        line.material = gridMaterial;

        y = transform.position.y;

        DrawGrid();
    }

    void DrawGrid()
    {
		bool xFull = false;

		int tempX = 0;
		int tempZ = 0;
		bool goUp = false;

		cellSize = createGrid.CellSize;
		gridSizeX = Mathf.FloorToInt(createGrid.originalGridSizeX / cellSize);
		gridSizeZ = Mathf.FloorToInt(createGrid.originalGridSizeZ / cellSize);

        lineLenghtX = gridSizeX * cellSize;
        lineLenghtZ = gridSizeZ * cellSize;

		maxLineCountX = gridSizeX;
		maxLineCountZ = gridSizeZ;

		lineCountX = 0;
		lineCountZ = 0;

		coefficientX = 0;
		coefficientZ = gridSizeZ;

        pointCount = (gridSizeX - 1) * 2 + (gridSizeZ - 1) * 2 + 7;
		line.SetVertexCount (pointCount);

		//the first point
		line.SetPosition (0, new Vector3 (x, y, z));

		for (int i = 1; i < pointCount; i++) {
			//suuntaan X
			if (!xFull) {

				//Tämä suunta on valmis
				if (lineCountX >= maxLineCountX) {
					line.SetPosition (i, new Vector3(cellSize * coefficientX, y, lineLenghtZ));
					xFull = true;
					if (line.GetPosition (i - 1).x == 0) {
						coefficientZ = 0;
						goUp = true;
					} else {
						goUp = false;
					}
				}

				else if (tempX == 0 || tempX == 1) {
					//ylös
					line.SetPosition (i, new Vector3(cellSize * coefficientX, y, lineLenghtZ));
					tempX++;
				}

				else{
					//alas
					line.SetPosition (i, new Vector3(cellSize * coefficientX, y, z));
					if (tempX == 2) {
						tempX++;
					} 

					else {
						tempX = 0;
					}
				}
			}

			//Suuntaan Z
			else{

				if (tempZ == 0 || tempZ == 1) {
					//ylös
					line.SetPosition (i, new Vector3(x, y, cellSize * coefficientZ));
					tempZ++;
				}

				else{
					//alas
					line.SetPosition (i, new Vector3(lineLenghtX, y, cellSize * coefficientZ));
					if (tempZ == 2) {
						tempZ++;
					} 

					else {
						tempZ = 0;
					}
				}
			}

			//X
			if (tempX % 2 != 0) {
				coefficientX++;
			} else {
				lineCountX++;
			}

			//Z
			if (xFull) {
				if (tempZ % 2 != 0) {

					if (goUp) {
						coefficientZ++;
					} else {
						coefficientZ--;
					}
				} else {
					lineCountZ++;
				}
			}
		}
    }
}
