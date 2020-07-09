/*
 Author: Abhinav Mishra
 emailid:teachmeabhinav@gmail.com 
 */

using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace Cosmos_Change_Feed
{
    public class CosmosDbApi
    {
        private readonly IMongoCollection<BsonDocument> _collection;
        private string _shardKey;

        public CosmosDbApi(string dbConnectionString, string dbName, string dbCollection, string shardKey)
        {
            if (dbConnectionString == null)
            {
                throw new ArgumentNullException(nameof(dbConnectionString));
            }

            if (dbName == null)
            {
                throw new ArgumentNullException(nameof(dbName));
            }

            if (dbCollection == null)
            {
                throw new ArgumentNullException(nameof(dbCollection));
            }
            _shardKey = shardKey ?? throw new ArgumentNullException(nameof(shardKey));
            var client = new MongoClient(dbConnectionString);
            var dataBase = client.GetDatabase(dbName);
            _collection = dataBase.GetCollection<BsonDocument>(dbCollection);
            _shardKey = shardKey;
        }
        public void ChangeFeed()
        {
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<BsonDocument>>()
                  .Match(change => change.OperationType == ChangeStreamOperationType.Insert
                  || change.OperationType == ChangeStreamOperationType.Update
                  || change.OperationType == ChangeStreamOperationType.Replace)
                  .AppendStage<ChangeStreamDocument<BsonDocument>, ChangeStreamDocument<BsonDocument>, BsonDocument>(
                  "{ $project: { '_id': 1, 'fullDocument': 1, 'ns': 1, 'documentKey': 1 }}");

            var options = new ChangeStreamOptions
            {
                FullDocument = ChangeStreamFullDocumentOption.UpdateLookup
            };
            Console.WriteLine($"Change Stream Start for {_collection}");
            var enumerator = _collection.Watch(pipeline, options).ToEnumerable().GetEnumerator();
            try
            {


                while (enumerator.MoveNext())
                {
                    if (enumerator.Current != null) Console.WriteLine(enumerator.Current.ToString());
                }
            }
            finally
            {
                enumerator.Dispose();
            }
        }
    }
}
