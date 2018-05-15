using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// Handles the writing and reading of files, as well as stores the commentcontainer.
/// Uses static methods, so that the script does not need to be attached to anything to work.
/// Could be scaled for other types of data as well (later). 
/// </summary>

//for networking
public class CommentContainer
{
    public List<CommentData> commentDatas = new List<CommentData>(); 
}

//for easy accessing and storing locally
public class CommentLists
{
    public List<Comment> textComments = new List<Comment>(); 
    public List<Comment> voiceComments = new List<Comment>();
    public List<Comment> thumbComments = new List<Comment>();
}

public class SaveData {

    public static CommentContainer commentContainer = new CommentContainer();
    public static CommentLists commentLists = new CommentLists();

    public delegate void SerializeAction();
    public static event SerializeAction OnLoaded;
    public static event SerializeAction OnBeforeSave;

    public static void Load(string filepath)
    {
        commentContainer = LoadComments(filepath);

        foreach (CommentData data in commentContainer.commentDatas)
        {
            SaveAndLoadComments.CreateComment(data);
        }

        if (OnLoaded != null)
            OnLoaded();
        ClearContainerList();
    }

    public static void Save(string filepath, CommentContainer commentDatas)
    {
        if (OnBeforeSave != null)
            OnBeforeSave();
        SaveComments(filepath, commentDatas);
        ClearContainerList();
    }


    public static void AddCommentData(CommentData data)
    {
        commentContainer.commentDatas.Add(data);
        Debug.Log("Comment added to containerlist");
    }

    public static void ClearContainerList()
    {
        commentContainer.commentDatas.Clear();
        Debug.Log("CommentContainer cleared");
    }

    public static void ClearCommentLists()
    {
        commentLists.textComments.Clear();
        commentLists.voiceComments.Clear();
        commentLists.thumbComments.Clear();
        Debug.Log("CommentLists cleared");
    }

    private static CommentContainer LoadComments(string filepath)
    {
        string jason = File.ReadAllText(filepath);

        return JsonUtility.FromJson<CommentContainer>(jason);
    }

    private static void SaveComments(string filepath, CommentContainer comments)
    {
        string jason = JsonUtility.ToJson(comments);
        StreamWriter sw = File.CreateText(filepath);  //creates or overwrites file at filepath
        sw.Close();
        File.WriteAllText(filepath, jason);
    }

}