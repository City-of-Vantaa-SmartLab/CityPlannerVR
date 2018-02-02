using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

	CreateGrid grid;

	public GameObject startPos;
	public GameObject endPos;

	GridTile startTile;
	GridTile endTile;

	// Use this for initialization
	void Start () {
		grid = GetComponent<CreateGrid> ();
	}

	void Update(){
		FindPath ();
	}
	
	void FindPath(){
		startTile = startPos.GetComponent<MeasurementPoint> ().Tile;
		endTile = endPos.GetComponent<MeasurementPoint> ().Tile;

		List<GridTile> openSet = new List<GridTile>();
		HashSet<GridTile> closedSet = new HashSet<GridTile> ();

		openSet.Add (startTile);
		while (openSet.Count > 0) {
			GridTile currentTile = openSet [0];
			Debug.Log (openSet.Count);
			//TÄSSÄ ON BUGI AINAKIN
			for (int i = 1; i < openSet.Count; i++) {
				//This is heavy, and can be optimized. I will do it, if I have the time (heap optimization)
				Debug.Log ("openSet[i].fCost = " + openSet [i].fCost);
				Debug.Log ("currentTile.fCost = " + currentTile.fCost);
				Debug.Log ("openSet[i].hCost = " + openSet[i].hCost);
				Debug.Log ("currentTile.hCost = " + currentTile.hCost);

				if (openSet [i].fCost < currentTile.fCost || openSet[i].fCost == currentTile.fCost && openSet[i].hCost < currentTile.hCost) {
					currentTile = openSet [i];
				}
			}

			openSet.Remove (currentTile);
			closedSet.Add (currentTile);

			if (currentTile == endTile) {
				RetracePath (startTile, endTile);
				return;
			}

			foreach (GridTile neighbour in grid.GetNeighbours(currentTile)) {
				if (closedSet.Contains (neighbour)) {
					continue;
				}

				int newMovementCostToNeighbour = currentTile.gCost + GetDistance (currentTile, neighbour);
				if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains (neighbour)) {
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = GetDistance (neighbour, endTile);
					neighbour.parent = currentTile;

					if (!openSet.Contains (neighbour)) {
						openSet.Add (neighbour);
					}
				}
			}
		}
	}

	void RetracePath(GridTile start, GridTile end){
		List<GridTile> path = new List<GridTile> ();
		GridTile currentNode = endTile;

		while (currentNode != start) {
			path.Add (currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse ();
		grid.path = path;
	}

	int GetDistance(GridTile tileA, GridTile tileB){
		int distX = Mathf.Abs (tileA.xPos - tileB.xPos);
		int distZ = Mathf.Abs (tileA.zPos - tileB.zPos);

		if (distX > distZ) {
			//TODO: Change these number later to represent bigger than 1 x 1 m grid
			Debug.Log(14 * distZ + 10 * (distX - distZ));
			return 14 * distZ + 10 * (distX - distZ);
		}
		Debug.Log(14 * distX + 10 * (distZ - distX));
		return 14 * distX + 10 * (distZ - distX);
	}
}
