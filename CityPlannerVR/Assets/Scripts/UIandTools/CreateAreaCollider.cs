using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CreateAreaCollider : MonoBehaviour {

    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;

    float faceSize;

    void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }


    public void MakeProceduralMesh()
    {
        vertices = new Vector3[AreaSelection.areaPoints.Count * 2];
		triangles = new int[(AreaSelection.areaPoints.Count - 1) * 6]; //+ ylös ja alas
        uvs = new Vector2[vertices.Length];

        int v = 0;
        int t = 0;

		if (AreaSelection.areaPoints.Count > 1) {
			for (int i = 1; i < AreaSelection.areaPoints.Count; i++) {
				vertices[v] = AreaSelection.areaPoints [i].transform.position;
				vertices[v + 1] = new Vector3 (vertices[v].x, vertices[v].y + 2, vertices[v].z);
				vertices[v + 2] = AreaSelection.areaPoints [i - 1].transform.position;
				vertices[v + 3] = new Vector3 (vertices[v + 2].x, vertices[v + 2].y + 2, vertices[v + 2].z);

				triangles[t] = v;
				triangles[t + 1] = v + 1;
				triangles[t + 2] = v + 2;
				triangles[t + 3] = v + 2;
				triangles[t + 4] = v + 1;
				triangles[t + 5] = v + 3;

				v += 4;
				t += 6;
			}

            //UVs will be same for every tile
            for (int i = 0; i < uvs.Length; i += 4)
            {
                uvs[i] = new Vector2(0, 0);
                uvs[i + 1] = new Vector2(1, 0);
                uvs[i + 2] = new Vector2(0, 1);
                uvs[i + 3] = new Vector2(1, 1);
            }

            UpdateMesh();
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
