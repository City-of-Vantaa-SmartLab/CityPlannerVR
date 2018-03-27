using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonObjectSpawner : MonoBehaviour {

	#region Private Attributes

	[SerializeField]
	private string itemPrefabName;

	[SerializeField]
	private string objectTag = "Spawnable";

	[SerializeField]
	private Transform spawnPoint;

	private List<GameObject> itemsInSpawner;

	#endregion

	void Start() {

		itemsInSpawner = new List<GameObject>();

		if (PhotonGameManager.Instance.isMultiplayerSceneLoaded) {
			InstantiateItem ();
		} else {
			PhotonGameManager.OnMultiplayerSceneLoaded += () =>
			{
				InstantiateItem();
			};
		}
	}

	// If oncoming item is a Spawnable, add to list of items in spawner
	// Take care of items with multiple colliders
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag(objectTag))
		{
			bool objectFound = false;

			// Spawnable objects can have multiple colliders, so same
			// object can trigger OnTriggerEnter multiple times.
			foreach (GameObject go in itemsInSpawner)
			{
				if (go.GetInstanceID() == other.gameObject.GetInstanceID())
				{
					// Found a match in the items list for this object.
					// This mean that this gameobject has already triggered OnTriggerEnter
					// and has been previously added to the items list. Do not
					// add it a second time.
					//Debug.Log(other.gameObject.name + " already found in here! Do not add a second time!");
					objectFound = true;
				}
			}

			if (objectFound == false)
			{
				itemsInSpawner.Add(other.gameObject);
				//Debug.Log("Added item: " + other.gameObject.name);
			}
		}
	}

	// If exiting item is a Spawnable (and not e.g a players controller),
	// remove it from items in spawner list. If there are no items left
	// in the spawner, spawn a new one. Take care of items with multiple colliders
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag(objectTag))
		{
			bool found = false;

			foreach (GameObject go in itemsInSpawner)
			{
				if (go.GetInstanceID() == other.gameObject.GetInstanceID())
				{
					// Found a match in the item list for this object.
					// This means that this gameObject has not yet triggered
					// a OnTriggerExit and we should remove this from the list.

					// If a match for this GameObject is not found, it most likely
					// means that it has already been removed previously, so do not
					// try to remove it again.
					//Debug.Log(other.gameObject.name + " found, first instance of exiting collider, this should not be seen twice");

					found = true;
				}
			}

			if (found == true)
			{
				Rigidbody r_body = other.gameObject.GetComponent<Rigidbody>();
				r_body.constraints = RigidbodyConstraints.None;
				itemsInSpawner.Remove(other.gameObject);
				if (itemsInSpawner.Count == 0)
				{
					Debug.Log ("Creating new item!");
					InstantiateItem();
				}
			}
		}
	}

	private void InstantiateItem()
	{
		GameObject clone = PhotonNetwork.Instantiate(itemPrefabName, spawnPoint.position, spawnPoint.rotation, 0);
		Rigidbody r_clone = clone.GetComponent<Rigidbody>();

		clone.transform.SetParent(this.transform);
		r_clone.constraints = RigidbodyConstraints.FreezeAll;
		itemsInSpawner.Add(clone);
	}
}
