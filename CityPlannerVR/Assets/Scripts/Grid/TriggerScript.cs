using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour {

	GridTile.GridState state;

	bool isInHand = false;

	public void GetGridState(GridTile.GridState _state){
		state = _state;
	}

    void OnTriggerEnter(Collider other)
    {
		if (other.tag == "Building") {
			if (other.GetComponent<IsAttachadToHand> () != null && !other.GetComponent<IsAttachadToHand> ().IsHolding) {
				if (state == GridTile.GridState.Full) {
					Debug.Log ("This tile is full in position " + transform.localPosition);

					other.GetComponent<SnapToGrid> ().MoveObjectToPoint();
					
				} else {
					Debug.Log ("This tile is empty in position ");
				}
			}
		}
    }
}
