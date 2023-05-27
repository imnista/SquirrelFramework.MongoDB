using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SquirrelFramework.Model;
using SquirrelFramework.Utility.Extensions;

namespace SquirrelFramework.Repository.Internal
{
    internal class DataCollectionSelector
    {
        private readonly MongoClient currentClient;
        private readonly string defaultDatabaseName;

        public DataCollectionSelector(MongoClient mongoClient, string defaultDatabaseName)
        {
            currentClient = mongoClient;
            this.defaultDatabaseName = defaultDatabaseName;
        }

        public virtual IEnumerable<string> GetDataCollectionList(string databaseName)
        {
            var collections =
                Enumerate(currentClient.GetDatabase(databaseName).ListCollections()).Select(
                    c => c.GetValue("name").AsString);
            return collections;
        }

        public static IEnumerable<BsonDocument> Enumerate(IAsyncCursor<BsonDocument> docs)
        {
            while (docs.MoveNext())
            {
                foreach (var item in docs.Current)
                {
                    yield return item;
                }
            }
        }

        public virtual IMongoCollection<T> GetDataCollection<T>()
        {
            var partitionLevel = RepositoryContext.GetPartitionSupportLevel();

            // 优先选择 实体标签所指定的数据库名
            var databaseName = typeof(T).GetAttributeValue<DatabaseAttribute, string>(t => t.Name);
            if (databaseName == null)
            {
                if (partitionLevel == PartitionLevel.DatabaseLevel)
                {
                    databaseName = RepositoryContext.GetDatabaseName();
                }
                else
                {
                    databaseName = defaultDatabaseName;
                }
            }

            if (databaseName == null)
            {
                throw new Exception(
                    "You must specified the [Database] attribute for the domain model or set the default database name at the configuration file.");
            }

            // <!> Since 1.0.14 [Collection] attribute is no longer required.
            // Get collection name from [Collection] Attribute
            // If not specified the [Collection] attribute, use the class name as the collection name
            var collectionName =
                typeof(T).GetAttributeValue<CollectionAttribute, string>(t => t.Name) ?? typeof(T).Name;
            if (partitionLevel == PartitionLevel.CollectionLevel)
            {
                collectionName = RepositoryContext.GetCollectionName(collectionName);
            }

            return MongoDBCollection.GetCollection<T>(currentClient, databaseName, collectionName);
        }

        public virtual IMongoCollection<T> GetDataCollection<T>(string collectionName)
        {
            var partitionLevel = RepositoryContext.GetPartitionSupportLevel();

            // 优先选择 实体标签所指定的数据库名
            var databaseName = typeof(T).GetAttributeValue<DatabaseAttribute, string>(t => t.Name);
            if (databaseName == null)
            {
                if (partitionLevel == PartitionLevel.DatabaseLevel)
                {
                    databaseName = RepositoryContext.GetDatabaseName();
                }
                else
                {
                    databaseName = defaultDatabaseName;
                }
            }

            if (databaseName == null)
            {
                throw new Exception(
                    "You must specified the [Database] attribute for the domain model or set the default database name at the configuration file.");
            }

            return MongoDBCollection.GetCollection<T>(currentClient, databaseName, collectionName);
        }

        public virtual IMongoCollection<T> GetDataCollection<T>(string databaseName, string collectionName)
        {
            return MongoDBCollection.GetCollection<T>(currentClient, databaseName, collectionName);
        }

        public virtual IMongoCollection<T> GetDataCollection<T>(MongoClient client, string databaseName,
            string collectionName)
        {
            return MongoDBCollection.GetCollection<T>(client, databaseName, collectionName);
        }
    }
}