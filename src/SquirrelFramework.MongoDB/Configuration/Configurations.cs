namespace SquirrelFramework.Configurations
{
  #region using directives

  using Microsoft.Extensions.Configuration;
  using System;

  #endregion using directives

  public class Configurations
  {
    public static string MongoDBConnectionString { get; private set; }
    public static string MongoDBDefaultDatabaseName { get; private set; }
    public static string Status { get; private set; }

    static Configurations()
    {
      try
      {
        var builder = new ConfigurationBuilder()
            //.SetBasePath(currentDirectory)
            .AddJsonFile("mongodb.json", true, true);
        Config = builder.Build();
        MongoDBConnectionString = GetConfig("MongoDBConnectionString");
        MongoDBDefaultDatabaseName = GetConfig("MongoDBDefaultDatabase");
        if (!string.IsNullOrWhiteSpace(MongoDBConnectionString))
        {
          Status = "loaded by json file";
        }
        else
        {
          Status = "json file not found";
        }
      }
      catch (Exception ex)
      {
        Status = ex.Message;
      }
    }

    public static void Configure(string mongoDBConnectionString, string mongoDBDefaultDatabaseName)
    {
      MongoDBConnectionString = mongoDBConnectionString;
      MongoDBDefaultDatabaseName = mongoDBDefaultDatabaseName;
      Status = "loaded by runtime configure";
    }

    // JSON Config Mode
    public static IConfigurationRoot Config { get; }

    public static string GetConfig(string key)
    {
      return Config.GetSection(key).Value;
    }

    public static bool GetBooleanConfig(string key)
    {
      return bool.Parse(GetConfig(key));
    }
  }
}
