/*
 Author: Abhinav Mishra
 emailid:teachmeabhinav@gmail.com 
 */
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChangeFeed
{
    public class CoreSQLAPI
    {
        private static CosmosClient client { get; set; }
        private string dbConnectionString { get; set; }
        private string collectiontoWatch { get; set; }
        private string collectionLease { get; set; }
        private string dbName { get; set; }

        public CoreSQLAPI(string dbConnectionString, string collectiontoWatch, string collectionLease, string dbName)
        {
            this.dbConnectionString = dbConnectionString ?? throw new ArgumentNullException(nameof(dbConnectionString));
            this.collectiontoWatch = collectiontoWatch ?? throw new ArgumentNullException(nameof(collectiontoWatch));
            this.collectionLease = collectionLease ?? throw new ArgumentNullException(nameof(collectionLease));
            this.dbName = dbName ?? throw new ArgumentNullException(nameof(dbName));
        }

        public void Changefeed()
        {
            Task.Run(async () =>
            {

                client = new CosmosClient(this.dbConnectionString);
                Console.WriteLine("Start Configuration");
                var database = client.GetDatabase(this.dbName);
                var cartContainer = database.GetContainer(this.collectiontoWatch);
                var leaseContainer = database.GetContainer(this.collectionLease);

                var cpf = cartContainer.GetChangeFeedProcessorBuilder<dynamic>("CLP Demo", processChanges)
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
        async Task processChanges(IReadOnlyCollection<dynamic> docs, CancellationToken cancellationToken)
        {
            foreach (var doc in docs)
            {
                Console.WriteLine($"Document Id {doc.id}");
                Console.WriteLine(doc);
            }
        }
    }
}
