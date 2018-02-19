using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveAndLoadObjects : MonoBehaviour {

    public static SaveAndLoadObjects saveLoad;

    public string objectName;
    public Vector3 objectPosition;
    public Vector3 objectRotation;
    public Vector3 objectScale;

    private string pathName = Application.persistentDataPath + "/joku.joku";

    public void SaveObject()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(pathName);

        ObjectData data = new ObjectData();
        data.objectName = objectName;
        data.objectPosition = objectPosition;
        data.objectRotation = objectRotation;
        data.objectScale = objectScale;

        testi test = new testi();
        test.joku[0] = data;

        bf.Serialize(file, data);
        bf.Serialize(file, test);
        file.Close();
    }

    public void Load()
    {
        if(File.Exists(pathName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(pathName, FileMode.Open);
            ObjectData data = (ObjectData)bf.Deserialize(file);
            file.Close();

            objectName = data.objectName;
            objectPosition = data.objectPosition;
            objectRotation = data.objectRotation;
            objectScale = data.objectScale;
        }
    }
}

[System.Serializable]
class testi
{
    public ObjectData[] joku;
}

[System.Serializable]
class ObjectData
{
    public string objectName;
    public Vector3 objectPosition;
    public Vector3 objectRotation;
    public Vector3 objectScale;
}