using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
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
    public static IMongoCollection<Comment> commentCollection;
    public static IMongoCollection<TransformData> transformCollection;
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
    private static MongoClientSettings defaultSettings = new MongoClientSettings
    {
        Server = new MongoServerAddress("192.168.100.21", 27017),
        ServerSelectionTimeout = TimeSpan.FromSeconds(3),
        Credential = defaultDBCredentials
    };

    #endregion

    public static void UseDefaultConnections()
    {
        ConnectToClient(defaultSettings);
        if (client != null)
        {
            activeDatabase = client.GetDatabase(defaultDB);
            if (activeDatabase != null)
                ConnectToDefaultCollections();
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
        commentCollection = activeDatabase.GetCollection<Comment>(commentColName);
        transformCollection = activeDatabase.GetCollection<TransformData>(transformColName);

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

    public static void TestConnections()
    {
        UseDefaultConnections();
        Debug.Log("Yhteyttä yritetty");
        //TestMethods();

    }

    public static void TestMethods()
    {
        Debug.Log("Tämä on testi");
        if (client != null)
        {
            if (activeDatabase != null)
            {
                //if (commentCollection != null)
                //ReadComments(commentCollection);
                //else
                //    Debug.Log("CommentCollection is null!");
                if (transformCollection != null)
                    ReadTransformNames(transformCollection);
                else
                    Debug.Log("CommentCollection is null!");
            }
            else
                Debug.Log("Active database is null!");
        }
        else
            Debug.Log("MongoClient is null!");
    }

    public static void ReadComments(IMongoCollection<Comment> collection)
    {
        
        var commentList = collection
                .Find(b => b.data != null)
                .Limit(5)
                .ToListAsync()
                .Result;

        foreach(Comment com in commentList)
        {
            Debug.Log(com.data);
        }
    }

    public static void WriteComment(Comment comment)
    {

    }

    public static void ReadTransformNames(IMongoCollection<TransformData> collection)
    {
        //Task<List<TransformData>> transformList = collection etc...
        var transformList = collection
            .Find(b => b.gameObjectName == "r_kioski" || b.gameObjectName == "PhotonNewBuildings")
            .Limit(5)
            .ToListAsync()
            .Result;

        foreach (TransformData tra in transformList)
        {
            Debug.Log(tra.gameObjectName);
        }
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

