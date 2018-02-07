using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO: Change and optimize sometime
//  -Add possible visual effect to show the path
//  -Deside if we will ever need the Retrace function
//  -Test if this actually works
//  -Make it so that the "full" tiles are not walkable

public class Pathfinding : MonoBehaviour {

	CreateGrid grid;

	// Use this for initialization 
	void Start () {
		grid = GetComponent<CreateGrid> ();
	}

    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        GridTile startNode = grid.GetTileAt(startPos.x, startPos.z);
        GridTile targetNode = grid.GetTileAt(targetPos.x, targetPos.z);

        Heap<GridTile> openSet = new Heap<GridTile>(grid.MaxSize);
        HashSet<GridTile> closedSet = new HashSet<GridTile>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            GridTile node = openSet.RemoveFirst();
           
            closedSet.Add(node);

            if (node == targetNode)
            {
                //RetracePath(startNode, targetNode);
                return;
            }

            foreach (GridTile neighbour in grid.GetNeighbours(node))
            {
                if (closedSet.Contains(neighbour))
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

 //   void RetracePath(GridTile start, GridTile end){
	//	List<GridTile> path = new List<GridTile> ();
	//	GridTile currentNode = endTile;

	//	while (currentNode != start) {
	//		path.Add (currentNode);
	//		currentNode = currentNode.parent;
	//	}
	//	path.Reverse ();
	//	grid.path = path;
	//}
}
