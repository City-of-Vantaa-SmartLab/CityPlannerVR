using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

//Debugausta varten
using System.Xml;
using System.Xml.Serialization;


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

    private string pathName;
    private string xmlPathName;

    private void Awake()
    {
        pathName = Application.persistentDataPath + "/ObjectData.dat";
        xmlPathName = Application.persistentDataPath + "/XmlData.dat";

        XmlLoadObject();
    }

    private void OnApplicationQuit()
    {
        XmlSaveObject();
    }

    //This will be the parent of the parent (the table we are going to put all the buildings and other stuff)
    public GameObject table;

    [HideInInspector]
    public ObjectDatabase dataDB;

    #region Xml debug
    public void XmlSaveObject()
    {
        //Delete the old savefile if there is one
        if (File.Exists(xmlPathName))
        {
            File.Delete(xmlPathName);
        }

        XmlSerializer serializer = new XmlSerializer(typeof(ObjectDatabase));
        FileStream file = File.Create(xmlPathName);

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

        serializer.Serialize(file, dataDB);
        file.Close();
    }

    public void XmlLoadObject()
    {
        if (File.Exists(xmlPathName))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ObjectDatabase));
            FileStream file = File.Open(xmlPathName, FileMode.Open);
            if (file.Length > 0)
            {
                dataDB = (ObjectDatabase)serializer.Deserialize(file);
                file.Close();

                GameObject parent = new GameObject
                {
                    name = "Parent"
                };
                //Hardcoded for now
                parent.transform.parent = table.transform;
                parent.transform.localPosition = new Vector3(0, 0, 0);
                parent.transform.localRotation = Quaternion.identity;
                parent.transform.localScale = new Vector3(1, 1, 1);

                //All the lists are (or should be) the same lenght
                for (int i = 0; i < dataDB.list.Count; i++)
                {
                    objectName = dataDB.list[i].objectName;
                    objectPosition = new Vector3(dataDB.list[i].objectPosition[0], dataDB.list[i].objectPosition[1], dataDB.list[i].objectPosition[2]);
                    objectRotation = new Quaternion(dataDB.list[i].objectRotation[0], dataDB.list[i].objectRotation[1], dataDB.list[i].objectRotation[2], dataDB.list[i].objectRotation[3]);
                    objectScale = new Vector3(dataDB.list[i].objectScale[0], dataDB.list[i].objectScale[1], dataDB.list[i].objectScale[2]);

                    dataDB.list.Remove(dataDB.list[i]);

                    GameObject go = Instantiate(Resources.Load("Prefabs/Buildings/" + objectName, typeof(GameObject)), objectPosition, objectRotation) as GameObject;
                    go.name = objectName;
                    go.transform.parent = parent.transform;
                    go.transform.localScale = objectScale;
                }
            }
        }
    }
    #endregion

    #region save and load
    public void SaveObject()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(pathName);
        //ObjectDatabase dataDB = new ObjectDatabase();

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
                //Hardcoded for now
                parent.transform.parent = table.transform;
                parent.transform.localPosition = new Vector3(0, 0, 0);
                parent.transform.localRotation = Quaternion.identity;
                parent.transform.localScale = new Vector3(1, 1, 1);

                //All the lists are (or should be) the same lenght
                for (int i = 0; i < dataDB.list.Count; i++)
                {
                    objectName = dataDB.list[i].objectName;
                    objectPosition = new Vector3(dataDB.list[i].objectPosition[0], dataDB.list[i].objectPosition[1], dataDB.list[i].objectPosition[2]);
                    objectRotation = new Quaternion(dataDB.list[i].objectRotation[0], dataDB.list[i].objectRotation[1], dataDB.list[i].objectRotation[2], dataDB.list[i].objectRotation[3]);
                    objectScale = new Vector3(dataDB.list[i].objectScale[0], dataDB.list[i].objectScale[1], dataDB.list[i].objectScale[2]);

                    dataDB.list.Remove(dataDB.list[i]);

                    GameObject go = Instantiate(Resources.Load("Prefabs/Buildings/" + objectName, typeof(GameObject))) as GameObject;
                    go.transform.position = objectPosition;
                    go.transform.rotation = objectRotation;
                    go.transform.localScale = objectScale;
                    //go.transform.parent = parent.transform;
                }
            }
        }
    }
#endregion
}



[System.Serializable]
public class ObjectData
{
    public string objectName;
    public float[] objectPosition = new float[3];
    public float[] objectRotation = new float[4];
    public float[] objectScale = new float[3];
}

[System.Serializable]
public class ObjectDatabase
{
    public List<ObjectData> list = new List<ObjectData>();
}