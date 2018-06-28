﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class PhotonSpawnableObject : MonoBehaviour {

	#region Public Attributes

	[SerializeField]
	public string itemPrefabName;

	[SerializeField]
	private string objectTag = "Spawnable";

	[SerializeField]
	public Transform spawnPoint;

	public GameObject itemInSpawner;

	public InputMaster inputMaster;

	private bool isFirstTime = true;

    #endregion

   //Use this for initialization
	void Start()
	{
		
	}

	void Update() {
		if (isFirstTime) {
			itemPrefabName = "Inventory/"+itemPrefabName;

			inputMaster = GameObject.Find("Player").GetComponent<InputMaster>();
			isFirstTime = false;

			this.GetItems (null);
		}
	}

   public void GetItems (GameObject dbitem) {

        if (PhotonNetwork.isMasterClient)
        {

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
		if (inputMaster == null) {
			inputMaster = GameObject.Find("Player").GetComponent<InputMaster>();
		}
		Debug.Log ("Spawner entered");
		if (other.CompareTag("GameController"))
		{
			Debug.LogWarning(this.name + " triggered by " + other.name);
			inputMaster.TriggerClicked += HandleTriggerClicked;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (inputMaster == null) {
			inputMaster = GameObject.Find("Player").GetComponent<InputMaster>();
		}
		Debug.Log ("Spawner exited");
		if (other.CompareTag("GameController"))
		{
			Debug.LogWarning(this.name + " exited by " + other.name);
			inputMaster.TriggerClicked -= HandleTriggerClicked;
		}
	}

	private void HandleTriggerClicked(object sender, ClickedEventArgs e)
	{
		Debug.LogWarning ("Trigger clicked");
		InstantiateRealItem (e.controllerIndex);
	}

	public void InstantiateItemInSpawner()
	{
		GameObject clone = PhotonNetwork.Instantiate(itemPrefabName, spawnPoint.position, spawnPoint.rotation, 0);

		clone.GetComponent<PhotonView> ().RPC ("FreezeObjectInSpawner", PhotonTargets.AllBuffered, clone.GetComponent<PhotonView>().viewID);

		itemInSpawner = clone;
		Debug.Log ("Spawner item instantiated");
	}

	private void InstantiateRealItem(uint controllerIndex)
	{
		Debug.LogWarning ("Starting to instantiate item");
		GameObject clone = PhotonNetwork.Instantiate(itemPrefabName, spawnPoint.position, spawnPoint.rotation, 0);

		Hand hand;

		if (controllerIndex == 1) {
			hand = GameObject.Find ("Hand1").GetComponent<Hand>();
		} else if (controllerIndex == 2) {
			hand = GameObject.Find ("Hand2").GetComponent<Hand>();
		} else {
			Debug.LogWarning ("Failed finding correct hand!");
			hand = null;
		}

		if (hand != null) {
			hand.AttachObject (clone);
		}

		Debug.LogWarning ("Real item instantiated");
	}

    public void DestroyItemInSpawner()
    {
        Destroy(itemInSpawner);
    }

}
