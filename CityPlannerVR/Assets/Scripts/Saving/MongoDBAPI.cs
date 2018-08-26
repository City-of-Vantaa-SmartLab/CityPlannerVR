using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class MongoDBAPI {

    public class CollectionData
    {
        public string ip;
        public string databaseName;
        public string collectionName;
    }


    //[Serializable]
    //private class BinaryDocument
    //{
    //    public string userName;
    //    public string filepath;
    //    public string submittedShortDate;
    //    public BsonBinaryData bsonBinaryData;
    //}

    //private static string defaultConnectionString = "mongodb://192.168.100.21:27017";

    #region variables

    public static MongoClient client;
    public static IMongoDatabase activeDatabase;
    //public static IMongoCollection<Comment> commentCollection;
    //public static IMongoCollection<Container<TransformData>> transformCollection;
    public static IMongoCollection<BsonDocument> transformCollection;
    public static IMongoCollection<BsonDocument> commentCollection;
    public static IMongoCollection<BsonDocument> imageCollection;
    public static IMongoCollection<BsonDocument> voiceCollection;
    public static IMongoCollection<BsonDocument> defaultFileCollection;
    private static string defaultDB = "tikkuraitti";
    private static string defaultUser = "buser";
    private static string defaultPwd = "1234";
    private static MongoCredential defaultDBCredentials =
        MongoCredential.CreateCredential(defaultDB, defaultUser, defaultPwd);
    private static string commentColName = "comments";
    private static string transformColName = "transforms";
    private static string imageColName = "images";
    private static string voiceColName = "voicefiles";
    private static string defaultColName = "otherfiles";
    private static char slash = Path.DirectorySeparatorChar;

    public static readonly string defaultFileFolder = Application.persistentDataPath + slash + "OtherFiles";
    public static readonly string imageFileFolder = Application.streamingAssetsPath + slash + "Screenshots";
    public static readonly string voiceFileFolder = Application.streamingAssetsPath + slash + "Comments" + slash + "VoiceComments" + slash + "AudioFiles";



    //private static CollectionData commentColSet = new CollectionData
    //{
    //    collectionName = "comments",
    //    databaseName = "tikkuraitti",
    //    ip = null
    //};
    //private static CollectionData transformColSet = new CollectionData
    //{
    //    collectionName = "transforms",
    //    databaseName = "tikkuraitti",
    //    ip = null
    //};

    //private static SslSettings defaultSSL;
    //private static MongoClientSettings defaultSettings = new MongoClientSettings
    //{
    //    Server = new MongoServerAddress("192.168.100.21", 27017),
    //    ServerSelectionTimeout = TimeSpan.FromSeconds(3),
    //    Credential = defaultDBCredentials
    //};

    //new default settings for port forwarded setup
    private static MongoClientSettings defaultSettings = new MongoClientSettings
    {
        Server = new MongoServerAddress("127.0.0.1", 27018),
        ServerSelectionTimeout = TimeSpan.FromSeconds(3)
        //Credential = defaultDBCredentials
    };

    #endregion

    /// <summary>
    /// Connects to client and collections, after which you can use transformCollection and
    /// commentCollection to access data with methods export/import JSONfile to/from Database
    /// </summary>

    public static bool UseDefaultConnections()
    {
        ConnectToClient(defaultSettings);
        if (client != null)
        {
            activeDatabase = client.GetDatabase(defaultDB);
            if (activeDatabase != null)
            {
                ConnectToDefaultCollections();
                return true;
            }
            else
            {
                Debug.Log("Found database server, but could not connect to database!");
                return false;
            }
        }
        else
        {
            Debug.Log("Could not find server, check SSH tunnel!");
            return false;
        }
    }

    public static void ConnectToClient(MongoClientSettings settings)
    {
        client = new MongoClient(settings);
    }

    public static void ConnectToDefaultCollections()
    {
        transformCollection = activeDatabase.GetCollection<BsonDocument>(transformColName);
        commentCollection = activeDatabase.GetCollection<BsonDocument>(commentColName);
        imageCollection = activeDatabase.GetCollection<BsonDocument>(imageColName);
        voiceCollection = activeDatabase.GetCollection<BsonDocument>(voiceColName);
        defaultFileCollection = activeDatabase.GetCollection<BsonDocument>(defaultColName);

    }

    public static IMongoCollection<T> ConnectToDatabaseCollection<T>(string collectionName)
    {
        IMongoCollection<T> coll = activeDatabase.GetCollection<T>(collectionName);
        return coll;
    }

    public static IMongoCollection<T> ConnectToDatabaseCollection<T>(string databaseName, string collectionName)
    {
        var db = client.GetDatabase(databaseName);
        IMongoCollection<T> coll = db.GetCollection<T>(collectionName);
        return coll;
    }

    public static bool TestConnections()
    {
        //Debug.Log("Connecting to database using default settings...");
        //UseDefaultConnections();
        if (client != null)
        {
            if (activeDatabase != null)
            {
                if (transformCollection != null && commentCollection != null)
                {
                    Debug.Log("Seems to be working!");
                    return true;
                }
                else
                    Debug.Log("At least one collection is null!");
            }
            else
                Debug.Log("Active database is null!");
        }
        else
            Debug.Log("MongoClient is null!");

        return false;
    }

    //public static void TestMethod1(string filepath, string filenameExtended)
    //{
    //    Debug.Log("Tallennetaan " + filepath);
    //    ImportBinaryFileToDatabase(imageCollection, filepath, filenameExtended);
    //}

    //public static void TestMethod2(string filepath)
    //{
    //    Debug.Log("Ladataan " + filepath);
    //    ExportBinaryFileFromDatabase(imageCollection, filepath);
    //}

    public static void ImportJSONFileToDatabase(IMongoCollection<BsonDocument> targetCollection, string filepath)
    {
        using (var streamReader = new StreamReader(filepath))
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                using (var jsonReader = new JsonReader(line))
                {
                    var context = BsonDeserializationContext.CreateRoot(jsonReader);
                    var document = targetCollection.DocumentSerializer.Deserialize(context);
                    targetCollection.InsertOne(document);
                }
            }
        }
    }

    public static bool ImportBinaryFileToDatabase(
        IMongoCollection<BsonDocument> targetCollection,FileInfoContainer fileInfo)
    {
        if (targetCollection == null)
        {
            Debug.Log("Target collection is null!");
            return false;
        }

        if (fileInfo == null)
        {
            Debug.Log("Fileinfo is null!");
            return false;
        }

        string filePathToUse;
        StreamReader streamReader;

        if (fileInfo.useFullPath)
            filePathToUse = fileInfo.fullFilePath;
        else if (fileInfo.fileType == SyncFiles.Filetype.image)
            filePathToUse = imageFileFolder + slash + fileInfo.filename;
        else if (fileInfo.fileType == SyncFiles.Filetype.voice)
            filePathToUse = voiceFileFolder + slash + fileInfo.filename;
        else
            filePathToUse = defaultFileFolder + slash + fileInfo.filename;

        if (string.IsNullOrEmpty(filePathToUse))
        {
            Debug.Log("The filepath is null or empty!");
            return false;
        }

        using (streamReader = new StreamReader(filePathToUse))

        {
            var bytes = File.ReadAllBytes(filePathToUse);
            BsonBinaryData binaryData = new BsonBinaryData(bytes);
            List<BsonElement> elementList = new List<BsonElement>();
            BsonElement elementByte = new BsonElement("bytes", binaryData);
            elementList.Add(elementByte);

            //if (string.IsNullOrEmpty(fileInfo.filename))
            //{
            //    Debug.Log("Filename is missing!");
            //    fileInfo.filename = "N/A";
            //}
            BsonElement elementFilename = new BsonElement("filename", BsonValue.Create(fileInfo.filename) ?? "N/A");
            elementList.Add(elementFilename);

            BsonElement elementFileType = new BsonElement("filetype", BsonValue.Create(fileInfo.fileType) ?? "N/A");
            elementList.Add(elementFileType);

            BsonElement elementUserWhoSaved = new BsonElement("user", BsonValue.Create(PhotonNetwork.player.NickName ?? "N/A" ));
            elementList.Add(elementUserWhoSaved);  //Only needed for database debugging

            BsonDocument document = new BsonDocument(elementList);
            targetCollection.InsertOne(document);
        }
        return true;
    }

    public static void ExportBinaryFileFromDatabase(IMongoCollection<BsonDocument> targetCollection, string folderPathName)
    {
        ExportBinaryFileFromDatabase(null, null, null, targetCollection, folderPathName, false, 20);
    }

    //Clean obsolete comments away later!
    public static void ExportBinaryFileFromDatabase(
        FilterDefinition<BsonDocument> filter,
        SortDefinition<BsonDocument> sort,                  
        ProjectionDefinition<BsonDocument> projection,      //Use include and exclude to target specific fields in the BsonDocument
        IMongoCollection<BsonDocument> targetCollection,    //The collection from where the method pulls BsonDocuments
        string folderPathName,                              //Saves files from the collection to this folder, unless useSavedFolderPath is true and BsonDocument has a string named foldername
        bool overWriteFiles,                                //Overwrites the same named files
        int limit)                                          //Limits the number of documents pulled from database
    {
        if (filter == null)
            filter = new BsonDocument();
        if (sort == null)
            sort = Builders<BsonDocument>.Sort.Descending("_id");
        if (projection == null)
            projection = Builders<BsonDocument>.Projection.Exclude("_id");

        var mongoCursor = targetCollection.Find(filter).Project(projection).Sort(sort).Limit(limit).ToCursor();  //With or without cursor, still missing something
        var document = mongoCursor.ToBsonDocument();  //This is the likely culprit

        if (document == null)
        {
            Debug.Log("Document is null!");
            return;
        }

        foreach (var doc in mongoCursor.ToEnumerable())
        {
            byte[] urpuus = { 0 };
            BsonBinaryData elementBytes = new BsonBinaryData(urpuus);
            string filename = "";
            string foldername = "";
            SyncFiles.Filetype filetype = SyncFiles.Filetype.defaultType;

            try
            {
                foreach (var element in doc.Elements)
                {
                    if (string.IsNullOrEmpty(element.Name))
                        Debug.Log("Empty or null field name!");
                    else
                    {
                        switch (element.Name)
                        {
                            case "bytes":
                                elementBytes = element.Value.AsBsonBinaryData;
                                break;

                            case "filename":
                                filename = element.Value.AsString;
                                break;
                            case "foldername":
                                foldername = element.Value.AsString;
                                break;
                            case "filetype":
                                filetype = (SyncFiles.Filetype)element.Value.AsInt32;
                                break;

                            default:
                                //Debug.Log("Could not recognize Bson document's field name: " + element.Name);
                                break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(foldername) && !string.IsNullOrEmpty(folderPathName))
                    foldername = folderPathName;

                var bytes = (byte[])elementBytes;
                if (string.IsNullOrEmpty(filename))
                    Debug.Log("Filename is empty!");
                else
                {
                    string fullPath = folderPathName + slash + filename;
                    if (!overWriteFiles && File.Exists(fullPath))
                    {
                        //Debug.Log("File exists, not overwriting!");
                    }
                    else
                        File.WriteAllBytes((fullPath), bytes);

                }

            }
            catch (Exception ex)
            {
                Debug.Log("Error with file: " + filename);
                Debug.Log(ex);
            }
        }
    }

    public static void ExportJSONFileFromDatabase<T>(IMongoCollection<BsonDocument> targetCollection, string filepath)
    {
        ExportJSONFileFromDatabase<T>(null, null, null, targetCollection, filepath, 20);
    }

    public static void ExportJSONFileFromDatabase<T>(FilterDefinition<BsonDocument> filter, SortDefinition<BsonDocument> sort,
        ProjectionDefinition<BsonDocument> projection, IMongoCollection<BsonDocument> targetCollection, string filepath, int limit)
    {

        if (filter == null)
            filter = new BsonDocument();
        if (sort == null)
            sort = Builders<BsonDocument>.Sort.Descending("date");
        if (projection == null)
            projection = Builders<BsonDocument>.Projection.Exclude("_id");

        var cursor = targetCollection.Find(filter).Project(projection).Sort(sort).Limit(limit).ToCursor();
        foreach (var document in cursor.ToEnumerable())
        {
            using (var streamWriter = new StreamWriter(filepath))
            using (var stringWriter = new StringWriter())
            using (var jsonWriter = new JsonWriter(stringWriter))
            {
                var context = BsonSerializationContext.CreateRoot(jsonWriter);
                targetCollection.DocumentSerializer.Serialize(context, document);
                var line = stringWriter.ToString();
                streamWriter.WriteLine(line);
            }
            SaveData.LoadItems<T>(filepath);  //Works, but reading the whole file (instead of piecemeal) or loading straight to memory would be more efficient

        }

    }

    public static void ExportContainersFromDatabase<T>(IMongoCollection<BsonDocument> targetCollection)
    {
        ExportContainersFromDatabase<T>(null, null, null, targetCollection, true);
    }

    //Not working at the moment
    public static void ExportContainersFromDatabase<T>(FilterDefinition<BsonDocument> filter, SortDefinition<BsonDocument> sort,
        ProjectionDefinition<BsonDocument> projection, IMongoCollection<BsonDocument> targetCollection, bool onlyExcludeID)
    {
        if (filter == null)
            filter = new BsonDocument();
        if (sort == null)
            sort = Builders<BsonDocument>.Sort.Descending("date");
        if (projection == null || !onlyExcludeID)
            projection = Builders<BsonDocument>.Projection.Exclude("_id");

        var cursor = targetCollection.Find(filter).Project(projection).Sort(sort).ToCursor();
        foreach (var document in cursor.ToEnumerable())
        {
            Container<T> newContainer = BsonSerializer.Deserialize<Container<T>>(document);  //Error: Invalid generic arguments, typearguments
            SaveData.LoadItems(newContainer);
        }
    }

    public static void UpdateFileInDatabase(FilterDefinition<BsonDocument> filter,
        UpdateDefinition<BsonDocument> update, IMongoCollection<BsonDocument> collection)
    {
        collection.UpdateOne(filter, update);
    }


}

