using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;

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

        //Load();
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
        comment.SortAndAddToLocalList();
        if (comment.data.commentType == Comment.CommentType.Voice)
            RecordComment.GenerateComment(data.userName, data.commentName, data.positions, data.commentedObjectName);
        return comment;
    }


    //Use eg. laserbutton to call the methods below
    public void SaveAsync()
    {
        Task saveTask = new Task(() => SaveToDatabase());
        saveTask.Start();
        //SaveToDatabase();
    }

    public void LoadAsync()
    {
        Task loadTask = new Task(() => LoadFromExternalSource(true));
        loadTask.Start();

        //taskA.Wait();

        //StartCoroutine(LoadFromExternalSource(true));

        //LoadFromExternalSource(true);
    }

    #region Database accessing

    public void SaveToDatabase()
    {
        pathName = folderPathName + slash + fileName + fileExtender;
        SaveData.SaveDatas(pathName, SaveData.commentContainer);
        //SaveData.ClearContainer(SaveData.commentContainer);
        Debug.Log("Starting to save...");
        //pathName = folderPathName + slash + fileName + fileExtender;
        MongoDBAPI.UseDefaultConnections();
        MongoDBAPI.ImportJSONFileToDatabase(MongoDBAPI.commentCollection, pathName);
        Debug.Log("Saving done!");
    }

    private void LoadFromExternalSource(bool useDatase)
    {
        //int countdown = 5;
        //while (countdown > 0)
        //{
        //    Debug.Log("Starting to load in " + countdown + " seconds...");
        //    countdown--;
        //    yield return new WaitForSeconds(1f);
        //}
        pathName = folderPathName + slash + fileName + fileExtender;
        if (useDatase)
        {
            Debug.Log("Starting to load...");
            MongoDBAPI.UseDefaultConnections();
            MongoDBAPI.ExportJSONFileFromDatabase<CommentData>(MongoDBAPI.commentCollection, pathName);
            //MongoDBAPI.ExportContainersFromDatabase<CommentData>(MongoDBAPI.commentCollection);

            //SaveData.LoadItems<CommentData>(pathName);
            Debug.Log("Loading done!");
        }
        else
            SaveData.LoadItems<CommentData>(pathName);



        //yield return null;
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
            //testComment.SortAndAddToLocalList();
        }
    }


}
