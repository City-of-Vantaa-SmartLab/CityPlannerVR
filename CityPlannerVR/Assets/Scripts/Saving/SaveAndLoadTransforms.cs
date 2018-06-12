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
    public int quickcheck;
}

public class SaveAndLoadTransforms : MonoBehaviour {

    public List<GameObject> holdersToBeSaved = new List<GameObject>();

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
        SubscriptionOn();

        Load();
    }

    private void OnDestroy()
    {
        SubscriptionOff();
    }

    private void SubscriptionOn()
    {
        SaveData.OnBeforeSaveTransforms += HandleBeforeSave;
        SaveData.OnLoadedTransforms += HandleLoading;
    }

    private void SubscriptionOff()
    {
        SaveData.OnBeforeSaveTransforms -= HandleBeforeSave;
        SaveData.OnLoadedTransforms -= HandleLoading;
    }



    private void HandleBeforeSave()
    {
        foreach (GameObject GO in holdersToBeSaved)
        {
            StoreData(GO.transform);
            Debug.Log("Storing holder's transform: " + GO.name);
            foreach (Transform tr in GO.transform)
            {
                StoreData(tr);
                Debug.Log("Storing " + tr.name);
            }
        }
    }

    private void HandleLoading()
    {
        Debug.Log("OnLoadedTransforms event fired");
    }

    public void Save()
    {
        SaveData.SaveDatas(pathName, SaveData.transformContainer);
    }

    public void Load()
    {
        SaveData.LoadItems<TransformData>(pathName);
    }

    private void StoreData(Transform t)
    {
        TransformData data = new TransformData();
        bool alreadyReplaced = false;

        data.localPosition = t.localPosition;
        data.localRotation = t.localRotation;
        data.gameObjectName = t.gameObject.name;
        data.gameObjectParentName = t.parent.name;
        data.quickcheck = GenerateQuickCheck(data, 3);
        foreach (TransformData trdata in SaveData.transformContainer.datas)
        {
            if (IsTheSameTransform(trdata, data))
            {
                trdata.localPosition = data.localPosition;
                trdata.localRotation = data.localRotation;
                alreadyReplaced = true;
                break;
            }
        }

        if (!alreadyReplaced)
            SaveData.AddData(data);
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

    public int GenerateQuickCheck(TransformData data, int subStringMaxLength)
    {
        string objectName = Comment.TruncateString(data.gameObjectName , subStringMaxLength);
        string parentName = Comment.TruncateString(data.gameObjectParentName, subStringMaxLength);
        
        string uberString = objectName + parentName;
        Debug.Log("Joining strings: " + objectName + " " + parentName);
        int magic = Comment.ConvertFirstCharsToInt(uberString, subStringMaxLength * 2);
        Debug.Log("QuickCheck: " + magic);
        return magic;
    }



    public static bool IsTheSameTransform(TransformData trdata1, TransformData trdata2)
    {
        if (trdata1.quickcheck == trdata2.quickcheck &&  //comparing ints is quicker than strings
            trdata1.gameObjectName == trdata2.gameObjectName &&
            trdata1.gameObjectParentName == trdata2.gameObjectParentName
            )
            return true;
        else
            return false;
    }



}
