using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralGrid : MonoBehaviour {

    Mesh mesh;

	Vector3[] vertices;
	int[] triangles;
	Vector2[] uvs;

	float cellSize;
	public float CellSize {
		get {
			return cellSize;
		}
		set {
			cellSize = value;
		}
	}

	int gridSize_x;
	public int GridSize_x {
		get {
			return gridSize_x;
		}
		set {
			gridSize_x = value;
		}
	}

	int gridSize_z;
	public int GridSize_z {
		get {
			return gridSize_z;
		}
		set {
			gridSize_z = value;
		}
	}

	void Awake(){
		mesh = GetComponent<MeshFilter> ().mesh;
	}
		

	public void MakeProceduralGrid(){
		vertices = new Vector3[gridSize_x * gridSize_z * 4];
		triangles = new int[gridSize_x * gridSize_z * 6];
		uvs = new Vector2[vertices.Length];

		int v = 0;
		int t = 0;

		float vertexOffset = cellSize * 0.5f;

		for (int x = 0; x < gridSize_x; x++) {
			for (int z = 0; z < gridSize_z; z++) {
				Vector3 cellOffset = new Vector3 (x * cellSize, 0, z * cellSize);

				vertices [v] = new Vector3 (-vertexOffset, transform.position.y, -vertexOffset) + cellOffset;
				vertices [v + 1] = new Vector3 (-vertexOffset, transform.position.y,  vertexOffset) + cellOffset;
				vertices [v + 2] = new Vector3 ( vertexOffset, transform.position.y, -vertexOffset) + cellOffset;
				vertices [v + 3] = new Vector3 ( vertexOffset, transform.position.y,  vertexOffset) + cellOffset;

				triangles [t] = v;
				triangles [t + 1] = v + 1;
				triangles [t + 2] = v + 2;
				triangles [t + 3] = v + 2;
				triangles [t + 4] = v + 1;
				triangles [t + 5] = v + 3;

				v += 4;
				t += 6;
			}
		}

        //UVs will be same for every tile
		for (int i = 0; i < uvs.Length; i += 4) {
			uvs [i]   = new Vector2 (0, 0);
			uvs [i+1] = new Vector2 (1, 0);
			uvs [i+2] = new Vector2 (0, 1);
			uvs [i+3] = new Vector2 (1, 1);
		}

		UpdateMesh ();

	}

	void UpdateMesh(){
		mesh.Clear();

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;

		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
	}

}
