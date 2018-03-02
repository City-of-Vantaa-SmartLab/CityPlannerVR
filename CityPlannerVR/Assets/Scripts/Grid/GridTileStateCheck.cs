using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks and changes the state of the tile according to colliding objects
/// </summary>

public class GridTileStateCheck : MonoBehaviour {

	//This gets its value from GridTile.cs when its created
	public GridTile tile = null;

    //This is part of the option1
    GameObject objectOnThisTile = null;

    private string objectTag = "Building";

    //If something that is building collides with me, I'm full
    void OnCollisionEnter(Collision other){
		if (other.collider.CompareTag(objectTag)) {

            #region Option1
            if(tile.State == GridTile.GridState.Full)
            {
                objectOnThisTile.transform.position = ObjectContainer.trashPoint;
            }
            #endregion

            #region option2
            //if (tile.State == GridTile.GridState.Full)
            //{
            //    other.transform.position = ObjectContainer.trashPoint;
            //}
            #endregion

            tile.State = GridTile.GridState.Full;
            objectOnThisTile = other.gameObject;
            other.gameObject.GetComponent<SnapToGrid>().IsOnGrid = true;
		}
	}

	//I will become empty if the collsion stops
	void OnCollisionExit(Collision other){
		if (other.collider.CompareTag(objectTag)) {
			tile.State = GridTile.GridState.Empty;
            objectOnThisTile = null;
            other.gameObject.GetComponent<SnapToGrid>().IsOnGrid = false;
        }
	}
}
