using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAreaCollider : MonoBehaviour {

    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;

    float faceSize;
    public float FaceSize
    {
        get
        {
            return faceSize;
        }
        set
        {
            faceSize = value;
        }
    }

    void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }


    public void MakeProceduralGrid()
    {
        vertices = new Vector3[AreaSelection.areaPoints.Count * 2];
		triangles = new int[AreaSelection.areaPoints.Count]; //+ ylös ja alas
        uvs = new Vector2[vertices.Length];

        int v = 0;
        int t = 0;

        float vertexOffset = faceSize * 0.5f;

		for (int i = 1; i < AreaSelection.areaPoints.Count; i++) {
			vertices [v] = AreaSelection.areaPoints [i].transform.position;
			vertices [v + 1] = new Vector3 (vertices[v].x, vertices[v].y + 2, vertices[v].z);

		}
            
        Vector3 cellOffset = new Vector3(faceSize, 0, faceSize);

        vertices[v] = new Vector3(-vertexOffset, transform.position.y, -vertexOffset) + cellOffset;
        vertices[v + 1] = new Vector3(-vertexOffset, transform.position.y, vertexOffset) + cellOffset;
        vertices[v + 2] = new Vector3(vertexOffset, transform.position.y, -vertexOffset) + cellOffset;
        vertices[v + 3] = new Vector3(vertexOffset, transform.position.y, vertexOffset) + cellOffset;

        triangles[t] = v;
        triangles[t + 1] = v + 1;
        triangles[t + 2] = v + 2;
        triangles[t + 3] = v + 2;
        triangles[t + 4] = v + 1;
        triangles[t + 5] = v + 3;

        v += 4;
                t += 6;
            
        

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
