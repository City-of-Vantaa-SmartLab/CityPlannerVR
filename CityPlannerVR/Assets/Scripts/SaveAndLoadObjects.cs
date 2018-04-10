﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

//Debugausta varten
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// Handles the saving of some data and loading it later
/// 
/// Tutorials used: https://www.youtube.com/watch?v=sWWZZByVvlU
/// (Debugging version) https://www.youtube.com/watch?v=6vl1IYMpwVQ
/// </summary>


//TODO: tehdään vain hostilla/serverillä ja clientit saa valmiina kaikki tiedot

public class SaveAndLoadObjects : MonoBehaviour {

    public static SaveAndLoadObjects saveLoad;

    [HideInInspector]
    public string objectName;
    [HideInInspector]
    public Vector3 objectPosition;
    [HideInInspector]
    public Quaternion objectRotation;
    [HideInInspector]
    public Vector3 objectScale;

    private string folderPathName;
    private string folder;
    private string fileName;
    private string fileExtender;
    private string pathName;
    private char slash = Path.DirectorySeparatorChar;
    //private string xmlPathName;

    DirectoryInfo files;
    FileInfo[] fileInfo;

    private void Awake()
    {
        folder = "SaveData";
        fileName = "ObjectData";
        fileExtender = ".dat";
        folderPathName = Application.persistentDataPath + slash + folder;
        pathName = folderPathName + slash + fileName + fileExtender;

        //xmlPathName = Application.persistentDataPath + "/XmlData.dat";
        if (!Directory.Exists(folderPathName))
        {
            Directory.CreateDirectory(folderPathName);
        }

        //LoadObject();
    }

    private void OnApplicationQuit()
    {
        //SaveObject();
    }

    //This will be the parent of the parent (the table we are going to put all the buildings and other stuff)
    public GameObject table;

    [HideInInspector]
    public ObjectDatabase dataDB;

    #region Xml debug
    //public void XmlSaveObject()
    //{
    //    //Delete the old savefile if there is one
    //    if (File.Exists(xmlPathName))
    //    {
    //        File.Delete(xmlPathName);
    //    }

    //    XmlSerializer serializer = new XmlSerializer(typeof(ObjectDatabase));
    //    FileStream file = File.Create(xmlPathName);

    //    for (int i = 0; i < ObjectContainer.objects.Count; i++)
    //    {

    //        dataDB.list.Add(new ObjectData());
    //        dataDB.list[i].objectName = ObjectContainer.objects[i].name;

    //        //Vector3
    //        dataDB.list[i].objectPosition[0] = ObjectContainer.objects[i].transform.localPosition.x;
    //        dataDB.list[i].objectPosition[1] = ObjectContainer.objects[i].transform.localPosition.y;
    //        dataDB.list[i].objectPosition[2] = ObjectContainer.objects[i].transform.localPosition.z;

    //        //Quaternion
    //        dataDB.list[i].objectRotation[0] = ObjectContainer.objects[i].transform.localRotation.x;
    //        dataDB.list[i].objectRotation[1] = ObjectContainer.objects[i].transform.localRotation.y;
    //        dataDB.list[i].objectRotation[2] = ObjectContainer.objects[i].transform.localRotation.z;
    //        dataDB.list[i].objectRotation[3] = ObjectContainer.objects[i].transform.localRotation.w;

    //        //Vector3
    //        dataDB.list[i].objectScale[0] = ObjectContainer.objects[i].transform.localScale.x;
    //        dataDB.list[i].objectScale[1] = ObjectContainer.objects[i].transform.localScale.y;
    //        dataDB.list[i].objectScale[2] = ObjectContainer.objects[i].transform.localScale.z;
    //    }

    //    serializer.Serialize(file, dataDB);
    //    file.Close();
    //}

    //public void XmlLoadObject()
    //{
    //    if (File.Exists(xmlPathName))
    //    {
    //        XmlSerializer serializer = new XmlSerializer(typeof(ObjectDatabase));
    //        FileStream file = File.Open(xmlPathName, FileMode.Open);
    //        if (file.Length > 0)
    //        {
    //            dataDB = (ObjectDatabase)serializer.Deserialize(file);
    //            file.Close();

    //            GameObject parent = new GameObject
    //            {
    //                name = "Parent"
    //            };
    //            //Hardcoded for now
    //            parent.transform.parent = table.transform;
    //            parent.transform.localPosition = new Vector3(0, 0, 0);
    //            parent.transform.localRotation = Quaternion.identity;
    //            parent.transform.localScale = new Vector3(1, 1, 1);

    //            for (int i = 0; i < dataDB.list.Count; ++i)
    //            {
    //                objectName = dataDB.list[i].objectName;
    //                objectPosition = new Vector3(dataDB.list[i].objectPosition[0], dataDB.list[i].objectPosition[1], dataDB.list[i].objectPosition[2]);
    //                objectRotation = new Quaternion(dataDB.list[i].objectRotation[0], dataDB.list[i].objectRotation[1], dataDB.list[i].objectRotation[2], dataDB.list[i].objectRotation[3]);
    //                objectScale = new Vector3(dataDB.list[i].objectScale[0], dataDB.list[i].objectScale[1], dataDB.list[i].objectScale[2]);

    //                GameObject go = Instantiate(Resources.Load("Prefabs/Buildings/" + objectName, typeof(GameObject))) as GameObject;
    //                go.name = objectName;
    //                go.transform.parent = parent.transform;
    //                go.transform.localPosition = objectPosition;
    //                go.transform.rotation = objectRotation;
    //                go.transform.localScale = objectScale;
    //            }

    //            InitializeObjectDatabase();

    //        }
    //    }
    //}
    #endregion

    #region save and load
    //This is not fully ready yet, but it is a good start
    public void SaveObject()
    {
        //Delete the old savefile if there is one
        if (File.Exists(pathName))
        {
            File.Delete(pathName);
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(pathName);

        for (int i = 0; i < ObjectContainer.objects.Count; i++)
        {
            dataDB.list.Add(new ObjectData());
            dataDB.list[i].objectName = ObjectContainer.objects[i].name;

            //Vector3
            dataDB.list[i].objectPosition[0] = ObjectContainer.objects[i].transform.localPosition.x;
            dataDB.list[i].objectPosition[1] = ObjectContainer.objects[i].transform.localPosition.y;
            dataDB.list[i].objectPosition[2] = ObjectContainer.objects[i].transform.localPosition.z;

            //Quaternion
            dataDB.list[i].objectRotation[0] = ObjectContainer.objects[i].transform.localRotation.x;
            dataDB.list[i].objectRotation[1] = ObjectContainer.objects[i].transform.localRotation.y;
            dataDB.list[i].objectRotation[2] = ObjectContainer.objects[i].transform.localRotation.z;
            dataDB.list[i].objectRotation[3] = ObjectContainer.objects[i].transform.localRotation.w;

            //Vector3
            dataDB.list[i].objectScale[0] = ObjectContainer.objects[i].transform.localScale.x;
            dataDB.list[i].objectScale[1] = ObjectContainer.objects[i].transform.localScale.y;
            dataDB.list[i].objectScale[2] = ObjectContainer.objects[i].transform.localScale.z;
        }

        bf.Serialize(file, dataDB);
        file.Close();
    }

    public void LoadObject()
    {
        files = new DirectoryInfo(folderPathName);
        //Get all the savefiles in the same folder 
        fileInfo = files.GetFiles();

        //Loop through each of the files and load the content
        foreach (var item in fileInfo)
        {
            pathName = Application.persistentDataPath + slash + folder + slash + item.Name;
            if (File.Exists(pathName))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(pathName, FileMode.Open);
                if (file.Length > 0)
                {
                    dataDB = (ObjectDatabase)bf.Deserialize(file);
                    file.Close();

                    GameObject parent = new GameObject
                    {
                        name = "Parent"
                    };
                    //Hardcoded
                    parent.transform.parent = table.transform;
                    parent.transform.localPosition = new Vector3(0, 0, 0);
                    parent.transform.localRotation = Quaternion.identity;
                    parent.transform.localScale = new Vector3(1, 1, 1);

                    for (int i = 0; i < dataDB.list.Count; ++i)
                    {
                        objectName = dataDB.list[i].objectName;
                        objectPosition = new Vector3(dataDB.list[i].objectPosition[0], dataDB.list[i].objectPosition[1], dataDB.list[i].objectPosition[2]);
                        objectRotation = new Quaternion(dataDB.list[i].objectRotation[0], dataDB.list[i].objectRotation[1], dataDB.list[i].objectRotation[2], dataDB.list[i].objectRotation[3]);
                        objectScale = new Vector3(dataDB.list[i].objectScale[0], dataDB.list[i].objectScale[1], dataDB.list[i].objectScale[2]);

                        GameObject go = Instantiate(Resources.Load("Prefabs/Buildings/" + objectName, typeof(GameObject))) as GameObject;
                        go.name = objectName;
                        go.transform.parent = parent.transform;
                        //Gives the parent object to the child so it knows who its parent is when there are multiple parents in the scene
                        go.GetComponent<SnapToGrid>().parent = parent;
                        go.transform.localPosition = objectPosition;
                        go.transform.rotation = objectRotation;
                        go.transform.localScale = objectScale;
                    }

                    //This must be done in order to prevent a bug that duplicates all the buildings 
                    InitializeObjectDatabase();
                }
            }
        }
    }
    #endregion

    void InitializeObjectDatabase()
    {
        while (dataDB.list.Count > 0)
        {
            dataDB.list.Remove(dataDB.list[0]);
        }
    }
}



[System.Serializable]
public class ObjectData
{
    public string objectName;
    public float[] objectPosition = new float[3]; //x, y, z
    public float[] objectRotation = new float[4]; //x, y, z, w
    public float[] objectScale = new float[3];    //x, y, z
}

//Contains all the objects from the scene that are saved
[System.Serializable]
public class ObjectDatabase
{
    public List<ObjectData> list = new List<ObjectData>();
}