using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Snaps a building to a grid
/// </summary>

public class SnapToGrid : MonoBehaviour {

	IsAttachedToHand attached;

    [HideInInspector]
    public GameObject parent;

    private bool isOnGrid = false;
    public bool IsOnGrid
    {
        get
        {
            return isOnGrid;
        }
        set
        {
            isOnGrid = value;
            if (isOnGrid)
            {
                if (!ObjectContainer.objects.Contains(gameObject))
                {
                    //Adds this gameObject to a static list for easy access
                    ObjectContainer.objects.Add(gameObject);
                    //Debug.Log(gameObject.name + " is in the list now.");
                }
            }
            else
            {
                if (ObjectContainer.objects.Contains(gameObject))
                {
                    //If the object is in the list, remove it
                    ObjectContainer.objects.Remove(gameObject);
                    //Debug.Log(gameObject.name + " removed from the list now.");
                }
            }
        }
    }

    void Start()
    {
        SnapPosition();
		attached = GetComponent<IsAttachedToHand> ();

		if (attached != null) {
			attached.OnSnapToGrid += CheckIfSnapping;
		}
    }

	void OnDestroy(){

        //Removes this object from the static list
        ObjectContainer.objects.Remove(gameObject);
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

        if (Physics.Raycast(ray, out hit, 10f, 1 << LayerMask.NameToLayer("GridLayer")))
        {
			//Moves the object to the grids position									    just a bit higher than the table, so the object collider won't go inside a table collider
			transform.position = new Vector3 (hit.collider.gameObject.transform.position.x, hit.collider.gameObject.transform.position.y * 1.3f, hit.collider.gameObject.transform.position.z);
            //transform.parent = parent.transform;

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

		transform.position = ObjectContainer.trashPoint;
	}
}
