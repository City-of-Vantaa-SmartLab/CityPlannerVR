using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonObjectOwnershipHandler : MonoBehaviour {

	GameObject localPlayer = null;

	private bool TransferOwnershipOnRequest = true;

	private void OnAttachedToHand(Valve.VR.InteractionSystem.Hand hand)
	{
		if (!GetComponent<PhotonView> ().isMine) {
			GetComponent<PhotonView> ().RequestOwnership ();
		}
	}

	public void OnOwnershipRequest(object[] viewAndPlayer)
	{
		PhotonView view = viewAndPlayer[0] as PhotonView;
		PhotonPlayer requestingPlayer = viewAndPlayer[1] as PhotonPlayer;

		Debug.Log("OnOwnershipRequest(): Player " + requestingPlayer + " requests ownership of: " + view + ".");
		if (this.TransferOwnershipOnRequest)
		{
			view.TransferOwnership(requestingPlayer.ID);
		}
	}

	public void OnOwnershipTransfered (object[] viewAndPlayers)
	{
		PhotonView view = viewAndPlayers[0] as PhotonView;

		PhotonPlayer newOwner = viewAndPlayers[1] as PhotonPlayer;

		PhotonPlayer oldOwner = viewAndPlayers[2] as PhotonPlayer;

		Debug.Log( "OnOwnershipTransfered for PhotonView"+view.ToString()+" from "+oldOwner+" to "+newOwner);
	}
}
