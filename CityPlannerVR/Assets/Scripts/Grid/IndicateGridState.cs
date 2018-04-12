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

	void OnTriggerEnter(Collider other){
        if (state.tile.State == GridTile.GridState.Full)
        {
            if (state.ObjectOnThisTile != other.gameObject)
            {
                snapToGrid = other.gameObject.GetComponent<SnapToGrid>();

                if (snapToGrid != null)
                {
                    snapToGrid.triggeredTiles.Add(gameObject);
                }
            }
        }
	}

	void OnTriggerExit(Collider other){
        if (state.tile.State == GridTile.GridState.Full)
        {

            if (state.ObjectOnThisTile != other.gameObject)
            {

                if (snapToGrid != null)
                {
                    snapToGrid.triggeredTiles.Remove(gameObject);
                }
            }
        }
	}


}
