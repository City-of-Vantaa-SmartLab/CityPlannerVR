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
    private Container<TransformData> startupContainer = new Container<TransformData>();

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
        //Debug.Log("OnLoadedTransforms event fired");
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
        HandleBeforeSave();  //remove this if the event system is implemented
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
        //SaveData.ClearContainer(SaveData.transformContainer);
        //StoreList(startupHolderList);
        Save(startupFileName, startupContainer);
    }

    public void LoadStartup()
    {
        Load(startupFileName);
    }

    private void StoreList(List<GameObject> holders)
    {
        foreach (GameObject GO in holders)
        {
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

    public static void ClearLevelFrom(Container<TransformData> holders)
    {
        foreach (TransformData data in holders.datas)
        {
            GameObject temp = GameObject.Find(data.gameObjectName);
            if (temp)
                PhotonNetwork.Destroy(temp);
        }
    }

    public void ClearLevelFromStartupItems()
    {
        ClearLevelFrom(startupContainer);
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
        StoreList(holdersToBeSaved);

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



}
