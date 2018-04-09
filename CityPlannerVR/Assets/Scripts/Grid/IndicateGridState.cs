using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicateGridState : MonoBehaviour {

    BoxCollider trigger;
	GridTileStateCheck state;

	SnapToGrid snapToGrid;


    void Awake()
    {
		trigger = GetComponent<BoxCollider> ();
		state = GetComponent<GridTileStateCheck> ();
    }

	void OnCollisionEnter(Collision other){
		if(state.tile.State == GridTile.GridState.Full){
			snapToGrid = other.gameObject.GetComponent<SnapToGrid> ();

			if (snapToGrid != null) {
				snapToGrid.triggeredTiles.Add (gameObject);
			}
		}
	}

	void OnCollisionExit(Collision other){
		if(state.tile.State == GridTile.GridState.Full){
			if (snapToGrid != null) {
				snapToGrid.triggeredTiles.Remove (gameObject);
			}
		}
	}


}
