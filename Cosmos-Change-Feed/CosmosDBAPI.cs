/*
 Author: Abhinav Mishra
 emailid:teachmeabhinav@gmail.com 
 */
using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace ChangeFeed
{
    public class CosmosDBAPI
    {
        private MongoClient client { get; set; }
        private IMongoDatabase dataBase { get; set; }
        private IMongoCollection<BsonDocument> collection { get; set; }
        private string dbConnectionString { get; set; }
        private string dbName { get; set; }
        private string dbCollection { get; set; }
        private string shardkey { get; set; }

        public CosmosDBAPI(string dbConnectionString, string dbName, string dbCollection, string _shardkey)
        {
            this.dbConnectionString = dbConnectionString ?? throw new ArgumentNullException(nameof(dbConnectionString));
            this.dbName = dbName ?? throw new ArgumentNullException(nameof(dbName));
            this.dbCollection = dbCollection ?? throw new ArgumentNullException(nameof(dbCollection));
            this.shardkey = _shardkey ?? throw new ArgumentNullException(nameof(_shardkey));
            this.dbConnectionString = dbConnectionString ?? throw new ArgumentNullException(nameof(dbConnectionString));
            this.dbName = dbName ?? throw new ArgumentNullException(nameof(dbName));
            this.dbCollection = dbCollection ?? throw new ArgumentNullException(nameof(dbCollection));
            client = new MongoClient(this.dbConnectionString);
            dataBase = client.GetDatabase(this.dbName);
            collection = dataBase.GetCollection<BsonDocument>(this.dbCollection);
            shardkey = _shardkey;
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
            Console.WriteLine($"Change Stream Start for {this.collection}");
            var enumerator = collection.Watch(pipeline, options).ToEnumerable().GetEnumerator();
            try
            {


                while (enumerator.MoveNext())
                {
                    Console.WriteLine(enumerator.Current.ToString());                  
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                enumerator.Dispose();
            }
        }
    }
}
