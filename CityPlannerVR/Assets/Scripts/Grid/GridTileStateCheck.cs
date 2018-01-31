using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTileStateCheck : MonoBehaviour {

	//This gets its value from GridTile.cs when its created
	public GridTile tile = null;
	//This is used just for debugging
	public GridTile.GridState tileState;

	void Start(){
		//This line can be removed after everything works
		tileState = tile.State;
	}

	//If something that is building collides with me, I'm full
	void OnCollisionEnter(Collision other){
		if (other.collider.tag == "Building") {
			tile.State = GridTile.GridState.Full;
			//This line can be removed after everything works
			tileState = tile.State;
		}
	}

	//I will become empty if the collsion stops
	void OnCollisionExit(Collision other){
		if (other.collider.tag == "Building") {
			tile.State = GridTile.GridState.Empty;
			//This line can be removed after everything works
			tileState = tile.State;
		}
	}
}
