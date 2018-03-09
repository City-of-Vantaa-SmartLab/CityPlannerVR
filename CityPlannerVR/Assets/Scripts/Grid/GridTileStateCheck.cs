using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTileStateCheck : MonoBehaviour {

	//This gets its value from GridTile.cs when its created
	public GridTile tile = null;

	//If something that is building collides with me, I'm full
	void OnCollisionEnter(Collision other){
		if (other.collider.tag == "Building") {
			tile.State = GridTile.GridState.Full;
			tile.containedObject = other.gameObject;
            other.gameObject.GetComponent<SnapToGrid>().IsOnGrid = true;
		}
	}

	//I will become empty if the collsion stops
	void OnCollisionExit(Collision other){
		if (other.collider.tag == "Building") {
			tile.State = GridTile.GridState.Empty;
			tile.containedObject = null;
            other.gameObject.GetComponent<SnapToGrid>().IsOnGrid = false;
        }
	}
}