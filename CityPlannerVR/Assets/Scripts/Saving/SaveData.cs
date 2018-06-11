using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

/// <summary>
/// Handles the writing and reading of files, as well as stores the commentcontainer.
/// Uses static methods, so that the script does not need to be attached to anything to work.
/// Could be scaled for other types of data as well (later). 
/// </summary>

//reference: https://msdn.microsoft.com/en-us/library/ms379564(v=vs.80).aspx
//left off at Generics and casting

//for networking
public class Container<T> /*where T : class //such as CommentData class*/
{
    public List<T> datas = new List<T>();


}

//public class CommentContainer
//{
//    public List<CommentData> commentDatas = new List<CommentData>(); 
//}

//for easy accessing and storing locally
public class CommentLists
{
    public List<Comment> textComments = new List<Comment>(); 
    public List<Comment> voiceComments = new List<Comment>();
    public List<Comment> thumbComments = new List<Comment>();
}

public class SaveData <T> {

    //public static CommentContainer commentContainer = new CommentContainer();
    public static Container<CommentData> commentContainer = new Container<CommentData>();
    public static Container<SaveAndLoadTransforms.TransformData> transformContainer = new Container<SaveAndLoadTransforms.TransformData>();
    public static CommentLists commentLists = new CommentLists();

    public delegate void SerializeAction();
    public static event SerializeAction OnLoadedComments;
    public static event SerializeAction OnBeforeSaveComments;
    public static event SerializeAction OnLoadedTransforms;
    public static event SerializeAction OnBeforeSaveTransforms;

    //public static void LoadComments(string filepath)
    //{
    //    commentContainer = LoadCommentDatas(filepath);

    //    foreach (CommentData data in commentContainer.commentDatas)
    //    {
    //        SaveAndLoadComments.CreateOldComment(data);
    //    }

    //    if (OnLoadedComments != null)
    //        OnLoadedComments();
    //    ClearCommentContainerList();
    //}

    //public static void SaveComments(string filepath, CommentContainer commentDatas)
    //{
    //    if (OnBeforeSaveComments != null)
    //        OnBeforeSaveComments();
    //    SaveCommentDatas(filepath, commentDatas);
    //    ClearCommentContainerList();
    //}

    public static void LoadItems(string filepath)
    {
        Container<T> tempContainer;
        tempContainer = LoadDatas(filepath);
        foreach (T data in tempContainer.datas)
        {
            if (data is CommentData)
                SaveAndLoadComments.CreateOldComment(data as CommentData);
            if (data is SaveAndLoadTransforms.TransformData)
                SaveAndLoadTransforms.MoveOldTransform(data as SaveAndLoadTransforms.TransformData);

        }
    }

    internal static void SaveItems(string filepath, object container)
    {
        string jason = JsonUtility.ToJson(container);
        StreamWriter sw = File.CreateText(filepath);  //creates or overwrites file at filepath
        sw.Close();
        File.WriteAllText(filepath, jason);
    }

    private static Container<T> LoadDatas(string filepath)
    {
        string jason = File.ReadAllText(filepath);
        return JsonUtility.FromJson<Container<T>>(jason);
    }


    //public static void AddCommentData(CommentData data)
    //{
    //    commentContainer.commentDatas.Add(data);
    //    Debug.Log("Comment added to containerlist");
    //}

    public static void AddData(T data)
    {
        if (data is CommentData)
            commentContainer.datas.Add(data as CommentData);
        if (data is SaveAndLoadTransforms.TransformData)
            transformContainer.datas.Add(data as SaveAndLoadTransforms.TransformData);
    }

    public static void ClearContainerList(Container<T> container)
    {
        container.datas.Clear();
        Debug.Log("CommentContainer cleared");
    }

    public static void ClearCommentLists()
    {
        commentLists.textComments.Clear();
        commentLists.voiceComments.Clear();
        commentLists.thumbComments.Clear();
        Debug.Log("CommentLists cleared");
    }


    //private static CommentContainer LoadCommentDatas(string filepath)
    //{
    //    string jason = File.ReadAllText(filepath);
    //    return JsonUtility.FromJson<CommentContainer>(jason);
    //}

    //private static void SaveCommentDatas(string filepath, CommentContainer comments)
    //{
    //    string jason = JsonUtility.ToJson(comments);
    //    StreamWriter sw = File.CreateText(filepath);  //creates or overwrites file at filepath
    //    sw.Close();
    //    File.WriteAllText(filepath, jason);
    //}

}