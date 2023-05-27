namespace SquirrelFramework.Test
{
    public class MongoDBBasicTest : IClassFixture<TestFixture>
    {
        private readonly TestFixture _testFixture;
        private MongoDBBasicTestCrudRepository _mongoDBBasicTestCrudRepository { get; set; }

        public MongoDBBasicTest(TestFixture testFixture)
        {
            _testFixture = testFixture;
            _mongoDBBasicTestCrudRepository = _testFixture.MongoDBBasicTestCrudRepository;
        }

        [Fact]
        public void AddAndRead_Success()
        {
            // Arrange
            var expected = new MongoDBBasicTestCrudModel
            {
                Id = _mongoDBBasicTestCrudRepository.GenerateNewId(),
                Geolocation = new Model.Geolocation(114.10, 168.18),
                Name = "George Qi",
                Gender = false,
                Age = 18,
                Number = 3.14,
                UniqueIdentity = Guid.NewGuid(),
                Location = new Model.Geolocation(114.10, 168.18),
                Time = DateTime.UtcNow,
                Money = 360000000,
            };

            // Act
            _mongoDBBasicTestCrudRepository.Add(expected);
            var actual = _mongoDBBasicTestCrudRepository.Get(expected.Id);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expected.Id, actual.Id);
        }
    }
}