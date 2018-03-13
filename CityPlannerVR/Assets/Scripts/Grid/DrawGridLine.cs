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

		int temp = 0;
		bool goDown = false;

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
				if (lineCountX > maxLineCountX) {
					
					xFull = true;
					if (line.GetPosition (i - 1).z == 0) {
						coefficientZ = 0;
						line.SetPosition (i, new Vector3 (x, y, cellSize * coefficientZ));
						goDown = true;
					} else {
						line.SetPosition (i, new Vector3 (x, y, cellSize * coefficientZ));
						goDown = false;
					}
				}

				else if (temp == 0 || temp == 1) {
					//ylös
					line.SetPosition (i, new Vector3(cellSize * coefficientX, y, lineLenghtZ));
				}

				else{
					//alas
					line.SetPosition (i, new Vector3(cellSize * coefficientX, y, z)); 
				}
			}

			//Suuntaan Z
			else{

				if ((goDown && (temp == 0 || temp == 3)) || (!goDown && (temp == 1 || temp == 2))) {
					//ylös
					line.SetPosition (i, new Vector3(x, y, cellSize * coefficientZ));
				}

				else{
					//alas
					line.SetPosition (i, new Vector3(lineLenghtX, y, cellSize * coefficientZ));
				}
			}

			//X
			if (!xFull) {
				if (temp % 2 == 0) {
					coefficientX++;
					lineCountX++;
				}
				if (temp == 3) {
					temp = 0;
				} else {
					temp++;
				}
			}

			//Z
			else {
				if (temp % 2 != 0) {

					if (goDown) {
						coefficientZ++;
						lineCountZ++;
					} else {
						coefficientZ--;
						lineCountZ++;
					}
				}
				if (temp == 3) {
					temp = 0;
				} else {
					temp++;
				}
			}
		}
    }
}
