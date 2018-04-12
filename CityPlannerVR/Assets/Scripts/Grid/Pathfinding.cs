using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pathfinding for the Measurement points to find the path
/// 
/// Tutorial: https://www.youtube.com/watch?v=mZfyt03LDH4
/// </summary>


public class Pathfinding : MonoBehaviour {
    
    private CreateGrid createGrid;
    private XRLineRenderer pathRenderer;

    // Use this for initialization 
    void Start () {
        createGrid = GetComponent<CreateGrid> ();
        pathRenderer = GetComponent<XRLineRenderer>();
    }

    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        GridTile startNode = createGrid.GetTileAt(startPos.x, startPos.z);
        GridTile targetNode = createGrid.GetTileAt(targetPos.x, targetPos.z);

        Heap<GridTile> openSet = new Heap<GridTile>(createGrid.MaxSize);
        HashSet<GridTile> closedSet = new HashSet<GridTile>();

        openSet.Add(startNode);

        //Initializes the path drawing every time, so if the path cannot be established, there will be no path visible
        pathRenderer.SetVertexCount(0);

        while (openSet.Count > 0)
        {
            GridTile node = openSet.RemoveFirst();
           
            closedSet.Add(node);

            if (node == targetNode)
            {
                RetracePath(startNode.tileObject.transform.localPosition, targetNode.tileObject.transform.localPosition);
                return;
            }

            foreach (GridTile neighbour in createGrid.GetNeighbours(node))
            {
                if (neighbour.State != GridTile.GridState.Empty || closedSet.Contains(neighbour))
                {
                    continue;
                }

                float newCostToNeighbour = node.gCost + MeasureDistance.CalculateDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = MeasureDistance.CalculateDistance(neighbour, targetNode);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                    else
                    {
                        openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
    }

    void RetracePath(Vector3 startPos, Vector3 targetPos)
    {
        GridTile start = createGrid.GetTileAt(startPos.x, startPos.z);
        GridTile end = createGrid.GetTileAt(targetPos.x, targetPos.z);

        List<GridTile> path = new List<GridTile>();
        GridTile currentNode = end;

        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Add(start);

        //This might actually be never needed, but I'm leaving it for now just in case 
        //(if we want something to move along the path etc.)
        path.Reverse();
        createGrid.path = path;

        DrawAndMeasurePath(path);

    }

    void DrawAndMeasurePath(List<GridTile> path)
    {
        int diagonalCount = 0;
        int verticalOrHorizontalCount = 0;

        //This might not be right, but it has to be tested out to know for sure
        //(Everything looks right so far)
        pathRenderer.SetVertexCount(path.Count);

        for (int i = 0; i < pathRenderer.GetVertexCount(); i++)
        {
            //This will make a line to represent the path found with the pathfinding
            //                                                                           we have to lift the line up a bit, so we can see it
            Vector3 linePath = new Vector3(path[i].tileObject.transform.localPosition.x, path[i].tileObject.transform.localPosition.y * 2, path[i].tileObject.transform.localPosition.z);
            pathRenderer.SetPosition(i, linePath);

            //This is used to measure the distance of the path that is found with the pathfinding
            if (i > 0)
            {

                if (path[i].xPos != path[i - 1].xPos && path[i].zPos != path[i - 1].zPos)
                {
                    diagonalCount++;
                }
                else
                {
                    verticalOrHorizontalCount++;
                }
            }
        }

        float distance = diagonalCount * 1.4f * createGrid.CellSize + verticalOrHorizontalCount * createGrid.CellSize;
        Debug.Log("Distance with pathfinding is " + distance);
    }
}
