using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class PhotonSpawnableObject : MonoBehaviour {

	#region Private Attributes

	[SerializeField]
	private string itemPrefabName;

	[SerializeField]
	private string objectTag = "Spawnable";

	[SerializeField]
	private Transform spawnPoint;

	private GameObject itemInSpawner;

	private InputMaster inputMaster;

	#endregion

	// Use this for initialization
	void Start () {

		itemInSpawner = new GameObject();
		inputMaster = GameObject.Find("Player").GetComponent<InputMaster>();

		if (PhotonGameManager.Instance.isMultiplayerSceneLoaded) {
			InstantiateItemInSpawner ();
		} else {
			PhotonGameManager.OnMultiplayerSceneLoaded += () =>
			{
				InstantiateItemInSpawner();
			};
		}

	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("GameController"))
		{
			Debug.Log(this.name + " triggered by " + other.name);
			inputMaster.TriggerClicked += HandleTriggerClicked;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("GameController"))
		{
			Debug.Log(this.name + " exited by " + other.name);
			inputMaster.TriggerClicked -= HandleTriggerClicked;
		}
	}

	private void HandleTriggerClicked(object sender, ClickedEventArgs e)
	{
		InstantiateRealItem (e.controllerIndex);
	}

	private void InstantiateItemInSpawner()
	{
		GameObject clone = PhotonNetwork.Instantiate(itemPrefabName, spawnPoint.position, spawnPoint.rotation, 0);

		Rigidbody r_clone = clone.GetComponent<Rigidbody>();
		clone.transform.SetParent(this.transform);
		r_clone.constraints = RigidbodyConstraints.FreezeAll;

		BoxCollider colliderB = clone.GetComponent<BoxCollider> ();
		if (colliderB != null) {
			colliderB.enabled = false;
		}
		CapsuleCollider colliderC = clone.GetComponent<CapsuleCollider> ();
		if (colliderC != null) {
			colliderC.enabled = false;
		}

		itemInSpawner = clone;
	}

	private void InstantiateRealItem(uint controllerIndex)
	{
		GameObject clone = PhotonNetwork.Instantiate(itemPrefabName, spawnPoint.position, spawnPoint.rotation, 0);

		Hand hand;

		if (controllerIndex == 1) {
			hand = GameObject.Find ("Hand1").GetComponent<Hand>();
		} else if (controllerIndex == 2) {
			hand = GameObject.Find ("Hand2").GetComponent<Hand>();
		} else {
			Debug.Log ("Failed finding correct hand!");
			hand = null;
		}

		if (hand != null) {
			hand.AttachObject (clone);
		}
	}
}
