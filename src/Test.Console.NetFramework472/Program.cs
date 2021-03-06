﻿using SquirrelFramework.Domain.Model;
using SquirrelFramework.Repository;
using System;

namespace Test.Console.NetFramework472
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      SquirrelFramework.Configurations.Configurations.Configure("mongodb://localhost:27017", "Test");
      System.Console.WriteLine(SquirrelFramework.Configurations.Configurations.Status);
      var testCrudRepository = new TestCrudRepository();
      var pingResult = testCrudRepository.Ping();
      var data = testCrudRepository.GetAll();
      System.Console.WriteLine(pingResult);
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
