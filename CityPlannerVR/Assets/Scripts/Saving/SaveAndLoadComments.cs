using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// The middleman between Comment and SaveData scripts. Manages local lists of comments in the depository.
/// </summary>

public class SaveAndLoadComments : MonoBehaviour
{
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
        //SaveData.ClearContainer(SaveData.commentContainer);
        SaveToDatabase();
    }

    public void Load()
    {
        LoadFromDatabase();
        //Container<CommentData> tempContainer;
        //tempContainer = SaveData.LoadDatas<CommentData>(pathName);
        SaveData.LoadItems<CommentData>(pathName);
    }

    #region Database accessing

    public void SaveToDatabase()
    {
        //pathName = folderPathName + slash + fileName + fileExtender;
        MongoDBAPI.UseDefaultConnections();
        MongoDBAPI.ImportJSONFileToDatabase(MongoDBAPI.commentCollection, pathName);
    }

    private void LoadFromDatabase()
    {
        //pathName = folderPathName + slash + fileName + fileExtender;
        MongoDBAPI.UseDefaultConnections();
        MongoDBAPI.ExportJSONFileFromDatabase(MongoDBAPI.commentCollection, pathName);
    }

    private void CountDatabaseCommentContainers()
    {
        if (MongoDBAPI.commentCollection == null)
            MongoDBAPI.UseDefaultConnections();
        var count = MongoDBAPI.commentCollection.CountDocuments(new MongoDB.Bson.BsonDocument());
    }

    public void LoadCommentsForVizualisation(string userName, bool descendingByDate)
    {
        var filter = MongoDB.Driver.Builders<MongoDB.Bson.BsonDocument>.Filter.Exists("i");
        var sort = MongoDB.Driver.Builders<MongoDB.Bson.BsonDocument>.Sort.Descending("i");

    }


    #endregion

    public void GenerateTestComments()
    {
        for (int i = 0; i < 5; i++)
        {
            Comment testComment = Comment.GenerateTestComment();
            testComment.data.dataString += i;
            testComment.SortAndAddToLocalList();
        }
    }


}
