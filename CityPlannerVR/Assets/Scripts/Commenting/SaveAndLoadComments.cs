using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// The middleman between Comment and SaveData scripts.
/// </summary>

public class SaveAndLoadComments : MonoBehaviour {

    private string folderPathName;
    private string folder;
    private string fileName;
    private string fileExtender;
    private string pathName;
    private char slash = Path.DirectorySeparatorChar;
    //public const string commentPrefabPath = "Prefabs/Marker";
    public bool save;
    public bool load;
    public GameObject depository;


    private void Awake()
    {
        folder = "SaveData";
        fileName = "CommentData";
        fileExtender = ".dat";
        folderPathName = Application.persistentDataPath + slash + folder;
        pathName = folderPathName + slash + fileName + fileExtender;
        if (!depository)
            depository = gameObject;
    }

    public static Comment CreateComment()
    {
        Comment comment = new Comment();

        //GameObject depository;
        //depository = GameObject.Find("GameController");
        //GameObject prefab = Resources.Load<GameObject>(prefabPath);

        //GameObject go = Instantiate(prefab, position, rotation) as GameObject;
        //Comment comment = go.GetComponent<Comment>() ?? go.AddComponent<Comment>(); //will add component if getcomponent returns null, will be changed later
        return comment;
    }

    public static Comment CreateComment(CommentData data)
    {
        Comment comment = CreateComment();
        comment._data = data;
        //comment.LoadData(); //done automatically with event SaveAnd....OnLoaded -> Comment.LoadData
        return comment;
    }

    public void Save()
    {
        SaveData.Save(pathName, SaveData.commentContainer);
    }

    public void Load()
    {
        SaveData.Load(pathName);
    }

    //update will be removed later, when saving is implented in UI
    private void Update()
    {
        if (save)
        {
            Save();
            save = false;
        }

        if (load)
        {
            Load();
            load = false;
        }
    }
}
