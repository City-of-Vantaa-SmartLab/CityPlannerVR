﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public List<GameObject> InventoryDB = new List<GameObject>();
    public int listSize;
    public int check;


    private void Awake()
    {

        foreach (GameObject item in Resources.LoadAll("Inventory", typeof(GameObject)))
        {

            InventoryDB.Add(item);
            listSize = InventoryDB.Count;

        }

        /*  int i = 0;
          foreach (GameObject spawnslot in _inventory.GetComponentsInChildren<GameObject>(true))
          {
              spawner = spawnslot.GetComponent<ObjectSpawner>();
              spawner.item = InventoryDB[i];
              i++;
          }*/

    }
    public GameObject DBAccess(int _i)
    {
        GameObject spawnitem;
        int i = _i;
        spawnitem = InventoryDB[i];


        return spawnitem;
    }

    public GameObject DBAccessUP(int _i)
    {
        GameObject spawnitem;
        int i = _i;
        spawnitem = InventoryDB[i];

        return spawnitem;
    }


    public GameObject DBAccessDown(int _i)
    {
        GameObject spawnitem;
        int i = _i;
        spawnitem = InventoryDB[i];
        return spawnitem;


    }
}
