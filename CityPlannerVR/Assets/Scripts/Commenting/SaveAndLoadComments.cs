using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// The middleman between Comment and SaveData scripts. Manages local lists of comments in the depository.
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
    public GameObject depositoryGO;
    public CommentDepository depository;
    private CommentContainer buffer;

    private void Awake()
    {
        folder = "SaveData";
        fileName = "CommentData";
        fileExtender = ".dat";
        folderPathName = Application.persistentDataPath + slash + folder;
        pathName = folderPathName + slash + fileName + fileExtender;
        if (!depositoryGO)
            depositoryGO = GetDepositoryGO();
        depository = depositoryGO.GetComponent<CommentDepository>() ?? depositoryGO.AddComponent<CommentDepository>();
    }

    private GameObject GetDepositoryGO()
    {
        if (gameObject.name == "Comments")
        {
            depositoryGO = gameObject;
        }
        else
        {
            Transform tempSearch = transform.Find("Comments");
            if (tempSearch)
            {
                depositoryGO = tempSearch.gameObject;
            }
            else
            {
                depositoryGO = new GameObject();
                depositoryGO.transform.parent = this.transform;
                depositoryGO.name = "Comments";
            }
        }
        return depositoryGO;
    }

    public static Comment CreateComment()
    {
        Comment comment = new Comment();

        //GameObject depository;
        //depository = GameObject.Find("GameController");
        //GameObject prefab = Resources.Load<GameObject>(prefabPath);

        //GameObject go = Instantiate(prefab, position, rotation) as GameObject;
        //Comment comment = go.GetComponent<Comment>() ?? go.AddComponent<Comment>(); //will add component if getcomponent returns null
        
        return comment;
    }

    public static Comment CreateComment(CommentData data)
    {
        Comment comment = CreateComment();
        comment._data = data;
        comment.SortAndAddToList();
        //comment.LoadData(); //done automatically with event SaveAnd....OnLoaded -> Comment.LoadData
        return comment;
    }

    //private void AddToDepo(Comment comment)
    //{
    //    switch (comment._data.type)
    //    {
    //        case Comment.CommentType.Text:
    //            depository.texts.Add(comment);
    //            break;

    //        case Comment.CommentType.Voice:
    //            depository.voices.Add(comment);
    //            break;

    //        case Comment.CommentType.Thumb:
    //            depository.thumbs.Add(comment);
    //            break;

    //        default:
    //            Debug.Log("Type not set for comment " + comment.name + " by user " + comment._userName);
    //            break;
    //    }
    //    depository.texts.Add(comment);
    //}

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
