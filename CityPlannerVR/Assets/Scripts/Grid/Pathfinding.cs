using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO: Change and optimize sometime
//  -Add possible visual effect to show the path

public class Pathfinding : MonoBehaviour {

	CreateGrid createGrid;
    private LineRenderer pathRenderer;

    // Use this for initialization 
    void Start () {
        createGrid = GetComponent<CreateGrid> ();
        pathRenderer = GetComponent<LineRenderer>();
    }

    public void FindPath(GridTile startNode, GridTile targetNode)
    {
        Heap<GridTile> openSet = new Heap<GridTile>(createGrid.MaxSize);
        HashSet<GridTile> closedSet = new HashSet<GridTile>();
        openSet.Add(startNode);

        pathRenderer.positionCount = 0;

        while (openSet.Count > 0)
        {
            GridTile node = openSet.RemoveFirst();
           
            closedSet.Add(node);

            if (node == targetNode)
            {
                RetracePath(startNode, targetNode);
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

    void RetracePath(GridTile start, GridTile end)
    {
        List<GridTile> path = new List<GridTile>();
        GridTile currentNode = end;

        int diagonalCount = 0;
        int verticalOrHorizontalCount = 0;

        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
            
        }

        path.Add(start);

        //This might actually be never needed, but I'm leaving it for now just in case
        path.Reverse();
        createGrid.path = path;

        //This might not be right, but it has to be tested out to know for sure
        pathRenderer.positionCount = path.Count;

        for (int i = 0; i < pathRenderer.positionCount; i++)
        {
            pathRenderer.SetPosition(i, path[i].tileObject.transform.position);
            if(i > 0)
            {

                if(path[i].xPos != path[i - 1].xPos && path[i].zPos != path[i - 1].zPos)
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
