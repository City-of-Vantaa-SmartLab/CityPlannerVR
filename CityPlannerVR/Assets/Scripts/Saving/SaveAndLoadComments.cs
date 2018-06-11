using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// The middleman between Comment and SaveData scripts. Manages local lists of comments in the depository.
/// </summary>

public class SaveAndLoadComments : MonoBehaviour {

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
        fileName = "CommentData";
        fileExtender = ".dat";
        folderPathName = Application.persistentDataPath + slash + folder;
        pathName = folderPathName + slash + fileName + fileExtender;

        Load();
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

    //Use this when the user creates an entirely new comment
    public Comment CreateNewComment(CommentData data)
    {
        Comment comment = CreateComment();
        data.submittedShortDate = System.DateTime.Now.ToShortDateString();
        data.submittedShortTime = System.DateTime.Now.ToShortTimeString();
        comment.data = data;
        comment.GenerateQuickCheck(3);
        //comment.LoadData();
        comment.SortAndAddToLocalList();
        //comment.ApplyDataToContainer();  //this will add the comment to savedata's commentcontainer  //should be done via sub in Comment script to savedata
        return comment;
    }

    //Use eg. laserbutton to call the methods below
    public void Save()
    {
        SaveData.SaveDatas(pathName, SaveData.commentContainer);
    }

    public void Load()
    {
        SaveData.LoadItems<CommentData>(pathName);
    }

}
