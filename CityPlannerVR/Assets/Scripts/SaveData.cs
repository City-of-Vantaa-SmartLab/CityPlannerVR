using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// Handles the writing and reading of files, as well as stores the commentcontainer.
/// Could be scaled for other types of data as well (later).
/// </summary>

public class CommentContainer
{
    public List<CommentData> commentDatas = new List<CommentData>();
}

public class SaveData {

    public static CommentContainer commentContainer = new CommentContainer();

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
        ClearCommentList();
    }

    public static void Save(string filepath, CommentContainer commentDatas)
    {
        if (OnBeforeSave != null)
            OnBeforeSave();
        SaveComments(filepath, commentDatas);
        ClearCommentList();
    }


    public static void AddCommentData(CommentData data)
    {
        commentContainer.commentDatas.Add(data);
        Debug.Log("Comment added to list");
    }

    public static void ClearCommentList()
    {
        commentContainer.commentDatas.Clear();
        Debug.Log("Temp comments cleared");
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