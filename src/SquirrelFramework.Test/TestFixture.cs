using SquirrelFramework.MongoDB;

namespace SquirrelFramework.Test
{
    public class TestFixture : IDisposable
    {
        public MongoDBBasicTestCrudRepository? MongoDBBasicTestCrudRepository { get; set; }

        public TestFixture()
        {
            // 在此处放置所有全局初始化逻辑
            ConnectMongoDB();
        }

        public void Dispose()
        {
            // 在此处放置所有全局清理逻辑
            DropMongoDB();
        }

        private void ConnectMongoDB()
        {
            // Please change the properties of mongodb.json
            // Build Action: Content
            // Copy to Output Directory: Copy always

            Configurations.Configure("mongodb://localhost:27017", "Test");
            MongoDBBasicTestCrudRepository = new MongoDBBasicTestCrudRepository();
            var pingResult = MongoDBBasicTestCrudRepository.Ping();
            Console.WriteLine(pingResult);
        }

        private void DropMongoDB()
        {
            // Drop all data
            MongoDBBasicTestCrudRepository?.DropCollection();
        }
    }

}
