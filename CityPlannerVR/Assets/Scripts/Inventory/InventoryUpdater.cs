using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

/// <summary>
/// Works with Item Database and LocalObject Spawner Scripts
/// </summary>
public class InventoryUpdater : MonoBehaviour
{

    [SerializeField]
    public GameObject itemDB;
    [SerializeField]
    public ItemDatabase itemData;
    private LocalObjectSpawner[] spawners;
    public int databaseSize;
    public int item_int = 0;


    private void Start()

    {

        itemData = itemDB.GetComponent<ItemDatabase>();
        databaseSize = itemData.listSize;
        spawners = gameObject.GetComponentsInChildren<LocalObjectSpawner>(true);
        InitItems();
    }

    public void InitItems()
    {

        foreach (LocalObjectSpawner spawn in spawners)
        {
            spawn.item = itemData.DBAccess(item_int);
            spawn.itemsInSpawner = new List<GameObject>();
            spawn.InstantiateItem();
            item_int++;
        }
    }

    public void NextItems()
    {
        if (item_int < databaseSize)
        {
            foreach (LocalObjectSpawner spawn in spawners)
            {
                if (item_int <= databaseSize)
                {
                    LocalObjectSpawner _spawn = spawn;
                    Debug.Log("ButtonUP");
                    DestroyCurrent(_spawn);
                    spawn.item = itemData.DBAccessUP(item_int);
                    Debug.Log("new item added");
                    item_int++;
                    spawn.itemsInSpawner = new List<GameObject>();
                    spawn.InstantiateItem();
                    Debug.Log("New crap added");
                }

            }
        }

        else if (item_int >= databaseSize)
        {
            Debug.Log("ButtonUPDeactivated");
        }
        else
            Debug.Log("What the hell happened");

    }


    public void PreviousItems()
    {
        if (item_int <= 9)
        {
            Debug.Log("DeactivateButton");
        }
        else
        {
            item_int = item_int - 18;
            foreach (LocalObjectSpawner spawn in spawners)
            {
                LocalObjectSpawner _spawn = spawn;
                Debug.Log("ButtonDown");
                DestroyCurrent(_spawn);

                spawn.item = itemData.DBAccessDown(item_int);
                item_int++;
                spawn.itemsInSpawner = new List<GameObject>();
                spawn.InstantiateItem();
                Debug.Log("New crap added");
            }
        }



        /* foreach (GameObject spawnslot in this.GetComponentsInChildren<GameObject>())
         {
             Debug.Log("ButtonDown");
             spawner = spawnslot.GetComponent<ObjectSpawner>();
             //spawner.item = null;
             spawner.item = itemData.DBAccessDown();
             spawner.itemsInSpawner = new List<GameObject>();
             spawner.InstantiateItem();
             */

    }

    public void DestroyCurrent(LocalObjectSpawner spawnedObject)
    {
        Debug.Log("Destroy Pushed");
        //List<GameObject> spawners = new List<GameObject>();

        //foreach (LocalObjectSpawner spawnedObject in spawners)
        // {
        Debug.Log("Lets Destroy");
        spawnedObject.item = null;
        spawnedObject.itemsInSpawner.Clear();
        spawnedObject.DestroyItem();


        //}
    }

}