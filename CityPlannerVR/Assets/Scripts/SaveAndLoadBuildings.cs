using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveAndLoadBuildings : MonoBehaviour {

    [Serializable] //attributes for json
    public class BuildingData
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
    public bool save;
    public bool load;

    private void Awake()
    {
        folder = "SaveData";
        fileName = "CommentData";
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
        SaveData.OnBeforeSaveBuildings += HandleBeforeSave;
        SaveData.OnLoadedBuildings += HandleLoading;
    }



    private void HandleBeforeSave()
    {
        if (photonBuildingsGO)
        {
            foreach (Transform t in photonBuildingsGO.transform)
            {
                StoreData(t);
            }
        }
    }

    private void HandleLoading()
    {
        throw new NotImplementedException();
    }

    public void Save()
    {
        SaveData.SaveBuildings(pathName, SaveData.buildingContainer);
    }
    //add generation script?
    public void Load()
    {
        SaveData.LoadBuildings(pathName);
    }

    private void StoreData(Transform t)
    {
        BuildingData data = new BuildingData();
        data.localPosition = t.localPosition;
        data.localRotation = t.localRotation;
        data.gameObjectName = t.gameObject.name;
        SaveData.AddbuildingData(data);
    }

    internal static void RelocateBuilding(BuildingData data)
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

    public static Comment CreateOldComment(CommentData data)
    {
        Comment comment = CreateComment();
        comment.data = data;
        //comment.LoadData();
        comment.SortAndAddToLocalList();
        return comment;
    }
}
