/*
 Author: Abhinav Mishra
 emailid:teachmeabhinav@gmail.com 
 */
using System;

namespace ChangeFeed
{
    class Program
    {
      
        static void Main(string[] args)
        {

            Console.WriteLine("Enter COSMOS API");
            Console.WriteLine("1 for MONGO 2 FOR SQL");
            var type = Convert.ToInt32(Console.ReadLine());
            switch (type)
            {
                case 1:
                    Console.WriteLine("DB Connection String");
                    string dbConnectionString = Console.ReadLine();
                    Console.WriteLine("DB Name");
                    string dbName = Console.ReadLine();
                    Console.WriteLine("DB Collection to watch");
                    string dbCollection = Console.ReadLine();
                    Console.WriteLine("Lease Container name");
                    string shardkey = Console.ReadLine();
                    var cosmos = new CosmosDBAPI(dbConnectionString, dbName, dbCollection, shardkey);
                    cosmos.ChangeFeed();
                    break;
                case 2:
                    Console.WriteLine("DB Connection String");
                    string dbConnectionString1 = Console.ReadLine();
                    Console.WriteLine("DB Name");
                    string dbName1 = Console.ReadLine();
                    Console.WriteLine("Collection Name to watch");
                    string collectiontoWatch = Console.ReadLine();
                    Console.WriteLine("Lease Collection");
                    string leaseCollection = Console.ReadLine();
                    var sql = new CoreSQLAPI(dbConnectionString1, collectiontoWatch, leaseCollection, dbName1);
                    sql.Changefeed();
                    break;
                default:
                    Console.WriteLine("Wrong Entry");
                    break;
            }


        }

    }
}
