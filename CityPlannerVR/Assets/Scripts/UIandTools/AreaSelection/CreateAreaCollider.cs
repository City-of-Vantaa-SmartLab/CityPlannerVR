using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CreateAreaCollider : MonoBehaviour {

    Mesh mesh;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    Vector3[] vertices;
    int[] triangles;

    float faceSize;

    void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

    }


    public void MakeProceduralMesh(List<Vector3> areaPoints)
    {
        vertices = new Vector3[areaPoints.Count * 2];
        if(areaPoints.Count == 1)
        {
            triangles = new int[areaPoints.Count * 6];
        }
        else
        {
            triangles = new int[areaPoints.Count * 6 + ((areaPoints.Count - 2) * 3) * 2 + 6]; //+ ylös ja alas
        }

        int v = 0;
        int m = 0;
        int t = 0;

		
		for (int i = 0; i < areaPoints.Count; i++) {
                
            vertices[v] = areaPoints[i];
            vertices[v + 1] = new Vector3(vertices[v].x, vertices[v].y + 2, vertices[v].z);

            v += 2;

	    }

        for (int i = 0; i < AreaSelection.areaPoints.Count - 1; i++)
        {
            if (triangles.Length > 0)
            {
                triangles[t]     = m;
                triangles[t + 1] = m + 1;
                triangles[t + 2] = m + 2;
                triangles[t + 3] = m + 2;
                triangles[t + 4] = m + 1;
                triangles[t + 5] = m + 3;

                t += 6;
                m += 2;
            }
        }
        //The last quad so the shape makes a loop
        if (triangles.Length > 0)
        {
            triangles[t] = m;
            triangles[t + 1] = m + 1;
            triangles[t + 2] = 0;
            triangles[t + 3] = 0;
            triangles[t + 4] = m + 1;
            triangles[t + 5] = 1;

            t += 6;
        }

        //top and bottom of the mesh
        if(AreaSelection.areaPoints.Count > 2)
        {
            //TODO: Rename jotain
			bool jotain = true;
			int n = 0;

            for (int i = 0; i < areaPoints.Count - 2; i++)
            {
                //Every other point in the list belongs up and others belong down

                if (jotain)
                {
                    //down
                    triangles[t]     = 0;
                    triangles[t + 1] = n + 2;
                    triangles[t + 2] = n + 4;
                    ////up
                    triangles[t + 3] = n + 5;
                    triangles[t + 4] = n + 3;
                    triangles[t + 5] = 1;

                    jotain = false;
                }
				
				else{
                    //down
					triangles [t]     = n + 4;
                    triangles[t + 1] = n + 6;
                    triangles [t + 2] = 0;
                    //up
                    triangles[t + 3] = 1;
                    triangles[t + 4] = n + 7;
                    triangles[t + 5] = n + 5;

                    jotain = true;

                    n += 4;
                }
                t += 6;
            }
        }

        UpdateMesh();
        
    }

    void UpdateMesh()
    {
        mesh.Clear();
        meshCollider.sharedMesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;

        mesh.RecalculateNormals();
        meshCollider.sharedMesh.RecalculateBounds();
        mesh.RecalculateBounds();
    }
}
