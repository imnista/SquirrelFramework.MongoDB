using MongoDB.Driver;

namespace SquirrelFramework.Repository.Internal
{
    internal class MongoDBCollection
    {
        public static IMongoCollection<T> GetCollection<T>(
            MongoClient client, 
            string databaseName,
            string collectionName)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client), "Mongo client object was not specified.");
            }
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentNullException(nameof(databaseName),
                    "You must specify a valid MongoDB database name.");
            }
            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentNullException(nameof(collectionName),
                    "You must specify a valid MongoDB collection name.");
            }
            var database = client.GetDatabase(databaseName);
            return database.GetCollection<T>(collectionName);
        }
    }
}