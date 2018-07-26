using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// Saves and loads transforms in order from files. Adds gameobjects from prefabs to scene if necessary.
/// Add holders (the parents) to a list in inspector, after which they can be stored via 
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

    static public Transform defaultParentCleanup;
    private string folderPathName;
    private string folder;
    private string defaultFileName;
    private string startupFileName;
    private string fileExtender;
    private string testFileName;
    private string latestFileName;
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
        testFileName = "Test";
        latestFileName = "Latest";
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
        RestoreToState(SaveData.transformContainer);
        //Debug.Log("OnLoadedTransforms event fired");
    }


    #region File and list manipulation

    /// <summary>
    /// Save transformcontainer to local file and DB using default settings.
    /// </summary>

    public void Save()
    {
        Save(defaultFileName, SaveData.transformContainer, true);
    }

    /// <summary>
    /// Load to transformcontainer from DB using default settings.
    /// </summary>

    public void Load()
    {
        Load(defaultFileName, true);
    }

    /// <summary>
    /// Saves a container to a filepath derived from filename parameter. Optionally uploads the savedata to a database using default settings.
    /// </summary>

    public void Save(string fileName, Container<TransformData> container, bool useDatabase)
    {
        HandleBeforeSave();  //remove this if the event system is implemented
        pathName = folderPathName + slash + fileName + fileExtender;
        SaveData.SaveDatas(pathName, container);
        if (useDatabase)
        {
            MongoDBAPI.UseDefaultConnections();
            MongoDBAPI.ImportJSONFileToDatabase(MongoDBAPI.transformCollection, pathName);
        }
    }

    public void Load(string fileName, bool useDatabase)
    {
        pathName = folderPathName + slash + fileName + fileExtender;
        if (useDatabase)
        {
            MongoDBAPI.UseDefaultConnections();
            MongoDBAPI.ExportJSONFileFromDatabase(MongoDBAPI.transformCollection, pathName);
        }
        SaveData.LoadItems<TransformData>(pathName);
    }



    public void SaveStartupList()
    {
        //SaveData.transformContainer.datas.Clear();
        //SaveData.ClearContainer(SaveData.transformContainer);
        //StoreList(startupHolderList);
        Save(startupFileName, SaveData.startupContainer, false);
    }

    /// <summary>
    /// Load startup items from a dedicated file.
    /// </summary>

    private void LoadStartup(bool useDatabase)
    {
        pathName = folderPathName + slash + startupFileName + fileExtender;
        if (useDatabase)
            MongoDBAPI.ExportJSONFileFromDatabase(MongoDBAPI.transformCollection, pathName);
        SaveData.startupContainer = SaveData.LoadDatas<TransformData>(pathName);
    }

    private void StoreList(List<GameObject> holders)
    {
        foreach (GameObject GO in holders)
        {
            if (GO == null)
            {
                Debug.Log("GameObject is null!");
                continue;
            }
            StoreData(GO.transform);  //stores the holder
            Debug.Log("Storing holder's transform: " + GO.name);
            foreach (Transform tr in GO.transform)
            {
                StoreData(tr);  //stores the holder's children
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

    #endregion


    #region Restoring objects

    /// <summary>
    /// Finds or creates a prefab and places it under its parent with saved transform coordinates.
    /// </summary>

    public static void RestoreTransform(TransformData data, Transform previousHolder)
    {
        GameObject temp = GameObject.Find(data.gameObjectName);
        if (temp)
            PutInPlace(temp, data, previousHolder);
        else
        {
            Debug.Log("Could not find gameobject with name " + data.gameObjectName + ", creating from prefab...)");

            temp = CreateFromPrefab(data);
            if (temp)
                PutInPlace(temp, data, previousHolder);
            else
                Debug.Log("Could not find prefab!");
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

    private static GameObject CreateFromPrefab(TransformData data)
    {
        GameObject temp;
        //System.Object[] what = new System.Object[0];
        //temp = PhotonNetwork.InstantiateSceneObject(data.gameObjectName, data.localPosition, data.localRotation, 0, what);
        //if (!temp)
            temp = PhotonNetwork.InstantiateSceneObject("Prefabs/" + data.gameObjectName, data.localPosition, data.localRotation, 0, null);
        if (!temp)
            temp = PhotonNetwork.InstantiateSceneObject("Prefabs/PhotonNewBuildings/" + data.gameObjectName, data.localPosition, data.localRotation, 0, null); ; //add more of these if necessary

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
        temp = CreateFromPrefab(data);
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


    #region Restoring scene

    public void ResetScene()
    {
        if (SaveData.startupContainer  == null)
            LoadStartup(false);
        if (SaveData.startupContainer == null)
            LoadStartup(true);
        RestoreToState(SaveData.startupContainer);
    }


    private static void ClearLevelFrom(Container<TransformData> holders)
    {
        foreach (TransformData data in holders.datas)
        {
            GameObject temp = GameObject.Find(data.gameObjectName);
            if (temp)
                PhotonNetwork.Destroy(temp);
        }
    }

    // returns C = A ∩ B, where A,B and C are containers. Also modifies A = A ∉ B and B = B ∉ A.
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
                    filterThenLeftOvers.datas.Remove(data2); //use filterThenLeftOvers to delete unwanted objects later
                    break;
                }
            }
        }
        return sharedGOS;
    }

    public void RestoreToState(Container<TransformData> ItemsLoaded)
    {
        SaveData.ClearContainer(SaveData.transformContainer);
        //StoreList(holdersToBeSaved);  //redundant?

        Container<TransformData> sharedContainer;
        sharedContainer = SeparateSharedGOS(ItemsLoaded, SaveData.transformContainer);  //use different container for robustness?
        ClearLevelFrom(SaveData.transformContainer);   //delete excess gameobjects
        
        foreach (TransformData data in sharedContainer.datas)
        {
            RestoreTransform(data, sharedContainer.previousHolder);  //find and place shared objects
        }

        //foreach (TransformData data in ItemsLoaded.datas)
        //{
        //    GameObject temp = CreateFromPrefab(data);  //instantiate the rest without searching for them  //created duplicates!
        //    if (temp)
        //        PutInPlace(temp, data, ItemsLoaded.previousHolder);
        //}

        foreach (TransformData data in ItemsLoaded.datas)
        {
            RestoreTransform(data, ItemsLoaded.previousHolder);  //the child objects might come from prefabs after all
        }

    }

    /// <summary>
    /// In case we want to avoid the default settings.
    /// </summary>

    public void LoadTransformsFromFile(string filepath)
    {
        Container<TransformData> tempContainer;
        tempContainer = SaveData.LoadDatas<TransformData>(filepath);
        RestoreToState(tempContainer);
    }



    #endregion


    #region QuickCecking

    public int GenerateQuickCheck(TransformData data, int subStringMaxLength)
    {
        string objectName = Comment.TruncateString(data.gameObjectName, subStringMaxLength);
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

    #region Database accessing



    public void TestDatabaseMethod1()
    {
        pathName = folderPathName + slash + testFileName + fileExtender;
        MongoDBAPI.TestMethod1(pathName);
    }

    //public void SaveLatestToDatabase()
    //{
    //    pathName = folderPathName + slash + latestFileName + fileExtender;
    //    SaveData.SaveFileToDatabase(pathName, 1);
    //}

    //public void LoadLatestFromDatabase()
    //{
    //    pathName = folderPathName + slash + latestFileName + fileExtender;
    //    MongoDBAPI.UseDefaultConnections();
    //    MongoDBAPI.ImportJSONFileToDatabase(MongoDBAPI.transformCollection, pathName);
    //}



    #endregion

}
