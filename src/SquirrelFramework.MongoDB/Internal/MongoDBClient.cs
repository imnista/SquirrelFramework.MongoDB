using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using SquirrelFramework.Model;
using SquirrelFramework.MongoDB;

namespace SquirrelFramework.Repository.Internal
{
    internal class MongoDBClient
    {
        static MongoDBClient()
        {
            // Register the base model
            BsonClassMap.RegisterClassMap<Document>(map =>
            {
                // https://imgalib.wordpress.com/2015/03/11/mongo-db-element-does-not-match-any-field-or-property-of-class/
                // https://stackoverflow.com/questions/28286797/missing-setrepresentation-method-in-c-sharp-mongodb-driver-2-0-0-beta1
                map.AutoMap();
                //map.SetIsRootClass(true);
                //map.SetIgnoreExtraElements(true);
                map.SetIdMember(
              map.GetMemberMap(m => m.Id)
                  .SetIdGenerator(StringObjectIdGenerator.Instance)
                  .SetSerializer(new StringSerializer(BsonType.ObjectId)
                  ));
            });
        }

        public static MongoClient Client { get; } = new MongoClient(Configurations.MongoDBConnectionString);

        public static string DefaultDatabaseName { get; } = Configurations.MongoDBDefaultDatabaseName;
    }
}