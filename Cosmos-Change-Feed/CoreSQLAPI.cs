/*
 Author: Abhinav Mishra
 emailid:teachmeabhinav@gmail.com 
 */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Cosmos_Change_Feed
{
    public class CoreSqlApi
    {
        private static CosmosClient _client;
        private readonly string _dbConnectionString;
        private readonly string _collectionToWatch;
        private readonly string _collectionLease;
        private readonly string _dbName;

        public CoreSqlApi(string dbConnectionString, string dbName, string collectionToWatch, string collectionLease)
        {
            _dbConnectionString = dbConnectionString ?? throw new ArgumentNullException(nameof(dbConnectionString));
            _collectionToWatch = collectionToWatch ?? throw new ArgumentNullException(nameof(collectionToWatch));
            _collectionLease = collectionLease ?? throw new ArgumentNullException(nameof(collectionLease));
            _dbName = dbName ?? throw new ArgumentNullException(nameof(dbName));
        }

        public void ChangeFeed()
        {
            Task.Run(async () =>
            {

                _client = new CosmosClient(_dbConnectionString);
                Console.WriteLine("Start Configuration");
                var database = _client.GetDatabase(_dbName);
                var cartContainer = database.GetContainer(_collectionToWatch);
                var leaseContainer = database.GetContainer(_collectionLease);

                var cpf = cartContainer.GetChangeFeedProcessorBuilder<dynamic>("CLP Demo", ProcessChanges)
                          .WithLeaseContainer(leaseContainer)
                          .WithInstanceName("Change Feed Processor Lib Demo")
                          .WithStartTime(DateTime.MinValue.ToUniversalTime()) // to get feed from staring of point
                          //.WithPollInterval for pooling
                          //.WithMaxItems max item                          
                          .Build();

                await cpf.StartAsync();
                Console.WriteLine("CPL start");
                Console.ReadKey(true);
                await cpf.StopAsync();
            }).Wait();
        }
        Task ProcessChanges(IReadOnlyCollection<dynamic> docs, CancellationToken cancellationToken)
        {
            foreach (var doc in docs)
            {
                Console.WriteLine($"Document Id {doc.id}");
                Console.WriteLine(doc);
                //Do custom processing
            }

            return Task.CompletedTask;
        }
    }
}
