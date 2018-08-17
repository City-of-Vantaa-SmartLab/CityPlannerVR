using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
using System.IO;
using System;


public class MongoDBAPI {

    public class CollectionData
    {
        public string ip;
        public string databaseName;
        public string collectionName;
    }

    [Serializable]
    private class BinaryDocument
    {
        public string userName;
        public string filepath;
        public string submittedShortDate;
        public BsonBinaryData bsonBinaryData;
    }

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
    private static string defaultDB = "tikkuraitti";
    private static string defaultUser = "buser";
    private static string defaultPwd = "1234";
    private static MongoCredential defaultDBCredentials =
        MongoCredential.CreateCredential(defaultDB, defaultUser, defaultPwd);
    private static string commentColName = "comments";
    private static string transformColName = "transforms";
    private static string imageColName = "images";
    private static string voiceColName = "voicefiles";




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

    public static void UseDefaultConnections()
    {
        ConnectToClient(defaultSettings);
        if (client != null)
        {
            activeDatabase = client.GetDatabase(defaultDB);
            if (activeDatabase != null)
                ConnectToDefaultCollections();
        }
        else
        {
            Debug.Log("Server client is null!");
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

    public static void TestMethod1(string filepath, string filenameExtended)
    {
        Debug.Log("Tallennetaan " + filepath);
        ImportBinaryFileToDatabase(imageCollection, filepath, filenameExtended);
    }

    public static void TestMethod2(string filepath)
    {
        Debug.Log("Ladataan " + filepath);
        ExportBinaryFileFromDatabase(imageCollection, filepath);
    }

    public static void ImportJSONFileToDatabase(IMongoCollection<BsonDocument> targetCollection, string filepath)
    {
        using (var streamReader = new StreamReader(filepath))
        {
            //Debug.Log("Streamreader start!");
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                using (var jsonReader = new JsonReader(line))
                {
                    var context = BsonDeserializationContext.CreateRoot(jsonReader);
                    var document = targetCollection.DocumentSerializer.Deserialize(context);
                    targetCollection.InsertOne(document);
                    //Debug.Log("Insert done!");
                }
            }
        }
    }

    public static void ImportBinaryFileToDatabase(IMongoCollection<BsonDocument> targetCollection, string filepath, string filenameExtended)
    {
        using (var streamReader = new StreamReader(filepath))
        {

            //document.userName = PhotonNetwork.player.NickName;
            //if (string.IsNullOrEmpty(document.userName))
            //    document.userName = "N/A";
            //document.filepath = filepath;
            //document.submittedShortDate = System.DateTime.Now.ToShortDateString();

            var bytes = File.ReadAllBytes(filepath);

            BsonBinaryData binaryData = new BsonBinaryData(bytes);
            List<BsonElement> elementList = new List<BsonElement>();
            //BsonElement elementByte = new BsonElement("bytes", binaryData.AsBsonValue);
            BsonElement elementByte = new BsonElement("bytes", binaryData);
            elementList.Add(elementByte);
            if (string.IsNullOrEmpty(filenameExtended))
            {
                Debug.Log("Filename is missing!");
                filenameExtended = "N/A";
            }
            BsonElement elementFilename = new BsonElement("filename", BsonValue.Create(filenameExtended));
            elementList.Add(elementFilename);

            BsonDocument document = new BsonDocument(elementList);
            targetCollection.InsertOne(document);

        }
    }

    public static void ExportBinaryFileFromDatabase(IMongoCollection<BsonDocument> targetCollection, string filepath)
    {
        ExportBinaryFileFromDatabase(null, null, null, targetCollection, filepath, true);
    }

    public static void ExportBinaryFileFromDatabase(FilterDefinition<BsonDocument> filter, SortDefinition<BsonDocument> sort,
    ProjectionDefinition<BsonDocument> projection, IMongoCollection<BsonDocument> targetCollection, string filepath, bool onlyExcludeID)
    {
        if (filter == null)
            filter = new BsonDocument();
        if (sort == null)
            sort = Builders<BsonDocument>.Sort.Descending("_id");
        //if (projection == null || !onlyExcludeID)
        //    projection = Builders<BsonDocument>.Projection.Exclude("_id");
        if (projection == null)
            projection = Builders<BsonDocument>.Projection.Include("bytes");

        var mongoCursor = targetCollection.Find(filter).Project(projection).Sort(sort).ToCursor();  //With or without cursor, still missing something
        var document = mongoCursor.ToBsonDocument();  //This is the likely culprit

        //foreach(var document in mongoCursor.ToEnumerable())
        //{

        //}

        BsonElement wut = new BsonElement();
        if (document == null)
        {
            Debug.Log("Document is null!");
            return;
        }
        //document.TryGetElement("bytes", out wut);
        wut = document.FirstOrDefault();
        if (wut == null)
        {
            Debug.Log("Wut is null!");
            return;
        }
        if (wut.Value == null)
        {
            Debug.Log("Wut value is null!");
            return;
        }
        Debug.Log("Wut is it: " + wut.Name);
        Debug.Log("Wut has it: " + wut.Value);
        //var wat = BsonBinaryData.Create(wut.Value);
        var wat = wut.Value.AsBsonBinaryData;
        //var bytes = wut.Value.AsBsonBinaryData.Bytes;
        var bytes = wat.Bytes;

        if (bytes == null)
            Debug.Log("Bytes are null!");
        else
            File.WriteAllBytes(filepath, bytes);

        //var cursor = targetCollection.Find(filter).Project(projection).Sort(sort).ToCursor();
        //foreach (var document in cursor.ToEnumerable())
        //{
        //    using (var streamWriter = new StreamWriter(filepath))
        //    using (var stringWriter = new StringWriter())
        //    //using (var jsonWriter = new JsonWriter(stringWriter))
        //    {
        //        //var context = BsonSerializationContext.CreateRoot(jsonWriter);
        //        //targetCollection.DocumentSerializer.Serialize(context, document);
        //        var line = stringWriter.ToString();
        //        streamWriter.WriteLine(line);
        //    }
        //}
    }

    public static void ExportJSONFileFromDatabase<T>(IMongoCollection<BsonDocument> targetCollection, string filepath)
    {
        //Task taskA = new Task(() => ExportJSONFileFromDatabase(null, null, null, targetCollection, filepath, true));
        //taskA.Start();
        //Debug.Log("Vladislav");
        ////taskA.Wait();
        //Debug.Log("Task don't hurt me");

        ExportJSONFileFromDatabase<T>(null, null, null, targetCollection, filepath, true);
    }

    public static void ExportJSONFileFromDatabase<T>(FilterDefinition<BsonDocument> filter, SortDefinition<BsonDocument> sort,
        ProjectionDefinition<BsonDocument> projection, IMongoCollection<BsonDocument> targetCollection, string filepath, bool onlyExcludeID)
    {
        //using (var streamWriter = new StreamWriter(filepath))
        //{
        if (filter == null)
            filter = new BsonDocument();
        if (sort == null)
            sort = Builders<BsonDocument>.Sort.Descending("date");
        if (projection == null || !onlyExcludeID)
            projection = Builders<BsonDocument>.Projection.Exclude("_id");

        var cursor = targetCollection.Find(filter).Project(projection).Sort(sort).ToCursor();
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

        //}
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



    //public static void LoadFileFromDatabase(FilterDefinition<BsonDocument> filter,
    //    string filepath, IMongoCollection<BsonDocument> collection)
    //{
    //    var document = collection.Find(filter).First();
    //}


    //instead this could be done with:
    //var filter = Builders<BsonDocument>.Filter.Eq("i", 10);
    //var update = Builders<BsonDocument>.Update.Set("i", 110);
    //collection.UpdateOne(filter, update);
    public static void UpdateFileInDatabase(FilterDefinition<BsonDocument> filter,
        UpdateDefinition<BsonDocument> update, IMongoCollection<BsonDocument> collection)
    {
        collection.UpdateOne(filter, update);
    }


}



////Codes below are from a tutorial: https://www.youtube.com/watch?v=rMbIx4Yk6U8
//namespace mongo20
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            var client = new MongoClient();
//            var db = client.GetDatabase("Tikkuraitti");
//            var coll = db.GetCollection<SaveData>("Comments");

//            var customerId = new ObjectId("5b3c9032ac815c5d9c8dafab");

//            var customers = coll
//                .Find(b => b.customers == customerId)
//                .Limit(5)
//                .ToListAsync()
//                .Result;

//            Console.WriteLine("Books");
//            foreach (var in customers)
//            {
//                Console.WriteLine(" * " + customerId. ;
//            }

//        }

//        private static async Task<ReplaceOneResult> SaveAsync<T>(
//            this IMongoCollection<T> collection, T entity) where T : IIdentified
//        {
//            return await collection.ReplaceOneAsync(
//                i => i.Id == entity.Id,
//                entity,
//                new UpdateOptions { IsUpsert = true });  //adds the entity, if not found in database
//        }

//        public interface IIdentified
//        {
//            ObjectId Id { get; }
//        }

//    }



//}

