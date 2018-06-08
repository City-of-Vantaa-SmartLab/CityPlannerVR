using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonNetworkedObject : Photon.MonoBehaviour {

	#region Private Variables

	//Continuosly lerp toward these to correct the position/rotation
	private Vector3 correctObjectPos = Vector3.zero;
	private Quaternion correctObjectRot = Quaternion.identity;

	private bool isInHand = false;
	private IsAttachedToHand isAttachedToHand = null;

	#endregion
	
	// Update is called once per frame
	void Update () {

		if (isAttachedToHand == null) {
			isAttachedToHand = this.gameObject.GetComponent<IsAttachedToHand> ();
		}

		if (!photonView.isMine) {
			transform.position = Vector3.Lerp (transform.position, this.correctObjectPos, Time.deltaTime * 5);
			transform.rotation = Quaternion.Lerp (transform.rotation, this.correctObjectRot, Time.deltaTime * 5);
		} else {
			if (isAttachedToHand.IsHolding && !isInHand) {
				isInHand = true;
			} else if(!isAttachedToHand.IsHolding && isInHand){
				isInHand = false;
			}
		}

		if (isInHand && this.gameObject.GetComponent<Rigidbody>().useGravity) {
			this.gameObject.GetComponent<Rigidbody> ().useGravity = false;
		}
		if(!isInHand && !this.gameObject.GetComponent<Rigidbody>().useGravity) {
			this.gameObject.GetComponent<Rigidbody> ().useGravity = true;
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);

		}
		else
		{
			// Network player, receive data
			this.correctObjectPos = (Vector3)stream.ReceiveNext();
			this.correctObjectRot = (Quaternion)stream.ReceiveNext();

		}
	}
}
