using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Toteuta se mitä tapahtuu, jos ruudussa on jo jotain
//TODO: Toteuta tapa katsoa, montako ruutua yksi talo vie ja laita kaikkien state "Full":ksi
//TODO: Korjaa rotaatio
//TODO: Estä taloja roikkumasta pöydän ulkopuolella (eli ettei saa asettaa taloa liian reunalle)

public class SnapToGrid : MonoBehaviour {

    LineRenderer lr;
	CreateGrid cg;

	GridTile tile;

    float scale = 0.025f;

    void Start()
    {
		DrawDebugLine ();

		cg = GameObject.FindGameObjectWithTag ("GridParent").GetComponent<CreateGrid> ();

        SnapPosition();

    }

    private void SnapPosition()
    {
        RaycastHit hit;

        Vector3 Start = transform.position;

        Ray ray = new Ray(Start, new Vector3(0, -1, 0));


        if (Physics.Raycast(ray, out hit, 1f, 1 << LayerMask.NameToLayer("GridLayer")))
        {
			//Get the tile we hit
			tile = cg.GetTileAt (Mathf.FloorToInt(hit.collider.transform.localPosition.x), Mathf.FloorToInt(hit.collider.transform.localPosition.z));

			//If there is nothing on the tile, we can put this object there
			if (tile.State == GridTile.GridState.Empty) {
				//Moves the building to the grids position									    just a bit higher than the table, so the building collider won't go inside table collider
				transform.position = new Vector3 (hit.collider.gameObject.transform.position.x, hit.collider.gameObject.transform.position.y * 1.3f, hit.collider.gameObject.transform.position.z);
				//The tile is now full, and no other objects should be able to be there at the same time
				tile.State = GridTile.GridState.Full;
			} 

			else {
				//There is already something in this tile, so we cannot put this here
				Debug.Log("This tile is full");
			}

			//CheckRotation();
        }
    }

    void CheckRotation()
    {
        float newZ = 0;

        if (transform.rotation.z >= 315 && transform.rotation.z < 45)
        {
            newZ = 0;
        }

        else if (transform.rotation.eulerAngles.z >= 45 && transform.rotation.eulerAngles.z < 135)
        {
            newZ = 90;
        }

        else if (transform.rotation.eulerAngles.z >= 135 && transform.rotation.eulerAngles.z < 225)
        {
            newZ = 180;
        }
        else
        {
            newZ = 270;
        }

        Debug.Log("newZ = " + newZ);

        //Debug.Log("z = " + transform.localRotation.eulerAngles.z);
        //Debug.Log("-z = " + -transform.localRotation.eulerAngles.z);
        //                                           First we reset the z angle and then add the correct angle
        transform.rotation = Quaternion.Euler(-90, 0, /*-transform.rotation.eulerAngles.z +*/ newZ);
    }

	//When we pick an object the tile it was in is set to be empty
    private void OnAttachedToHand(Valve.VR.InteractionSystem.Hand hand)
    {
		tile.State = GridTile.GridState.Empty;
    }

	//When we drop an object it will snap to the grid
    private void OnDetachedFromHand(Valve.VR.InteractionSystem.Hand hand)
    {
        SnapPosition();
    }

	private void DrawDebugLine(){
		lr = gameObject.AddComponent<LineRenderer>();
		//Defines how many points we have to draw the line through
		lr.positionCount = 2;
		//Don't know if I have to use them both, but I'm using them just in case
		lr.startWidth = 0.01f;
		lr.endWidth = 0.01f;
		//Used so the grid scales correctly on the table
		lr.useWorldSpace = false;
		lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

		lr.SetPosition(0, Vector3.zero);
		lr.SetPosition(1, new Vector3(0, 0, -0.5f));
	}
}
