using UnityEngine;
using System.Collections;

public class PlayerCollisionCheck : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
		
	//Separate MonoBehaviour for collisions, because PunBehaviour does not implement collision checks
	private void OnTriggerEnter(Collider other)
	{
		this.gameObject.GetComponent<PhotonPlayerAvatar> ().CollisionHappened (other);
	}
}

