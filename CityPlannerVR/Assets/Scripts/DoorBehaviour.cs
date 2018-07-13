using UnityEngine;
using System.Collections;
using Valve.VR.InteractionSystem;

public class DoorBehaviour : MonoBehaviour
{

	private bool isActive = true;

	// Use this for initialization
	void Start ()
	{
	}
	
	public void CloseDoor()
	{
		isActive = false;
		this.gameObject.GetComponent<Interactable> ().enabled = false;
	}
}

