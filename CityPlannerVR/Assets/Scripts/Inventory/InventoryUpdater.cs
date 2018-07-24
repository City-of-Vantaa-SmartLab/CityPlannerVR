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
    private PhotonSpawnableObject[] spawners;
    public int databaseSize;
    public int item_int = 0;
    public float lastitemdec;
    public float lastitemf;
    public int lastitem;


    private void Start()

    {
        itemDB = GameObject.Find("ItemDatabase");
        itemData = itemDB.GetComponent<ItemDatabase>();
        databaseSize = itemData.listSize;

        float checknumber = (databaseSize*1.0f / 9.0f);
        lastitemdec = Mathf.Repeat(checknumber, 1.0f);
     
        if (lastitemdec == 0.0f)
            lastitem = 9;
        else
        {
            lastitemf = 9 * lastitemdec;
            lastitem = Mathf.RoundToInt(lastitemf);
        }
            
       
        spawners = gameObject.GetComponentsInChildren<PhotonSpawnableObject>(true);
        foreach (PhotonSpawnableObject spawn in spawners)
        {
            spawn.spawnPoint = spawn.gameObject.transform;
        }
            PhotonInitItems();
    }

    public void PhotonInitItems()
    {
        
        foreach (PhotonSpawnableObject spawn in spawners)
        {
            spawn.GetItems(itemData.DBAccess(item_int));
            item_int++;
        }
    }

    public void NextItems()
    {
        if (item_int < databaseSize)
        {
            foreach (PhotonSpawnableObject spawn in spawners)
            {
                if (item_int <= databaseSize)
                {
                    PhotonSpawnableObject _spawn = spawn;
                    Debug.Log("ButtonUP");
                    DestroyCurrent(_spawn);
                    spawn.itemInSpawner = itemData.DBAccessUP(item_int);
                    Debug.Log("new item added");
                    item_int++;
                    spawn.InstantiateLocalItemInSpawner(spawn.itemInSpawner);
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
            item_int = 9;
        }
        else if (item_int == databaseSize)
        {
            item_int = item_int - (9 + lastitem);
            foreach (PhotonSpawnableObject spawn in spawners)
            {
                PhotonSpawnableObject _spawn = spawn;
                Debug.Log("ButtonDown");
                DestroyCurrent(_spawn);

                spawn.itemInSpawner = itemData.DBAccessDown(item_int);
                item_int++;
                spawn.InstantiateLocalItemInSpawner(spawn.itemInSpawner);
                Debug.Log("New crap added");
            }
        }

        else
        {
            item_int = item_int - 18;
            foreach (PhotonSpawnableObject spawn in spawners)
            {
                PhotonSpawnableObject _spawn = spawn;
                Debug.Log("ButtonDown");
                DestroyCurrent(_spawn);

                spawn.itemInSpawner = itemData.DBAccessDown(item_int);
                item_int++;
                spawn.InstantiateLocalItemInSpawner(spawn.itemInSpawner);
                Debug.Log("New crap added");
            }
        }

    }

    public void DestroyCurrent(PhotonSpawnableObject spawnedObject)
    {
        Debug.Log("Destroy Pushed");
        Debug.Log("Lets Destroy");
        spawnedObject.DestroyItemInSpawner();


        //}
    }

}