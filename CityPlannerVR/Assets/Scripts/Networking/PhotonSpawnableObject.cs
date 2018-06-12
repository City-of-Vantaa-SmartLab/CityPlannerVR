﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class PhotonSpawnableObject : MonoBehaviour {

	#region Private Attributes

	[SerializeField]
	public string itemPrefabName;

	[SerializeField]
	private string objectTag = "Spawnable";

	[SerializeField]
	public Transform spawnPoint;

	public GameObject itemInSpawner;

	public InputMaster inputMaster;

    #endregion

   //Use this for initialization

   public void GetItems (GameObject dbitem) {

        if (PhotonNetwork.isMasterClient)
        {

            itemInSpawner = dbitem;
            itemPrefabName = "Inventory/"+dbitem.name;

            inputMaster = GameObject.Find("Player").GetComponent<InputMaster>();

            if (PhotonGameManager.Instance.isMultiplayerSceneLoaded)
            {

                InstantiateItemInSpawner();

            }
            else
            {
                PhotonGameManager.OnMultiplayerSceneLoaded += () =>
                {

                    InstantiateItemInSpawner();
                };
            }
        }
    }

    private void OnTriggerEnter(Collider other)
	{
		Debug.Log ("Spawner entered");
		if (other.CompareTag("GameController"))
		{
			Debug.Log(this.name + " triggered by " + other.name);
			inputMaster.TriggerClicked += HandleTriggerClicked;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		Debug.Log ("Spawner exited");
		if (other.CompareTag("GameController"))
		{
			Debug.Log(this.name + " exited by " + other.name);
			inputMaster.TriggerClicked -= HandleTriggerClicked;
		}
	}

	private void HandleTriggerClicked(object sender, ClickedEventArgs e)
	{
		Debug.Log ("Trigger clicked");
		InstantiateRealItem (e.controllerIndex);
	}

	public void InstantiateItemInSpawner()
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
		Debug.Log ("Spawner item instantiated");
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

		Debug.Log ("Real item instantiated");
	}

    public void DestroyItemInSpawner()
    {
        Destroy(itemInSpawner);
    }
}
