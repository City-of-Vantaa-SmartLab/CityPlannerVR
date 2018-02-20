using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


//TODO: tehdään vain hostilla/serverillä ja clientit saa valmiina kaikki tiedot

public class SaveAndLoadObjects : MonoBehaviour {

    public static SaveAndLoadObjects saveLoad;

    public string objectName;
    public Vector3 objectPosition;
    public Quaternion objectRotation;
    public Vector3 objectScale;

    private string pathName;

    private void Awake()
    {
        pathName = Application.persistentDataPath + "/ObjectData.dat";

        LoadObject();
    }

    private void OnApplicationQuit()
    {
        SaveObject();
    }

    public GameObject table;

    public void SaveObject()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(pathName);
        ObjectList ol = new ObjectList();

        for (int i = 0; i < ObjectContainer.objects.Count; i++)
        {
            ObjectData data = new ObjectData();
            data.objectName = objectName;
            data.objectPosition = objectPosition;
            data.objectRotation = objectRotation;
            data.objectScale = objectScale;

            ol.objectDataList.Add(data);
        }

        bf.Serialize(file, ol);
        file.Close();
    }

    public void LoadObject()
    {
        if(File.Exists(pathName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(pathName, FileMode.Open);
            ObjectList ol = (ObjectList)bf.Deserialize(file);
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

            for (int i = 0; i < ol.objectDataList.Count; i++)
            {
                objectName = ol.objectDataList[i].objectName;
                objectPosition = ol.objectDataList[i].objectPosition;
                objectRotation = ol.objectDataList[i].objectRotation;
                objectScale = ol.objectDataList[i].objectScale;

                GameObject go = Instantiate(GameObject.Find(objectName), objectPosition, objectRotation);
                go.transform.parent = parent.transform;
                go.transform.localScale = objectScale;
            }
        }
    }
}

[System.Serializable]
class ObjectList
{
    public List<ObjectData> objectDataList;
}

[System.Serializable]
class ObjectData
{
    public string objectName;
    public Vector3 objectPosition;
    public Quaternion objectRotation;
    public Vector3 objectScale;
}