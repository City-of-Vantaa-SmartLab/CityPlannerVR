﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToGrid : MonoBehaviour {

	CreateGrid createGrid;

	GridTile tile;

	IsAttachedToHand attached;

    void Start()
    {
        createGrid = GameObject.FindGameObjectWithTag ("GridParent").GetComponent<CreateGrid> ();

        SnapPosition();
		attached = GetComponent<IsAttachedToHand> ();

		if (attached != null) {
			attached.OnSnapToGrid += CheckIfSnapping;
		}
    }

	void CheckIfSnapping(){
		if (attached != null) {
			Debug.Log ("IsHolding a building: " + attached.IsHolding);
			if (!attached.IsHolding) {
				SnapPosition ();
			}
		}
	}

    private void SnapPosition()
    {
        RaycastHit hit;

        Vector3 Start = transform.position;

        Ray ray = new Ray(Start, Vector3.down);


        if (Physics.Raycast(ray, out hit, 10f, 1 << LayerMask.NameToLayer("GridLayer")))
        {
			//Get the tile we hit
			tile = createGrid.GetTileAt (hit.collider.transform.localPosition.x, hit.collider.transform.localPosition.z);

			//If there is nothing on the tile, we can put this object there
			if (tile.State == GridTile.GridState.Empty) {
				//Moves the building to the grids position									    just a bit higher than the table, so the building collider won't go inside table collider
				transform.position = new Vector3 (hit.collider.gameObject.transform.position.x, hit.collider.gameObject.transform.position.y * 1.3f, hit.collider.gameObject.transform.position.z);
				//The tile is now full, and no other objects should be able to be there at the same time
				//This might not be necessary anymore, but I will look into it later (maybe)
				//tile.State = GridTile.GridState.Full;
			} 

			else {
				//There is already something in this tile, so we cannot put this here
				Debug.Log("This tile is full");

				//These are different solutions to same problem. We'll test them out 
				MoveObjectToPoint ();
			}
				
			CheckRotation();
        }
    }

    void CheckRotation()
    {
        float newY = 0;

		if (transform.rotation.eulerAngles.y >= 0 && transform.rotation.eulerAngles.y < 45)
        {
            newY = 0;
        }

		else if(transform.rotation.eulerAngles.y >= 315 && transform.rotation.eulerAngles.y < 360){
			newY = 0;
		}

        else if (transform.rotation.eulerAngles.y >= 45 && transform.rotation.eulerAngles.y < 135)
        {
            newY = 90;
        }

        else if (transform.rotation.eulerAngles.y >= 135 && transform.rotation.eulerAngles.y < 225)
        {
            newY = 180;
        }

        else
        {
            newY = 270;
        }

        transform.rotation = Quaternion.Euler(0, newY, 0);
    }

    //This might also be unnecessary
	//When we pick an object, the tile it was in is set to be empty
  //  private void OnAttachedToHand(Valve.VR.InteractionSystem.Hand hand)
  //  {
		//tile.State = GridTile.GridState.Empty;
  //  }

		
	//if there is alredy something in this tile, we move this object away
	public void MoveObjectToPoint(){

		GameObject go = GameObject.Find ("Temporary table/Sphere");
		transform.position = go.transform.position;
	}
}
