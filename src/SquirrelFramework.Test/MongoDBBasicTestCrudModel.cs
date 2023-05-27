using SquirrelFramework.Model;

namespace SquirrelFramework.Test
{
    [Model.Collection("MongoDBBasicTestCrudModel")]
    public class MongoDBBasicTestCrudModel : Document
    {
        public string? Name { get; set; }
        public bool Gender { get; set; }
        public int Age { get; set; }
        public double Number { get; set; }
        public Guid UniqueIdentity { get; set; }
        public Geolocation? Location { get; set; }
        public DateTime Time { get; set; }
        public decimal Money { get; set; }
    }
}
