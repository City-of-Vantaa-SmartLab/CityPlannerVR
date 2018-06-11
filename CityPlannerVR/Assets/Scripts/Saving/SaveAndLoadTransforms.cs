using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[Serializable] //attributes for json
public class TransformData
{
    public string gameObjectName;
    public string gameObjectParentName;
    public Vector3 localPosition;
    public Quaternion localRotation;
}

public class SaveAndLoadTransforms : MonoBehaviour {

    GameObject photonBuildingsGO;
    List<GameObject> photonBuildings;

    public string localPlayerName;
    static public Transform defaultParentCleanup;
    private string folderPathName;
    private string folder;
    private string fileName;
    private string fileExtender;
    private string pathName;
    private char slash = Path.DirectorySeparatorChar;
    //public bool save;
    //public bool load;

    private void Awake()
    {
        folder = "SaveData";
        fileName = "TransformData";
        fileExtender = ".dat";
        folderPathName = Application.persistentDataPath + slash + folder;
        pathName = folderPathName + slash + fileName + fileExtender;

        if (!defaultParentCleanup)
            defaultParentCleanup = GameObject.Find("CleanUp").transform;
        if (!defaultParentCleanup)
            Debug.LogError("Default parent could not be set for cleaning up!");

        Load();
    }



    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (!photonBuildingsGO)
            photonBuildingsGO = GameObject.Find("PhotonNewBuildings");
    }


    private void SubscriptionOn()
    {
        //inputMaster.MenuButtonClicked += HandleMenuClicked;
        SaveData.OnBeforeSaveTransforms += HandleBeforeSave;
        SaveData.OnLoadedTransforms += HandleLoading;
    }



    private void HandleBeforeSave()
    {
        if (photonBuildingsGO)
        {
            foreach (Transform tr in photonBuildingsGO.transform)
            {
                StoreData(tr);
            }
        }
    }

    private void HandleLoading()
    {
        Debug.Log("Transforms loaded");
    }

    public void Save()
    {
        SaveData.SaveDatas(pathName, SaveData.transformContainer);
    }
    //add generation script?
    public void Load()
    {
        SaveData.LoadItems<TransformData>(pathName);
    }

    private void StoreData(Transform t)
    {
        TransformData data = new TransformData();
        data.localPosition = t.localPosition;
        data.localRotation = t.localRotation;
        data.gameObjectName = t.gameObject.name;
        data.gameObjectParentName = t.parent.name;
        //SaveData<TransformData>.AddbuildingData(data);
        //SaveData<TransformData>.transformContainer

    }

    public static void RestoreTransform(TransformData data, Transform holder)
    {
        GameObject temp = GameObject.Find(data.gameObjectName);
        if (temp)
            PutInPlace(temp, data, holder);
        else
        {
            Debug.Log("Could not find gameobject with name " + data.gameObjectName + ", creating from prefab...)");
            GameObject newObject;
            newObject = Resources.Load<GameObject>("Prefabs/" + data.gameObjectName);
            if (newObject)
                PutInPlace(newObject, data, holder);
        }
    }

    private static void PutInPlace(GameObject GO, TransformData data, Transform holder)
    {
        Transform parent = CheckOrFindOrGenerateParent(data, holder);
        if (parent)
            GO.transform.parent = parent;
        GO.transform.localPosition = data.localPosition;
        GO.transform.localRotation = data.localRotation;
    }

    private static Transform CheckOrFindOrGenerateParent(TransformData data, Transform previousHolder)
    {
        if (data.gameObjectParentName == previousHolder.name)
            return previousHolder;
        GameObject temp = GameObject.Find(data.gameObjectParentName);
        if (temp)
        {
            SaveData.transformContainer.previousHolder = temp.transform;
            return temp.transform;
        }
        temp = Resources.Load<GameObject>("Prefabs/" + data.gameObjectParentName);
        if (temp)
        {
            SaveData.transformContainer.previousHolder = temp.transform;
            return temp.transform;
        }
        else if (defaultParentCleanup)
        {
            Debug.Log("Could not find parent for " + data.gameObjectName + " with name: "
                + data.gameObjectParentName + ", moving under cleanup object");
            return defaultParentCleanup;
        }
        else
            return null;
    }



}
