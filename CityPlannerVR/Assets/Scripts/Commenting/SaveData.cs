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
public class Container<T>
{
    public List<T> datas = new List<T>();
}

public class CommentContainer
{
    public List<CommentData> commentDatas = new List<CommentData>(); 
}

public class BuildingContainer
{
    public List<SaveAndLoadBuildings.BuildingData> buildingDatas = new List<SaveAndLoadBuildings.BuildingData>();
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
    public static BuildingContainer buildingContainer = new BuildingContainer();

    public delegate void SerializeAction();
    public static event SerializeAction OnLoadedComments;
    public static event SerializeAction OnBeforeSaveComments;
    public static event SerializeAction OnLoadedBuildings;
    public static event SerializeAction OnBeforeSaveBuildings;

    public static void LoadComments(string filepath)
    {
        commentContainer = LoadCommentDatas(filepath);

        foreach (CommentData data in commentContainer.commentDatas)
        {
            SaveAndLoadComments.CreateOldComment(data);
        }

        if (OnLoadedComments != null)
            OnLoadedComments();
        ClearCommentContainerList();
    }

    public static void SaveComments(string filepath, CommentContainer commentDatas)
    {
        if (OnBeforeSaveComments != null)
            OnBeforeSaveComments();
        SaveCommentDatas(filepath, commentDatas);
        ClearCommentContainerList();
    }

    public static void LoadBuildings(string filepath)
    {
        buildingContainer = LoadBuildinDatas(filepath);

        foreach (var data in buildingContainer.buildingDatas)
        {
            SaveAndLoadBuildings.RelocateBuilding(data);
        }

        if (OnLoadedBuildings != null)
            OnLoadedBuildings();
        ClearBuildingContainerList();
    }

    public static void SaveBuildings(string filepath, BuildingContainer buildingDatas)
    {
        if (OnBeforeSaveBuildings != null)
            OnBeforeSaveBuildings();
        SaveBuildingDatas(filepath, buildingDatas);
        ClearCommentContainerList();
    }

    #region ListManipulations
    //Copypaste because of static methods

    public static void AddCommentData(CommentData data)
    {
        commentContainer.commentDatas.Add(data);
        Debug.Log("Comment added to containerlist");
    }

    public static void ClearCommentContainerList()
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

    public static void AddbuildingData(SaveAndLoadBuildings.BuildingData data)
    {
        buildingContainer.buildingDatas.Add(data);
        Debug.Log("Building added to containerlist");
    }

    public static void ClearBuildingContainerList()
    {
        buildingContainer.buildingDatas.Clear();
        Debug.Log("BuildingContainer cleared");
    }


    #endregion

    #region LoadSaveDatas

    private static CommentContainer LoadCommentDatas(string filepath)
    {
        string jason = File.ReadAllText(filepath);
        return JsonUtility.FromJson<CommentContainer>(jason);
    }

    private static void SaveCommentDatas(string filepath, CommentContainer comments)
    {
        string jason = JsonUtility.ToJson(comments);
        StreamWriter sw = File.CreateText(filepath);  //creates or overwrites file at filepath
        sw.Close();
        File.WriteAllText(filepath, jason);
    }

    private static BuildingContainer LoadBuildinDatas(string filepath)
    {
        string jason = File.ReadAllText(filepath);
        return JsonUtility.FromJson<BuildingContainer>(jason);
    }

    private static void SaveBuildingDatas(string filepath, BuildingContainer buildings)
    {
        string jason = JsonUtility.ToJson(buildings);
        StreamWriter sw = File.CreateText(filepath);  //creates or overwrites file at filepath
        sw.Close();
        File.WriteAllText(filepath, jason);
    }

    #endregion

}