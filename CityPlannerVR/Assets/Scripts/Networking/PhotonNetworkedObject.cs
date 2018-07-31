﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonNetworkedObject : Photon.MonoBehaviour {

	#region Private Variables

	//Continuosly lerp toward these to correct the position/rotation
	private Vector3 correctObjectPos = Vector3.zero;
	private Quaternion correctObjectRot = Quaternion.identity;

	private bool isInHand = false;
	private IsAttachedToHand isAttachedToHand = null;

    private bool firstTime = true;

    #endregion

    //variables for destroying the object if on the floor
    public int destroyCountermax = 5;
    public float destroyWaitSeconds = 1.0f;
	
	// Update is called once per frame
	void Update () {


        if (isAttachedToHand == null) {
			isAttachedToHand = this.gameObject.GetComponent<IsAttachedToHand> ();

		}

		if (!photonView.isMine) {
			transform.position = Vector3.Lerp (transform.position, this.correctObjectPos, Time.deltaTime * 5);
			transform.rotation = Quaternion.Lerp (transform.rotation, this.correctObjectRot, Time.deltaTime * 5);
		} else {
			if (this.gameObject.GetComponent<Rigidbody> ().isKinematic && isInHand == false) {
				//Debug.LogWarning ("IsKinematic true!");
				isInHand = true;
				this.gameObject.GetComponent<PhotonView> ().RPC ("SetIsKinematic", PhotonTargets.AllBuffered, isInHand);
			} else if (!this.gameObject.GetComponent<Rigidbody> ().isKinematic && isInHand == true) {
				//Debug.LogWarning ("IsKinematic false!");
				isInHand = false;
				this.gameObject.GetComponent<PhotonView> ().RPC ("SetIsKinematic", PhotonTargets.AllBuffered, isInHand);
			}
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

	[PunRPC]
	public void SetIsKinematic(bool isTrue)
	{
		this.gameObject.GetComponent<Rigidbody> ().isKinematic = isTrue;
	}

	[PunRPC]
	public void FreezeObjectInSpawner(int viewId)
	{
		GameObject obj = PhotonView.Find (viewId).gameObject;
		if (obj != null) {
			Rigidbody rigidObj = obj.GetComponent<Rigidbody> ();
			//obj.transform.SetParent(this.transform);
			rigidObj.constraints = RigidbodyConstraints.FreezeAll;

			BoxCollider colliderB = obj.GetComponent<BoxCollider> ();
			if (colliderB != null) {
				colliderB.enabled = false;
			}
			CapsuleCollider colliderC = obj.GetComponent<CapsuleCollider> ();
			if (colliderC != null) {
				colliderC.enabled = false;
			}
		}
	}

    public IEnumerator ObjectDestroyCounter()
    {
        //Debug.Log(gameObject.name + " Coroutine Started");
        int counter = 0;
        while (counter < destroyCountermax)
        {
            counter++;
            yield return new WaitForSeconds(destroyWaitSeconds);
        }
        PhotonNetwork.Destroy(gameObject);
    }


}
