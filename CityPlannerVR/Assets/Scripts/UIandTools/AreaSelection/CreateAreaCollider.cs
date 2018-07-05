using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates the visuals of the area and the mesh collider
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CreateAreaCollider : MonoBehaviour {

    Mesh mesh;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    Vector3[] vertices;
    int[] triangles;

    RestrictObjectInteraction restrictObjectInteraction;

    void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        restrictObjectInteraction = GetComponent<RestrictObjectInteraction>();

    }

    /// <summary>
    /// Calls the CreateMesh rpc on every client
    /// </summary>
    /// <param name="app">The positions of the AreaPoints (which are the corners of the area)</param>
    /// <param name="owner">Owner of the collider</param>
    public void CallRPC(Vector3[] app, string owner)
    {
       GetComponent<PhotonView>().RPC("CreateMesh", PhotonTargets.All, new object[] {app, owner});
    }

    /// <summary>
    /// Calls the necessary methods to create the area mesh on all clients
    /// </summary>
    /// <param name="app">The positions of the AreaPoints (which are the corners of the area)</param>
    /// <param name="owner">The name of the owner of the collider</param>
    [PunRPC]
    void CreateMesh(Vector3[] app, string owner)
    {
        MakeProceduralMesh(app);
        restrictObjectInteraction.SetOwnerName(owner);
    }

    /// <summary>
    /// Creates the actual mesh which is given to the meshcollider
    /// </summary>
    /// <param name="areaPoints">The corners of the area</param>
    public void MakeProceduralMesh(Vector3[] areaPoints)
    {
        vertices = new Vector3[areaPoints.Length * 2];
        //if there is only 2 points, we don't need the roof and the floor of the mesh 
        if(areaPoints.Length == 1)
        {
            triangles = new int[areaPoints.Length * 6];
        }
        //if there is more than 2 points, also include the space for the roof and floor triangles
        else
        {
            triangles = new int[areaPoints.Length * 6 + ((areaPoints.Length - 2) * 3) * 2 + 6];
        }

        //These are help indexes that are used to create correct amount of vertices and triangles to correct positions
        int v = 0;
        int m = 0;
        int t = 0;

		//Add all the requered vertices to array
		for (int i = 0; i < areaPoints.Length; i++) {
                
            vertices[v] = areaPoints[i];
            //The area which player defines is 2D so make another point on top of it to make a 3D mesh
            vertices[v + 1] = new Vector3(vertices[v].x, vertices[v].y + 2, vertices[v].z);

            v += 2;
	    }
        
        //Add triangles to array
        for (int i = 0; i < areaPoints.Length - 1; i++)
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
        if(areaPoints.Length > 2)
        {
            //If we have odd or even number of triangles on top and on bottom of the mesh
			bool oddTriangles = true;
            //This is just a help index to that is used to put the roof and floor triangles to correct places
			int n = 0;

            for (int i = 0; i < areaPoints.Length - 2; i++)
            {
                //Every other point in the list belongs up and others belong down

                if (oddTriangles)
                {
                    //Floor
                    triangles[t]     = 0;
                    triangles[t + 1] = n + 2;
                    triangles[t + 2] = n + 4;
                    //Roof
                    triangles[t + 3] = n + 5;
                    triangles[t + 4] = n + 3;
                    triangles[t + 5] = 1;

                    oddTriangles = false;
                }
				
				else{
                    //Floor
					triangles [t]     = n + 4;
                    triangles[t + 1] = n + 6;
                    triangles [t + 2] = 0;
                    //Roof
                    triangles[t + 3] = 1;
                    triangles[t + 4] = n + 7;
                    triangles[t + 5] = n + 5;

                    oddTriangles = true;

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
        //meshCollider.sharedMesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;

        mesh.RecalculateNormals();
        meshCollider.sharedMesh.RecalculateBounds();
        mesh.RecalculateBounds();
    }
}
