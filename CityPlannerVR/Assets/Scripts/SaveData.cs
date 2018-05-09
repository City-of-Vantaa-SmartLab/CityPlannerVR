using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveData {

    public static CommentContainer commentContainer = new CommentContainer();

    public delegate void SerializeAction();
    public static event SerializeAction OnLoaded;
    public static event SerializeAction OnBeforeSave;

    public static void Load(string filepath)
    {
        commentContainer = LoadComments(filepath); 
    }

    public static void Save(string filepath, CommentContainer comments)
    {
        OnBeforeSave();
        SaveComments(filepath, comments);
        ClearCommentList();
    }


    public static void AddCommentData(CommentData data)
    {
        commentContainer.comments.Add(data);
    }

    public static void ClearCommentList()
    {
        commentContainer.comments.Clear();
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


public class CommentContainer
{
    public List<CommentData> comments = new List<CommentData>();
}