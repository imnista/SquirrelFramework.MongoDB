namespace SquirrelFramework.MongoDB
{
    public static class Configurations
    {
        public static string MongoDBConnectionString { get; private set; } = "mongodb://localhost:27017";

        public static string MongoDBDefaultDatabaseName { get; private set; } = "Demo";

        public static void Configure(string mongoDBConnectionString, string mongoDBDefaultDatabaseName)
        {
            MongoDBConnectionString = mongoDBConnectionString;
            MongoDBDefaultDatabaseName = mongoDBDefaultDatabaseName;
        }
    }
}
