﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Transformdata is a package, which is used with SaveAndLoadTransforms class to export
/// and import gameobject's positional data with external files.
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

/// <summary>
/// Used in saving and loading a predefined set of transforms (in SaveData class) to an external file.
/// It is also used when storing said data to a database, with the help of MongoDBAPI.
/// </summary>

public class SaveAndLoadTransforms : MonoBehaviour {

    //public List<GameObject> holdersToBeSaved = new List<GameObject>();  //moved under SaveData
    //public List<GameObject> startupHolderList = new List<GameObject>();

    static public Transform defaultParentCleanup;
    private string folderPathName;
    private string folder;
    private string defaultFileName;
    private string startupFileName;
    private string fileExtenderDat;
    private string fileExtenderPng;
    private string testFileName;
    private string latestFileName;
    private string pathNameFolder;
    private char slash = Path.DirectorySeparatorChar;

    private void Awake()
    {
        folder = "SaveData";
        defaultFileName = "TransformData";
        startupFileName = "StartupTransformData";
        fileExtenderDat = ".dat";
        fileExtenderPng = ".png";
        testFileName = "TEST2";
        latestFileName = "Latest";
        folderPathName = Application.persistentDataPath + slash + folder;

        if (!defaultParentCleanup)
            defaultParentCleanup = GameObject.Find("CleanUp").transform ?? new GameObject("CleanUp").transform;
        SubscriptionOn();

    }


    private void OnDestroy()
    {
        SubscriptionOff();
    }

    private void SubscriptionOn()
    {
        SaveData.OnLoadedTransforms += HandleLoading;
    }

    private void SubscriptionOff()
    {
        SaveData.OnLoadedTransforms -= HandleLoading;
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

    public void SaveAsync()
    {
        Task saveTask = new Task(() =>Save(defaultFileName, SaveData.transformContainer, false));
        saveTask.Start();
        //Save(defaultFileName, SaveData.transformContainer, false);
    }

    /// <summary>
    /// Load to transformcontainer from DB using default settings.
    /// </summary>

    public void LoadAsync()
    {
        Task loadTask = new Task(() => Load(defaultFileName, true));
        loadTask.Start();
        //Load(defaultFileName, true);
    }

    /// <summary>
    /// Saves a container to a filepath derived from filename parameter. Optionally uploads the savedata to a database using default settings.
    /// </summary>

    public void Save(string fileName, Container<TransformData> container, bool useDatabase)
    {
        StartCoroutine(SaveWhenContainerIsReady(fileName, container, useDatabase));
    }

    private IEnumerator SaveWhenContainerIsReady(string fileName, Container<TransformData> container, bool useDatabase)
    {
        //Debug.Log("Starting coroutine...");
        pathNameFolder = folderPathName + slash + fileName + fileExtenderDat;

        SaveData.transformCount = 0;
        SaveData.BeforeSavingTransforms();

        while (SaveData.transformCount < SaveData.amountOfTransforms)
        {
            Debug.Log("Waiting for 0,1s....");
            yield return new WaitForSeconds(.1f);
        }
        SaveData.transformCount = 0;
        SaveData.SaveDatas(pathNameFolder, container);
        //SaveData.ClearContainer(container);
        if (useDatabase)
        {
            if (MongoDBAPI.UseDefaultConnections())
                MongoDBAPI.ImportJSONFileToDatabase(MongoDBAPI.transformCollection, pathNameFolder);
        }
        //Debug.Log("Saving done!");
    }

    public void Load(string fileName, bool useDatabase)
    {
        pathNameFolder = folderPathName + slash + fileName + fileExtenderDat;
        if (useDatabase)
        {
            if (MongoDBAPI.UseDefaultConnections())
                MongoDBAPI.ExportJSONFileFromDatabase<TransformData>(MongoDBAPI.transformCollection, pathNameFolder);
            //MongoDBAPI.ExportContainersFromDatabase<TransformData>(MongoDBAPI.transformCollection);
        }
        else
            SaveData.LoadItems<TransformData>(pathNameFolder);
    }

    /// <summary>
    /// For redundancy and testing alternative startups
    /// </summary>

    public void SaveStartupList()
    {
        Save(startupFileName, SaveData.transformContainer, false);
    }

    /// <summary>
    /// Load startup items from a dedicated file.
    /// </summary>

    private void LoadStartup(bool useDatabase)
    {
        //pathName = folderPathName + slash + startupFileName + fileExtender;
        //if (useDatabase)
        //    MongoDBAPI.ExportJSONFileFromDatabase(MongoDBAPI.transformCollection, pathName);
        //SaveData.startupContainer = SaveData.LoadDatas<TransformData>(pathName);
        Load(startupFileName, useDatabase);
    }

    private void StoreList(List<GameObject> GOSToBeSaved)
    {
        foreach (GameObject GO in GOSToBeSaved)
        {
            if (GO == null)
            {
                Debug.Log("GameObject is null!");
                continue;
            }
            StoreData(GO.transform);  //stores the holder
            Debug.Log("Storing holder's transform: " + GO.name);
            //foreach (Transform tr in GO.transform)
            //{
            //    StoreData(tr);  //stores the holder's children
            //    //Debug.Log("Storing " + tr.name);
            //}
        }
    }

    /// <summary>
    /// Evaluates a transform's properties to a transformdata class, which is added to savedatas transformcontainer.
    /// If a transformdata with the same gameobjectname is found, its position is updated instead of adding it again to the container.
    /// </summary>

    public static void StoreData(Transform t)
    {
        TransformData data = GenerateTransformData(t);
        bool alreadyReplaced = false;

        //foreach (TransformData trdata in SaveData.transformContainer.datas)  //This was in place for duplicates. Redundant since better container management
        //{
        //    if (IsTheSameTransform(trdata, data))
        //    {
        //        trdata.localPosition = data.localPosition;
        //        trdata.localRotation = data.localRotation;
        //        alreadyReplaced = true;
        //        break;
        //    }
        //}

        if (!alreadyReplaced)
            SaveData.AddData(data);
    }

    public static TransformData GenerateTransformData(Transform t)
    {
        TransformData data = new TransformData
        {
            localPosition = t.localPosition,
            localRotation = t.localRotation,
            gameObjectName = t.gameObject.name,
        };

        if (t.parent == null)
            data.gameObjectParentName = "";
        else
            data.gameObjectParentName = t.parent.name;
        data.quickcheck = GenerateQuickCheck(data, 2);
        return data;
    }

    #endregion


    #region Restoring objects

    /// <summary>
    /// Finds or creates a prefab and places it under its parent with saved transform coordinates.
    /// </summary>

    public static void RestoreTransform(GameObject objectToBeRestored, TransformData data, Transform previousHolder, bool createNewWithoutSearching)
    {
        if (!createNewWithoutSearching)
        {
            if (objectToBeRestored == null)
                objectToBeRestored = GameObject.Find(data.gameObjectName);
            if (objectToBeRestored)
                PutInPlace(objectToBeRestored, data, previousHolder);
            else
            {
                Debug.Log("Could not find gameobject with name " + data.gameObjectName + ", creating from prefab...)");
            }
        }

        objectToBeRestored = CreateFromPrefab(data);
        if (objectToBeRestored)
            PutInPlace(objectToBeRestored, data, previousHolder);
        else
            Debug.Log("Could not find prefab!");
    }

    private static void PutInPlace(GameObject GO, TransformData data, Transform previousHolder)
    {
        Transform parent = CheckForParent(data, previousHolder);
        if (parent)
            GO.transform.parent = parent;
        GO.transform.localPosition = data.localPosition;
        GO.transform.localRotation = data.localRotation;
    }

    private static GameObject CreateFromPrefab(TransformData data)
    {
        string GOName = RemovePossibleCloneFromEnd(data.gameObjectName);
        GameObject temp;
        //System.Object[] what = new System.Object[0];
        //temp = PhotonNetwork.InstantiateSceneObject(data.gameObjectName, data.localPosition, data.localRotation, 0, what);
        //if (!temp)

        //object superTemp = Resources.Load("Prefabs/" + GOName);
        //if (superTemp == null)
        //    superTemp = Resources.Load("Prefabs/Inventory/" + GOName);
        //if (superTemp == null)
        //    superTemp = Resources.Load("Prefabs/PhotonNewBuildings/" + GOName);

        temp = PhotonNetwork.InstantiateSceneObject("Inventory/" + GOName, data.localPosition, data.localRotation, 0, null);
        if (!temp)
            temp = PhotonNetwork.InstantiateSceneObject("Prefabs/PhotonNewBuildings/" + GOName, data.localPosition, data.localRotation, 0, null); ; //add more of these if necessary
        if (!temp)
            temp = PhotonNetwork.InstantiateSceneObject("Prefabs/" + GOName, data.localPosition, data.localRotation, 0, null); ; 

        if (temp)
        {
            if (temp.GetComponent<SaveThisAsTransform>() == null)
                temp.AddComponent<SaveThisAsTransform>();
            return temp;
        }
        else
            return null;
    }

    private static string RemovePossibleCloneFromEnd(string gameObjectName)
    {
        //Debug.Log("Removing possible clone...");
        string cloneString = "(Clone)";
        if (gameObjectName.EndsWith(cloneString))
        {
            //Debug.Log("Removing clonestring!");
            string temp = gameObjectName.Substring(0, gameObjectName.Length - cloneString.Length);
            return temp;
        }

        return gameObjectName;
    }

    private static Transform CheckForParent(TransformData data, Transform previousHolder)
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
        //temp = CreateFromPrefab(data);
        //if (temp)
        //{
        //    SaveData.transformContainer.previousHolder = temp.transform;
        //    return temp.transform;
        //}
        else if (defaultParentCleanup)
        {
            //Debug.Log("Could not find parent for " + data.gameObjectName + " with name: "
            //    + data.gameObjectParentName + ", moving under cleanup object");
            return defaultParentCleanup;
        }
        else
            return null;
    }

    #endregion


    #region Restoring scene

    public void ResetSceneQuick()
    {
        SaveData.ResetScene(true);

        //if (SaveData.startupContainer  == null)
        //    LoadStartup(false);
        //if (SaveData.startupContainer == null)
        //    LoadStartup(true);
        //RestoreToState(SaveData.startupContainer);
    }

    public void ResetSceneFromDefaultFile()
    {
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


    /// <summary>
    /// Returns C = A ∩ B, where A,B and C are containers. Also modifies A = A ∉ B and B = B ∉ A.
    /// </summary>

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

    /// <summary>
    /// 
    /// </summary>

    public void RestoreToState(Container<TransformData> ItemsLoaded)
    {
        SaveData.ClearContainer(SaveData.transformContainer);
        //StoreList(holdersToBeSaved);  //redundant?

        Container<TransformData> sharedContainer;
        sharedContainer = SeparateSharedGOS(ItemsLoaded, SaveData.transformContainer);  //use different container for robustness?
        ClearLevelFrom(SaveData.transformContainer);   //delete excess gameobjects
        
        foreach (TransformData data in sharedContainer.datas)
        {
            RestoreTransform(null, data, sharedContainer.previousHolder, false);  //find and place shared objects
        }

        //foreach (TransformData data in ItemsLoaded.datas)
        //{
        //    GameObject temp = CreateFromPrefab(data);  //instantiate the rest without searching for them  //created duplicates!
        //    if (temp)
        //        PutInPlace(temp, data, ItemsLoaded.previousHolder);
        //}

        foreach (TransformData data in ItemsLoaded.datas)
        {
            RestoreTransform(null, data, ItemsLoaded.previousHolder, true);
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

    public static int GenerateQuickCheck(TransformData data, int subStringMaxLength)
    {
        string objectName;
        string parentName;
        if (!string.IsNullOrEmpty(data.gameObjectName))
            objectName = Comment.TruncateString(data.gameObjectName, subStringMaxLength);
        else
            objectName = "";

        if (!string.IsNullOrEmpty(data.gameObjectParentName))
            parentName = Comment.TruncateString(data.gameObjectParentName, subStringMaxLength);
        else
            parentName = "";

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



    //public void TestDatabaseMethod1()
    //{
    //    pathNameFolder = folderPathName + slash + testFileName + fileExtenderPng;
    //    MongoDBAPI.UseDefaultConnections();
    //    MongoDBAPI.TestMethod1(pathNameFolder, testFileName + fileExtenderPng);
    //}

    //public void TestDatabaseMethod2()
    //{
    //    pathNameFolder = folderPathName + slash + testFileName + fileExtenderPng;
    //    MongoDBAPI.UseDefaultConnections();
    //    MongoDBAPI.TestMethod2(folderPathName);
    //}

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
