using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Snaps a building to a grid
/// </summary>

public class SnapToGrid : MonoBehaviour {

	CreateGrid createGrid;

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

    //This will allow us to change the hand holding the object without it trying to snap to the grid
	void CheckIfSnapping(){
		if (attached != null) {
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

		if(Physics.Raycast(ray, out hit, 10f, 1 << LayerMask.NameToLayer("Building"))){
			hit.collider.transform.position = new Vector3 (0, 0, 0);
		}


        if (Physics.Raycast(ray, out hit, 10f, 1 << LayerMask.NameToLayer("GridLayer")))
        {
			//Moves the building to the grids position									    just a bit higher than the table, so the building collider won't go inside a table collider
			transform.position = new Vector3 (hit.collider.gameObject.transform.position.x, hit.collider.gameObject.transform.position.y * 1.3f, hit.collider.gameObject.transform.position.z);
				
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
		
	//if there is alredy something in this tile, we move this object away
	public void MoveObjectToPoint(){

		GameObject go = GameObject.Find ("Temporary table/Sphere");
		transform.position = go.transform.position;
	}
}
