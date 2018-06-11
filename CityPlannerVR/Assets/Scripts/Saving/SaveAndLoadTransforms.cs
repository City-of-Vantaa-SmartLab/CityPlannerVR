using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveAndLoadTransforms : MonoBehaviour {

    [Serializable] //attributes for json
    public class TransformData
    {
        public string gameObjectName;
        public Vector3 localPosition;
        public Quaternion localRotation;
    }

    GameObject photonBuildingsGO;
    List<GameObject> photonBuildings;

    public string localPlayerName;
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
        SaveData<TransformData>.OnBeforeSaveTransforms += HandleBeforeSave;
        SaveData<TransformData>.OnLoadedTransforms += HandleLoading;
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
        throw new NotImplementedException();
    }

    public void Save()
    {
        SaveData<TransformData>.SaveItems(pathName, SaveData<TransformData>.transformContainer);
    }
    //add generation script?
    public void Load()
    {
        SaveData<TransformData>.LoadItems(pathName);
    }

    private void StoreData(Transform t)
    {
        TransformData data = new TransformData();
        data.localPosition = t.localPosition;
        data.localRotation = t.localRotation;
        data.gameObjectName = t.gameObject.name;
        //SaveData<TransformData>.AddbuildingData(data);
        //SaveData<TransformData>.transformContainer

    }

    internal static void RelocateTransform(TransformData data)
    {
        GameObject temp = GameObject.Find(data.gameObjectName);
        temp.transform.localPosition = data.localPosition;
        temp.transform.localRotation = data.localRotation;
    }

    public static Comment CreateComment()
    {
        Comment comment = new Comment();
        return comment;
    }

    public static void MoveOldTransform(TransformData data)
    {

        //Comment comment = CreateComment();
        //comment.data = data;
        ////comment.LoadData();
        //comment.SortAndAddToLocalList();
        //return comment;
    }
}
