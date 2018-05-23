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
    public bool save;
    public bool load;

    private void Awake()
    {
        folder = "SaveData";
        fileName = "CommentData";
        fileExtender = ".dat";
        folderPathName = Application.persistentDataPath + slash + folder;
        pathName = folderPathName + slash + fileName + fileExtender;
    }

    public static Comment CreateComment()
    {
        Comment comment = new Comment();
        return comment;
    }

    public static Comment CreateOldComment(CommentData data)
    {
        Comment comment = CreateComment();
        comment._data = data;
        comment.LoadData();
        comment.SortAndAddToLocalList();
        return comment;
    }

    //Use this when the user creates an entirely new comment
    public Comment CreateNewComment(CommentData data)
    {
        Comment comment = CreateComment();
        comment._data = data;
        comment.LoadData();
        comment.SortAndAddToLocalList();
        comment.ApplyDataToContainer();  //this will add the comment to savedata's commentcontainer
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
