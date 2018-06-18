using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// Saves and loads transforms in order from files. Adds gameobjects from prefabs to scene if necessary.
/// </summary>

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
    public List<GameObject> startupHolderList = new List<GameObject>();

    public string localPlayerName;
    static public Transform defaultParentCleanup;
    private string folderPathName;
    private string folder;
    private string defaultFileName;
    private string startupFileName;
    private string fileExtender;
    private string pathName;
    private char slash = Path.DirectorySeparatorChar;
    //public bool save;
    //public bool load;

    private void Awake()
    {
        folder = "SaveData";
        defaultFileName = "TransformData";
        startupFileName = "StartupTransformData";
        fileExtender = ".dat";
        folderPathName = Application.persistentDataPath + slash + folder;

        if (!defaultParentCleanup)
            defaultParentCleanup = GameObject.Find("CleanUp").transform;
        if (!defaultParentCleanup)
            Debug.LogError("Default parent could not be set for cleaning up!");
        SubscriptionOn();

        //LoadStartup();
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
        StoreList(holdersToBeSaved);
    }

    private void HandleLoading()
    {
        Debug.Log("OnLoadedTransforms event fired");
    }

    #region File and list manipulation

    public void Save()
    {
        Save(defaultFileName, SaveData.transformContainer);
    }

    public void Load()
    {
        Load(defaultFileName);
    }

    public void Save(string fileName, Container<TransformData> container)
    {
        pathName = folderPathName + slash + fileName + fileExtender;
        SaveData.SaveDatas(pathName, container);
    }

    public void Load(string fileName)
    {
        pathName = folderPathName + slash + fileName + fileExtender;
        SaveData.LoadItems<TransformData>(pathName);
    }

    public void SaveStartupList()
    {
        //SaveData.transformContainer.datas.Clear();
        SaveData.ClearContainer(SaveData.transformContainer);
        StoreList(startupHolderList);
        Save(startupFileName, SaveData.transformContainer);
    }

    public void LoadStartup()
    {
        Load(startupFileName);
    }

    private void StoreList(List<GameObject> gameObjects)
    {
        foreach (GameObject GO in gameObjects)
        {
            StoreData(GO.transform);
            Debug.Log("Storing holder's transform: " + GO.name);
            foreach (Transform tr in GO.transform)
            {
                StoreData(tr);
                //Debug.Log("Storing " + tr.name);
            }
        }
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

    public void GenerateDefaultDataFile()
    {
        StoreList(startupHolderList);
        Debug.Log("Transformdata file created for startup");
    }

    #endregion

    #region Restoring objects

    public static void RestoreFromContainer<T>(Container<T> tempContainer)
    {
        Container<TransformData> currentItems = SaveData.transformContainer;

        //Container<TransformData> sharedGOS = SeparateSharedGOS(tempContainer, currentItems);
    }


    public static void RestoreTransform(TransformData data, Transform previousHolder)
    {
        GameObject temp = GameObject.Find(data.gameObjectName);
        if (temp)
            PutInPlace(temp, data, previousHolder);
        else
        {
            Debug.Log("Could not find gameobject with name " + data.gameObjectName + ", creating from prefab...)");

            temp = CreateFromPrefab(data.gameObjectName);
            if (temp)
                PutInPlace(temp, data, previousHolder);
        }
    }

    private static void PutInPlace(GameObject GO, TransformData data, Transform previousHolder)
    {
        Transform parent = CheckOrFindOrGenerateParent(data, previousHolder);
        if (parent)
            GO.transform.parent = parent;
        GO.transform.localPosition = data.localPosition;
        GO.transform.localRotation = data.localRotation;
    }

    private static GameObject CreateFromPrefab(string GOName)
    {
        GameObject temp;
        temp = Resources.Load<GameObject>("Prefabs/" + GOName);
        if (!temp)
            temp = Resources.Load<GameObject>("Prefabs/Buildings" + GOName); //add more of these if necessary

        if (temp)
            return temp;
        else
            return null;
    }

    private static Transform CheckOrFindOrGenerateParent(TransformData data, Transform previousHolder)
    {
        if (previousHolder != null)
        {
            if (data.gameObjectParentName == previousHolder.name)
                return previousHolder;
        }
        GameObject temp = GameObject.Find(data.gameObjectParentName);
        if (temp)
        {
            SaveData.transformContainer.previousHolder = temp.transform;
            return temp.transform;
        }
        temp = CreateFromPrefab(data.gameObjectParentName);
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

    #endregion

    #region QuickCecking

    public int GenerateQuickCheck(TransformData data, int subStringMaxLength)
    {
        string objectName = Comment.TruncateString(data.gameObjectName , subStringMaxLength);
        string parentName = Comment.TruncateString(data.gameObjectParentName, subStringMaxLength);
        
        string uberString = objectName + parentName;
        //Debug.Log("Joining strings: " + objectName + " " + parentName);
        int magic = Comment.ConvertFirstCharsToInt(uberString, subStringMaxLength * 2);
        //Debug.Log("QuickCheck: " + magic);
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

    #endregion


    public static void ClearLevelFrom(Container<TransformData> holders)
    {
        foreach (TransformData data in holders.datas)
        {
            GameObject temp = GameObject.Find(data.gameObjectName);
            if (temp)
                Destroy(temp);
        }
    }

    static private Container<TransformData> SeparateSharedGOS (Container<TransformData> loadingThenMissing, Container<TransformData> filterThenLeftOvers)
    {
        Container<TransformData> sharedGOS = new Container<TransformData>();
        foreach(TransformData data1 in loadingThenMissing.datas)
        {
            foreach (TransformData data2 in filterThenLeftOvers.datas)
            {
                if (data1.gameObjectName == data2.gameObjectName)
                {
                    sharedGOS.datas.Add(data1);
                    loadingThenMissing.datas.Remove(data1);
                    filterThenLeftOvers.datas.Remove(data2); //use holder2 later to delete unwanted objects
                    break;
                }
            }
        }
        return sharedGOS;
    }

    public void RestoreToState(Container<TransformData> ItemsLoaded)
    {
        //Container<TransformData> newItems = SaveData.LoadDatas<TransformData>(filepath);
        if (SaveData.transformContainer.datas.Count != 0)
        {
            Container<TransformData> sharedContainer;
            sharedContainer = SeparateSharedGOS(ItemsLoaded, SaveData.transformContainer);  //use different container for robustness?
            ClearLevelFrom(SaveData.transformContainer);  //delete excess gameobjects
            foreach (TransformData data in sharedContainer.datas)
            {
                RestoreTransform(data, sharedContainer.previousHolder);  //find and place shared objects
            }
        }

        foreach (TransformData data in ItemsLoaded.datas)
        {
            GameObject temp = CreateFromPrefab(data.gameObjectName);  //instantiate the rest without searching for them
            if (temp)
                PutInPlace(temp, data, ItemsLoaded.previousHolder);
        }

    }

    public void LoadTransformsFromFile(string filepath)
    {
        Container<TransformData> tempContainer;
        tempContainer = SaveData.LoadDatas<TransformData>(filepath);
        RestoreToState(tempContainer);
    }

}
