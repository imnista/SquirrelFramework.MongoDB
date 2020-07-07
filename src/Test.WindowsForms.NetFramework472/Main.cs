using SquirrelFramework.Domain.Model;
using SquirrelFramework.Repository;
using System;
using System.Windows.Forms;

namespace Test.WindowsForms.NetFramework472
{
  public partial class Main : Form
  {
    public Main()
    {
      InitializeComponent();
    }

    private void buttonLoad_Click(object sender, EventArgs e)
    {
      //SquirrelFramework.Configurations.Configurations.Configure("mongodb://localhost:27017", "Test");
      Console.WriteLine(SquirrelFramework.Configurations.Configurations.Status);
      var testCrudRepository = new TestCrudRepository();
      var pingResult = testCrudRepository.Ping();

      var model1 = new TestCrudModel
      {
        Name = "HendryForTestAdd1",
        Age = 18
      };
      testCrudRepository.Add(model1);

      var model2 = new TestCrudModel
      {
        Name = "HendryForTestAdd2",
        Age = 22
      };
      testCrudRepository.Add(model2);

      var data = testCrudRepository.GetAll();
      dataGridView.DataSource = data;
    }
  }

  public class TestCrudRepository : RepositoryBase<TestCrudModel>
  {
  }

  [Collection("TestCrudRecord")]
  public class TestCrudModel : DomainModel
  {
    public string Name { get; set; }
    public bool Gender { get; set; }
    public int Age { get; set; }
    public double Number { get; set; }
    public Guid UniqueIdentity { get; set; }
    public Geolocation Location { get; set; }
    public DateTime Time { get; set; }
    public decimal Money { get; set; }
  }
}
