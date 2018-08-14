using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
//using MongoDB.Driver.Core;
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

    //private static string defaultConnectionString = "mongodb://192.168.100.21:27017";

    #region variables

    public static MongoClient client;
    public static IMongoDatabase activeDatabase;
    //public static IMongoCollection<Comment> commentCollection;
    //public static IMongoCollection<Container<TransformData>> transformCollection;
    public static IMongoCollection<BsonDocument> transformCollection;
    public static IMongoCollection<BsonDocument> commentCollection;
    private static string defaultDB = "tikkuraitti";
    private static string defaultUser = "buser";
    private static string defaultPwd = "1234";
    private static MongoCredential defaultDBCredentials =
        MongoCredential.CreateCredential(defaultDB, defaultUser, defaultPwd);
    private static string commentColName = "comments";
    private static string transformColName = "transforms";



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
        //commentCollection = ConnectToDatabaseCollection<Comment>(commentColName);
        //transformCollection = ConnectToDatabaseCollection<TransformData>(transformColName);
        //commentCollection = activeDatabase.GetCollection<Comment>(commentColName);
        //transformCollection = activeDatabase.GetCollection<Container<TransformData>>(transformColName);
        transformCollection = activeDatabase.GetCollection<BsonDocument>(transformColName);
        commentCollection = activeDatabase.GetCollection<BsonDocument>(commentColName);
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

    public static void TestMethod1(string filepath)
    {
        Debug.Log("Tämä on testi1");
        ImportJSONFileToDatabase(transformCollection, filepath);
        //ExportJSONFileFromDatabase(mongoCollection, filepath);
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

    public static void ExportJSONFileFromDatabase(IMongoCollection<BsonDocument> targetCollection, string filepath)
    {
        ExportJSONFileFromDatabase(null, null, null, targetCollection, filepath, true);
    }

    public static void ExportJSONFileFromDatabase(FilterDefinition<BsonDocument> filter, SortDefinition<BsonDocument> sort,
        ProjectionDefinition<BsonDocument> projection, IMongoCollection<BsonDocument> targetCollection, string filepath, bool excludeID)
    {
        using (var streamWriter = new StreamWriter(filepath))
        {
            if (filter == null)
                filter = new BsonDocument();
            if (sort == null)
                sort = Builders<BsonDocument>.Sort.Descending("date");
            if (projection == null && !excludeID)
                projection = Builders<BsonDocument>.Projection.Exclude("_id");

            var cursor = targetCollection.Find(filter).Project(projection).Sort(sort).ToCursor();
            foreach (var document in cursor.ToEnumerable())
            {

                using (var stringWriter = new StringWriter())
                using (var jsonWriter = new JsonWriter(stringWriter))
                {
                    var context = BsonSerializationContext.CreateRoot(jsonWriter);
                    targetCollection.DocumentSerializer.Serialize(context, document);
                    var line = stringWriter.ToString();
                    streamWriter.WriteLine(line);
                }
            }

        }
    }

    public static void ExportContainersFromDatabase<T>(IMongoCollection<BsonDocument> targetCollection)
    {
        ExportContainersFromDatabase<T>(null, null, null, targetCollection, true);
    }

    public static void ExportContainersFromDatabase<T>(FilterDefinition<BsonDocument> filter, SortDefinition<BsonDocument> sort,
        ProjectionDefinition<BsonDocument> projection, IMongoCollection<BsonDocument> targetCollection, bool excludeID)
    {
        if (filter == null)
            filter = new BsonDocument();
        if (sort == null)
            sort = Builders<BsonDocument>.Sort.Descending("date");
        if (projection == null || !excludeID)
            projection = Builders<BsonDocument>.Projection.Exclude("_id");

        var cursor = targetCollection.Find(filter).Project(projection).Sort(sort).ToCursor();
        foreach (var document in cursor.ToEnumerable())
        {
            Container<T> newContainer = BsonSerializer.Deserialize<Container<T>>(document);
            SaveData.LoadItems(newContainer);
        }
    }



    public static void LoadFileFromDatabase(FilterDefinition<BsonDocument> filter,
        string filepath, IMongoCollection<BsonDocument> collection)
    {
        var document = collection.Find(filter).First();
    }


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

