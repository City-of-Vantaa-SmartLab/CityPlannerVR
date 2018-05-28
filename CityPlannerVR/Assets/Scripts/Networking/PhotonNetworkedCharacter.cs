using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonNetworkedCharacter : Photon.MonoBehaviour {

	#region Private Variables

	//Continuosly lerp toward these to correct the position/rotation
	private Vector3 correctPlayerPos = Vector3.zero;
	private Quaternion correctPlayerRot = Quaternion.identity;
    private Vector3 correctPlayerScale = Vector3.one;

	#endregion

	
	// Update is called once per frame
	void Update () {

		if (!photonView.isMine)
		{
			transform.position = Vector3.Lerp(transform.position, this.correctPlayerPos, Time.deltaTime * 5);
			transform.rotation = Quaternion.Lerp(transform.rotation, this.correctPlayerRot, Time.deltaTime * 5);
            transform.localScale = Vector3.Lerp(transform.localScale, this.correctPlayerScale, Time.deltaTime * 5);
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
            stream.SendNext(transform.localScale);

		}
		else
		{
			// Network player, receive data
			this.correctPlayerPos = (Vector3)stream.ReceiveNext();
			this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
            this.correctPlayerScale = (Vector3)stream.ReceiveNext();
		}
	}
}
