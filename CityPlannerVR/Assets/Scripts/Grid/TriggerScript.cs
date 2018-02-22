using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Is used to check if a tile is full before we put the building down.
/// Option1: There is a little bug, where you can put building inside other if you can trigger the OnTriggerEnter() before leting go of the building 
///     (this doesn't break the game however (hopefully))
/// Option1: Is a bit heavy right now (propably)
/// Option2: There is a bug, where some of the tables are moved when initialized 
/// (Possibly because they are so big, that they collide with the collider before trigger)
/// </summary>
public class TriggerScript : MonoBehaviour {

	GridTile.GridState state;

	public void GetGridState(GridTile.GridState _state){
		state = _state;
	}

    //These are the different options to the same problem
    #region option1

    //This list count should and will always be 2 at max, which let's me do some assumptions later
    //It stores the building on this tile and if there are more than one, does something
    //BUG: if both players try to put a building on top of another building, this might cause something unexpected
    List<GameObject> buildings;
    IsAttachedToHand attached;

    void Start()
    {
        buildings = new List<GameObject>();
    }

    //This must have some effect on SnapToGrid
    void OnTriggerEnter(Collider other)
    {
        //If the triggering object is building
        if (other.tag == "Building")
        {
            if(state == GridTile.GridState.Full)
            {
                //Muutetaan meshin väriä hieman punaisemmaksi tai jotain
                
            }

            attached = other.GetComponent<IsAttachedToHand>();
            //Check if the player is not holding the object anymore
            if (!attached.IsHolding)
            {
                #region Check_if_two buildings_are_in_the_same_gridTile
                //This grid has a building now
                buildings.Add(other.gameObject);

                //If there tries to be more than one building in this gridTile
                if (buildings.Count > 1)
                {
                    //If we are trying to put the same building again to this gridTile
                    if (buildings[0].name == buildings[1].name)
                    {
                        //We don't want that
                        buildings.Remove(buildings[0]);
                    }
                    //The building was different
                    else
                    {
                       //We move the old building out of the way, and put the new one here
                       buildings[0].GetComponent<SnapToGrid>().MoveObjectToPoint();
                       buildings.Remove(buildings[0]);                        
                    }
                }
                #endregion
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //There should never be more than one object on one grid object
        //and because the grid object won't trigger unless something is actually placed on it,
        //I can assume that buldings.Count == 1 
        //this check is just in case (if I forget to add some other checks or something)
        if (buildings.Count > 0)
        {
            buildings.Remove(buildings[0]);
        }

        //Muutetaan meshin väri takaisin normaaliksi
    }
    #endregion

    #region option2
    //private void OnTriggerEnter(Collider other)
    //{
    //    attached = other.GetComponent<IsAttachedToHand>();
    //    if (!attached.IsHolding)
    //    {

    //        //Debug.Log ("This tile is full in position " + transform.localPosition);
    //        if (state == GridTile.GridState.Full)
    //        {
    //            Debug.Log(gameObject.name + " is full");
    //            other.GetComponent<SnapToGrid>().MoveObjectToPoint();
    //        }
    //        else
    //        {
    //            Debug.Log("This tile is empty in position ");
    //        }

    //    }
    //}
    #endregion
}
