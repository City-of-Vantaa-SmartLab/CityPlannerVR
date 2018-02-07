using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        List<GridTile> openSet = new List<GridTile>();
        HashSet<GridTile> closedSet = new HashSet<GridTile>();
        openSet.Add(startNode);
        Debug.Log("Checkpoint 1");

        while (openSet.Count > 0)
        {
            Debug.Log("Checkpoint 2");
            GridTile node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                Debug.Log("Checkpoint 3");
                if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
                {
                    Debug.Log("Checkpoint 4");
                    if (openSet[i].hCost < node.hCost)
                    {
                        Debug.Log("Checkpoint 5");
                        node = openSet[i];
                    }
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);

            if (node == targetNode)
            {
                //RetracePath(startNode, targetNode);
                return;
            }

            foreach (GridTile neighbour in grid.GetNeighbours(node))
            {
                Debug.Log("Checkpoint 6");
                if (closedSet.Contains(neighbour))
                {
                    continue;
                }

                float newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    Debug.Log("Checkpoint 7");
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
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

	public float GetDistance(GridTile tileA, GridTile tileB){
		float distX = Mathf.Abs (tileA.xPos - tileB.xPos);
		float distZ = Mathf.Abs (tileA.zPos - tileB.zPos);

		if (distX > distZ) {
			//1.4 is the distance needed to go in diagonal line (if normally distance is 1)
			return (1.4f * grid.CellSize) * distZ + grid.CellSize * (distX - distZ);
		}
		return (1.4f * grid.CellSize) * distX + grid.CellSize * (distZ - distX);
	}
}
