﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using System.Reflection;
//using MongoDB.Driver.Builders;
using PS.Models;

namespace PS.Services
{
    public class MongoRepository : IMongoRepository
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;
        private const string conString = "mongodb://localhost:27017";

        public MongoRepository()
        {
            _client = new MongoClient(conString);
            _database = _client.GetDatabase("car");
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName, string database)
        {
        var dataBase = _client.GetDatabase(database);
            var collection = dataBase.GetCollection<T>(collectionName);
            return collection;
        }

        public List<string> getAll()
        {
            var list = _database.ListCollectionsAsync().Result.ToListAsync().Result;
            List<string> carCollectionName = new List<string>();
            var bson = (BsonExtensionMethods.ToJson(list));
            var bsonDictionary = JsonConvert.DeserializeObject<List<Dictionary<string, dynamic>>>(bson);

            foreach (var i in bsonDictionary)
            {
                carCollectionName.Add(i.Values.First());
            }

            return carCollectionName;
        }

        public List<Car> getAll(string colletionName)
        {
            var collection = _database.GetCollection<Car>(colletionName);
            var modelList = collection.Find(new BsonDocument()).ToListAsync().Result;
            return modelList;
        }
        public List<Car> getSelected(string id)
        {
            var collection = _database.GetCollection<Car>("Skoda");
            if (!string.IsNullOrEmpty(id))
            {
                var modelList = collection.Find(b => b._id == new ObjectId(id)).ToListAsync().Result;
                return modelList;
            }
            return new List<Car>();
        }
        public List<string> getTypeFromCollection(string colletionName)
        {
            List<string> typeArray = new List<string>();
            if (!string.IsNullOrEmpty(colletionName))
            {
                var modelList = getAll(colletionName);
                foreach (var m in modelList)
                {
                    //  typeArray.Add(m.Type);
                }
                var query = from c in modelList
                                //  where c.Type.Contains("Sedan")
                            select c;
            }
            return typeArray;

        }
        //public IQueryable<List<Car>> select()
        //{
        //    var collection = _database.GetCollection<Car>("Skoda").Find(new BsonDocument());
        //   var query = from c in collection
        //    //            where c.Type.Contains("Sedan")
        //    //            select c;

        //    ////delete code
        //    //var query1 = Query<Car>.EQ(e => e.name, "Rapid");
        //    //collection.DeleteOneAsync(query1);
        //    ////delete code end

        //    //return query;
        //}


        public bool insert()
        {
            var collection = _database.GetCollection<Car>("Skoda");
            Car c = new Car();
            c.name = "Test";
            //  c.Type = "SUV";
            collection.InsertOneAsync(c);
            return true;
        }
        public IEnumerable<IEnumerable<string>> convertToPresentationList(List<string> collectionList)

        {
            int take, skip, divisor;
            List<IEnumerable<string>> listAll = new List<IEnumerable<string>>();
            collectionList.Sort();

            divisor = collectionList.Count < 10 ? 3 : 4;

            take = (collectionList.Count + divisor - 1) / divisor;

            for (int i = 0; i < divisor; i++)
            {
                skip = i * take;
                listAll.Add(collectionList.Skip(skip).Take(take).ToList());

            }

            return listAll;
        }


        public List<IEnumerable<int>> getYears()
        {
            var yearsList = new List<int>();
            int take, skip, divisor;
            var currYear = DateTime.Now.Year;
            for(int i = 1990; i<= currYear; i++)
            {
                yearsList.Add(i);
            }

            yearsList.Sort();
            List<IEnumerable<int>> listAll = new List<IEnumerable<int>>();

            divisor = yearsList.Count < 10 ? 3 : 4;

            take = (yearsList.Count + divisor - 1) / divisor;

            for (int i = 0; i < divisor; i++)
            {
                skip = i * take;
                listAll.Add(yearsList.Skip(skip).Take(take).ToList());

            }

            return listAll;
        }
    }
}
