SquirrelFramework.MongoDB 2.0.0
===============

Squirrel Framework - A lightweight back-end framework and utilities kit based on MongoDB.

The word Squirrel has the meaning of squirrels and storage. The Squirrel Framework is working to make your MongoDB / Azure Cosmos DB-based applications lightweight and fast.

[Dependencies Description](https://github.com/imnista/SquirrelFramework/blob/master/Dependencies.md)

[Release notes: 2.0.0 (Chinese, 中文)](http://nap7.com/squirrel-framework-2-0-0/)

[Release notes: 1.0.15 (Chinese, 中文)](http://nap7.com/squirrel-framework-1-0-15/)

[Release notes: 1.0.14 (Chinese, 中文)](http://nap7.com/squirrel-framework-1-0-14/)

[Quick Start (Chinese, 中文)](http://nap7.com/squirrel-framework-1-0-13/)

Get Package
------------

You can get the latest stable release from the [official Nuget.org feed](https://www.nuget.org/packages/SquirrelFramework.MongoDB)

<https://www.nuget.org/packages/SquirrelFramework.MongoDB>

Getting Started
---------------

1. Create a .NET project, please ensure that the target framework `MUST be .NET 6 or later`

1. Get the Nuget package by searching the keyword "SquirrelFramework.MongoDB" or using the Project Manager

    ```Shell
    Install-Package SquirrelFramework.MongoDB -Version 2.0.0
    ```

1. Create your Domain Model

    ```C#
    using SquirrelFramework.Model;

    [Database("YourDatabaseName")]
    [Collection("UsersCollectionName")]
    public class User : Document
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
    }
    ```
    `The [Database] attribute is not necessary`, you can set the default MongoDB database name when initialing the configurations.

    ```C#    
    Configurations.Configure("mongodb://localhost:27017", "Test");
    MongoDBBasicTestCrudRepository = new MongoDBBasicTestCrudRepository();
    var pingResult = MongoDBBasicTestCrudRepository.Ping();
    Console.WriteLine(pingResult);
    ```

    `Since 1.0.14 the [Collection] attribute is no longer required.` If you not specified the [Collection] attribute, the Squirrel Framework use the class name as the collection name.

1. Create your Repository for MongoDB CRUD

    ```C#
    using SquirrelFramework.Repository;

    public class UserRepository: RepositoryBase<User> {}
    ```

1. Now you are free to perform various operations on MongoDB, here are some examples

    ```C#
            var userRepo = new UserRepository();
    ```

    * Add a new user record

        ```C#
            userRepo.Add(new User{
                Name = "Hendry",
                Gender = "Male",
                Age = 18,
                Geolocation = new Geolocation(121.551949, 38.890957)
            });
        ```

    * Get all users who are 2 kilometers away from me

        ```C#
            userRepo.GetNearBy(new Geolocation(121.551949, 38.890957), 2000);
        ```

    * Bulk delete users who are older than 25 asynchronously

        ```C#
            userRepo.DeleteManyAsync(u => u.Age > 25);
        ```

    * Get the third page (15 records) of all user data and descending sort by the Age field

        ```C#
            //  Method signature
            //  public IEnumerable<TDomain> GetAllByPageSortBy(
            //      int pageIndex,
            //      int pageSize,
            //      Expression<Func<TDomain, object>> sortBy,
            //      bool isSortByDescending = false
            //  );

            userRepo.GetAllByPageSortBy(2, 15, u => u.Age, true);
        ```

1. If your data collection is dynamic, for example you have multiple collections to store your user information:
    * Users201801
    * Users201802
    * Users201803
    * ...

    You can use the CustomizedRepositoryBase class as a base class

    ```C#
        public class UserRepository: CustomizedRepositoryBase<User> {}
    ```

    Then you can provide the specific collection name for each CRUD operation.

    * Add a new user record
        ```C#
            var userRepo = new UserRepository();
            userRepo.Add("Users201805", new User{
                Name = "George",
                Gender = "Male",
                Age = 18,
                Geolocation = new Geolocation(121.551949, 38.890957)
            });
        ```

1. Create your Domain Service (This is not a necessary step)

    ```C#
        public class UserService : ServiceBase<User, UserRepository> {}
    ```

Contributing
------------

* George Qi              me@nap7.com

`If you have any questions, please feel free to contact me`
