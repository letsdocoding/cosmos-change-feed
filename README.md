# Cosmos-Change-Feed
This repo has sample code that can be used to  the kick start the change feed functionality  for anyone  who want to do hands on cosmos change feed for SQL or Cosmos

For SQL -  Create 2 container in Cosmos one for watch and another for lease. For Mongo just create one collection to watch

The following limitations are applicable when using change streams:

The operationType and updateDescription properties are not yet supported in the output document. The insert, update, and replace operations types are currently supported. Delete operation or other events are not yet supported.

More detail can be found at https://docs.microsoft.com/en-us/azure/cosmos-db/mongodb-change-streams?tabs=javascript



