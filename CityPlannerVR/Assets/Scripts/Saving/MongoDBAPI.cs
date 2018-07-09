//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using MongoDB.Bson;
//using MongoDB.Driver;
//using System.Threading.Tasks;
//using System.Text;
//using System.Linq;
//using System;

//public class MongoDBAPI {

//    public class CollectionData
//    {
//        public string ip;
//        public string databaseName;
//        public string collectionName;
//    }




//    private static string defaultConnectionString = "mongodb://192.168.100.21:27017";

//    public static MongoClient client = new MongoClient();
//    public static CollectionData defaultPositions = new CollectionData();
//    public static CollectionData defaultComments = new CollectionData();


//    public static void ConnectToDefaultPositions()
//    {

//    }

//    public static void ConnectToDatabaseCollection(CollectionData col)
//    {
//        ConnectToDatabaseCollection(col.ip, col.databaseName, col.collectionName);
//    }

//    public static void ConnectToDatabaseCollection(string ip, string databaseName, string collectionName)
//    {
//        client.GetDatabase(databaseName);


//    }

//    public static void Test()
//    {
//        Debug.Log("Tämä on testi");
//    }

//}



////namespace mongo20
////{
////    class Program
////    {
////        static void Main(string[] args)
////        {
////            var client = new MongoClient();
////            var db = client.GetDatabase("Tikkuraitti");
////            var coll = db.GetCollection<SaveData>("Comments");

////            var customerId = new ObjectId("5b3c9032ac815c5d9c8dafab");

////            var customers = coll
////                .Find(b => b.customers == customerId)
////                .Limit(5)
////                .ToListAsync()
////                .Result;

////            Console.WriteLine("Books");
////            foreach (var in customers)
////            {
////                Console.WriteLine(" * " + customerId. ;
////            }

////        }

////        private static async Task<ReplaceOneResult> SaveAsync<T>(
////            this IMongoCollection<T> collection, T entity) where T : IIdentified
////        {
////            return await collection.ReplaceOneAsync(
////                i => i.Id == entity.Id,
////                entity,
////                new UpdateOptions { IsUpsert = true });  //adds the entity, if not found in database
////        }

////        public interface IIdentified
////        {
////            ObjectId Id { get; }
////        }

////    }



////}





////public class MongoDBAPI : MonoBehaviour {

////	// Use this for initialization
////	void Start () {

////	}

////	// Update is called once per frame
////	void Update () {

////	}
////}
