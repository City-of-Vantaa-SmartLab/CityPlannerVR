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

public class Container<T> /*where T : parentClass  //if needed, eg. CommentData as child class -> through inheritance?*/
{
    public List<T> datas = new List<T>();
    public Transform previousHolder = null; //used when loading transformdata
}

//for easy accessing and storing locally

public class SaveData {

    public class CommentLists
    {
        public List<Comment> textComments = new List<Comment>();
        public List<Comment> voiceComments = new List<Comment>();
        public List<Comment> thumbComments = new List<Comment>();
    }

    //public static CommentContainer commentContainer = new CommentContainer();
    public static Container<CommentData> commentContainer = new Container<CommentData>();
    public static Container<TransformData> transformContainer = new Container<TransformData>();
    public static CommentLists commentLists = new CommentLists();

    public delegate void SerializeAction();
    public static event SerializeAction OnLoadedComments;
    public static event SerializeAction OnBeforeSaveComments;
    public static event SerializeAction OnLoadedTransforms;
    public static event SerializeAction OnBeforeSaveTransforms;

    public static void LoadItems<T>(string filepath)
    {
        Container<T> tempContainer;
        tempContainer = LoadDatas<T>(filepath);
        foreach (T data in tempContainer.datas)
        {
            if (data is CommentData)
                SaveAndLoadComments.CreateOldComment(data as CommentData);
            if (data is TransformData)
                SaveAndLoadTransforms.RestoreTransform(data as TransformData, tempContainer.previousHolder);
        }
        if (tempContainer.datas.Count != 0 && tempContainer.datas[0] is CommentData)
        {
            if (OnLoadedComments != null)
                OnLoadedComments();
        }

        if (tempContainer.datas.Count != 0 && tempContainer.datas[0] is TransformData)
        {
            if (OnLoadedTransforms != null)
                OnLoadedTransforms();
        }
    }

    internal static void SaveDatas(string filepath, object container)
    {
        string jason = JsonUtility.ToJson(container);
        StreamWriter sw = File.CreateText(filepath);  //creates or overwrites file at filepath
        sw.Close();
        File.WriteAllText(filepath, jason);
    }

    public static Container<T> LoadDatas<T>(string filepath)
    {
        string jason = File.ReadAllText(filepath);
        return JsonUtility.FromJson<Container<T>>(jason);
    }

    public static void AddData<T> (T data)
    {
        if (data is CommentData)
            commentContainer.datas.Add(data as CommentData);
        if (data is TransformData)
            transformContainer.datas.Add(data as TransformData);
    }

    public static void ClearContainer<T>(Container<T> container)
    {
        container.datas.Clear();
        container.previousHolder = null;
        Debug.Log("Container cleared");
    }

    public static void ClearCommentLists()
    {
        commentLists.textComments.Clear();
        commentLists.voiceComments.Clear();
        commentLists.thumbComments.Clear();
        Debug.Log("CommentLists cleared");
    }

}