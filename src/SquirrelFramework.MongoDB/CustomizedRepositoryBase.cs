namespace SquirrelFramework.Repository
{
  #region using directives

  using Domain.Model;
  using MongoDB.Bson;
  using MongoDB.Driver;
  using MongoDB.Driver.Linq;
  using System;
  using System.Collections.Generic;
  using System.Linq.Expressions;
  using System.Threading.Tasks;
  using Utility.Common.Coding;

  #endregion using directives

  public abstract class CustomizedRepositoryBase<TDomain> where TDomain : DomainModel
  {
    public const double EarthRadius = 6378.137; // KM

    //private static readonly object classMapLocker = new object();
    private readonly DataCollectionSelector dataSelector;

    protected CustomizedRepositoryBase()
    {
      // Register the sub-model
      // Doing the following works is for implementing SetIgnoreExtraElements.
      // That will allow us to create a new class which just contains the partial fields
      // of the MongoDB Collection.
      // BUT when registering the sub class of the DomainModel,
      // the BsonClassMap gets the fields from the Type.GetFields(),
      // and it only contains the fields of itself,
      // refer to https://stackoverflow.com/questions/9201859/why-doesnt-type-getfields-return-backing-fields-in-a-base-class
      // but not the fields of its base class.
      // So it means that we must register the base class (DomainModel) either.
      // If we register both the sub model and the base model,
      // that would automatically add a new database field "_t" for recording
      // the inheritance relationship between base class and the sub-model class.
      // However, the storage and execution cost will increase.
      // So we disable this feature.
      /*
      var type = typeof(TDomain);
      if (!BsonClassMap.IsClassMapRegistered(type))
      {
          lock (classMapLocker)
          {
              if (!BsonClassMap.IsClassMapRegistered(type))
              {
                  var map = new BsonClassMap(type);
                  map.AutoMap();
                  map.SetIsRootClass(false);
                  map.SetIgnoreExtraElements(true);
                  BsonClassMap.RegisterClassMap(map);
              }
          }
      }
      */
      this.dataSelector = new DataCollectionSelector(MongoDBClient.Client, MongoDBClient.DefaultDatabaseName);
    }

    #region GetNearBy

    //redius unit: Meters
    public IEnumerable<TDomain> GetNearBy(string collectionName, Geolocation centerPoint, double radius /* Meter */)
    {
      // 获取表
      var collection = this.GetCollection(collectionName);

      // 建立空间搜索 索引
      //collection.Indexes.List().
      collection.Indexes.CreateOneAsync(Builders<TDomain>.IndexKeys.Geo2DSphere(m => m.Geolocation)).Wait();

      // 空间搜索算法
      var rangeInKm = radius / 1000; /* KM */
      var radians = rangeInKm / EarthRadius;

      var filter = Builders<TDomain>.Filter.NearSphere(m => m.Geolocation, centerPoint.Longitude,
          centerPoint.Latitude,
          radians);
      return collection.Find(filter).ToList();
    }

    public IEnumerable<TDomain> GetNearBy(
        string collectionName,
        Expression<Func<TDomain, object>> field,
        Geolocation centerPoint,
        double maxRadius /* Meter */,
        double minRadius /* Meter */)
    {
      // 获取表
      var collection = this.GetCollection(collectionName);

      // TODO
      // >>>>?????? 首先检查是不是已经有了这个索引！！！
      // https://stackoverflow.com/questions/35019313/checking-if-an-index-exists-in-mongodb
      var geo2DSphereIndexKeyName =
          $"{collectionName}_{LambdaExpressionHelper.RetrievePropertyNameByLambdaExpression(field)}";

      // 建立空间搜索 索引
      collection.Indexes.CreateOneAsync(Builders<TDomain>.IndexKeys.Geo2DSphere(field), new CreateIndexOptions
      {
        Name = geo2DSphereIndexKeyName
      }).Wait();

      // 空间搜索算法
      var maxDistanceRangeInKm = maxRadius / 1000; /* KM */
      var maxDistanceRadians = maxDistanceRangeInKm / EarthRadius;

      var minDistanceRangeInKm = minRadius / 1000; /* KM */
      var minDistanceRadians = minDistanceRangeInKm / EarthRadius;

      // Add Filter

      var filter = Builders<TDomain>.Filter.NearSphere(
          field,
          centerPoint.Longitude,
          centerPoint.Latitude,
          maxDistanceRadians,
          minDistanceRadians
      );
      return collection.Find(filter).ToList();
    }

    #endregion GetNearBy

    #region GetCount

    public long GetCount(string collectionName)
    {
      return this.GetCollection(collectionName).Count(_ => true);
    }

    public long GetCount(string collectionName, Expression<Func<TDomain, bool>> filter)
    {
      return this.GetCollection(collectionName).Count(filter);
    }

    #endregion GetCount

    #region GetAll

    public IEnumerable<TDomain> GetAll(string collectionName)
    {
      return this.GetCollection(collectionName).Find(_ => true).ToList();
    }

    public IEnumerable<TDomain> GetAll(string collectionName, Expression<Func<TDomain, object>> sortBy,
        bool isSortByDescending = false)
    {
      return isSortByDescending
          ? this.GetCollection(collectionName).Find(_ => true).SortByDescending(sortBy).ToList()
          : this.GetCollection(collectionName).Find(_ => true).SortBy(sortBy).ToList();
    }

    public IEnumerable<TDomain> GetAll(string collectionName, Expression<Func<TDomain, bool>> filter)
    {
      return this.GetCollection(collectionName).Find(filter).ToList();
    }

    public IEnumerable<TDomain> GetAll(string collectionName, Expression<Func<TDomain, bool>> filter,
        Expression<Func<TDomain, object>> sortBy, bool isSortByDescending = false)
    {
      return isSortByDescending
          ? this.GetCollection(collectionName).Find(filter).SortByDescending(sortBy).ToList()
          : this.GetCollection(collectionName).Find(filter).SortBy(sortBy).ToList();
    }

    #endregion GetAll

    #region Pagination

    public int GetPageCount(string collectionName, int pageSize)
    {
      return Convert.ToInt32(
          Math.Ceiling(this.GetCollection(collectionName).Count(_ => true) * 1.0
                       / pageSize));
    }

    public int GetPageCount(string collectionName, int pageSize, Expression<Func<TDomain, bool>> filter)
    {
      return Convert.ToInt32(
          Math.Ceiling(this.GetCollection(collectionName).Count(filter) * 1.0
                       / pageSize));
    }

    public IEnumerable<TDomain> GetAllByPage(string collectionName, int pageIndex, int pageSize)
    {
      return this.GetCollection(collectionName).Find(_ => true).Skip(pageIndex * pageSize).Limit(pageSize)
          .ToList();
    }

    public IEnumerable<TDomain> GetAllByPageSortBy(string collectionName, int pageIndex, int pageSize,
        Expression<Func<TDomain, object>> sortBy, bool isSortByDescending = false)
    {
      return isSortByDescending
          ? this.GetCollection(collectionName).Find(_ => true).SortByDescending(sortBy).Skip(pageIndex * pageSize)
              .Limit(pageSize).ToList()
          : this.GetCollection(collectionName).Find(_ => true).SortBy(sortBy).Skip(pageIndex * pageSize)
              .Limit(pageSize).ToList();
    }

    public IEnumerable<TDomain> GetAllByPage(string collectionName, int pageIndex, int pageSize,
        Expression<Func<TDomain, bool>> filter)
    {
      return this.GetCollection(collectionName).Find(filter).Skip(pageIndex * pageSize).Limit(pageSize).ToList();
    }

    public IEnumerable<TDomain> GetAllByPageSortBy(string collectionName, int pageIndex, int pageSize,
        Expression<Func<TDomain, bool>> filter, Expression<Func<TDomain, object>> sortBy,
        bool isSortByDescending = false)
    {
      return isSortByDescending
          ? this.GetCollection(collectionName).Find(filter).SortByDescending(sortBy).Skip(pageIndex * pageSize)
              .Limit(pageSize).ToList()
          : this.GetCollection(collectionName).Find(filter).SortBy(sortBy).Skip(pageIndex * pageSize)
              .Limit(pageSize).ToList();
    }

    #endregion Pagination

    #region GetTop

    public IEnumerable<TDomain> GetTop(
        string collectionName,
        int resultNumber,
        Expression<Func<TDomain, object>> sortBy,
        bool isSortByDescending = false)
    {
      return this.GetAllByPageSortBy(collectionName, 0, resultNumber, sortBy, isSortByDescending);
    }

    public IEnumerable<TDomain> GetTop(
        string collectionName,
        double percent,
        Expression<Func<TDomain, object>> sortBy,
        bool isSortByDescending = false)
    {
      if (percent < 0d || percent > 1d)
      {
        throw new Exception("Please specific a valid percent value from 0 to 1, e.g. 0.45 (means 45%)");
      }
      var pageCount = this.GetCount(collectionName);
      var resultNumber = Convert.ToInt32(pageCount * percent);
      // Caution the loss of accuracy (long to int)
      return this.GetAllByPageSortBy(collectionName, 0, resultNumber, sortBy, isSortByDescending);
    }

    public IEnumerable<TDomain> GetTop(
        string collectionName,
        int resultNumber,
        Expression<Func<TDomain, bool>> filter,
        Expression<Func<TDomain, object>> sortBy,
        bool isSortByDescending = false)
    {
      return this.GetAllByPageSortBy(collectionName, 0, resultNumber, filter, sortBy, isSortByDescending);
    }

    public IEnumerable<TDomain> GetTop(
        string collectionName,
        double percent,
        Expression<Func<TDomain, bool>> filter,
        Expression<Func<TDomain, object>> sortBy,
        bool isSortByDescending = false)
    {
      if (percent < 0d || percent > 1d)
      {
        throw new Exception("Please specific a valid percent value from 0 to 1, e.g. 0.45 (means 45%)");
      }
      var pageCount = this.GetCount(collectionName, filter);
      var resultNumber = Convert.ToInt32(pageCount * percent);
      // Caution the loss of accuracy (long to int)
      return this.GetAllByPageSortBy(collectionName, 0, resultNumber, filter, sortBy, isSortByDescending);
    }

    #endregion GetTop

    #region CRUD

    public TDomain Get(string collectionName, string id)
    {
      return this.GetCollection(collectionName).Find(doc => doc.Id == id).FirstOrDefault();
    }

    public TDomain Get(string collectionName, Expression<Func<TDomain, bool>> filter)
    {
      return this.GetCollection(collectionName).Find(filter).FirstOrDefault();
    }

    public void Add(string collectionName, TDomain model)
    {
      this.GetCollection(collectionName).InsertOne(model);
    }

    public Task AddAsync(string collectionName, TDomain model)
    {
      return this.GetCollection(collectionName).InsertOneAsync(model);
    }

    public void AddMany(string collectionName, IEnumerable<TDomain> models)
    {
      this.GetCollection(collectionName).InsertMany(models);
    }

    public Task AddManyAsync(string collectionName, IEnumerable<TDomain> models)
    {
      return this.GetCollection(collectionName).InsertManyAsync(models);
    }

    public long Update(string collectionName, TDomain model)
    {
      var executeResult = this.GetCollection(collectionName).ReplaceOne(doc => doc.Id == model.Id, model);
      if (executeResult.IsModifiedCountAvailable)
      {
        return executeResult.ModifiedCount;
      }
      return -1;
    }

    public Task UpdateAsync(string collectionName, TDomain model)
    {
      return this.GetCollection(collectionName).ReplaceOneAsync(doc => doc.Id == model.Id, model);
    }

    public void UpdateMany(string collectionName, IEnumerable<TDomain> items)
    {
      var collection = this.GetCollection(collectionName);
      foreach (var item in items)
      {
        collection.ReplaceOne(doc => doc.Id == item.Id, item);
      }
    }

    public void UpdateManyAsync(string collectionName, IEnumerable<TDomain> items)
    {
      var collection = this.GetCollection(collectionName);
      foreach (var item in items)
      {
        collection.ReplaceOneAsync(doc => doc.Id == item.Id, item);
      }
    }

    public long Delete(string collectionName, string id)
    {
      return this.GetCollection(collectionName).DeleteOne(doc => doc.Id == id).DeletedCount;
    }

    public Task DeleteAsync(string collectionName, string id)
    {
      return this.GetCollection(collectionName).DeleteOneAsync(doc => doc.Id == id);
    }

    public long DeleteMany(string collectionName, Expression<Func<TDomain, bool>> filter)
    {
      return this.GetCollection(collectionName).DeleteMany(filter).DeletedCount;
    }

    public Task DeleteManyAsync(string collectionName, Expression<Func<TDomain, bool>> filter)
    {
      return this.GetCollection(collectionName).DeleteManyAsync(filter);
    }

    #endregion CRUD

    #region Utilities Operation

    protected IMongoCollection<TDomain> GetCollection(string collectionName)
    {
      if (string.IsNullOrWhiteSpace(collectionName))
      {
        return this.dataSelector.GetDataCollection<TDomain>();
      }
      return this.dataSelector.GetDataCollection<TDomain>(collectionName);
    }

    public IMongoQueryable<TDomain> AsQueryable(string collectionName)
    {
      return this.GetCollection(collectionName).AsQueryable();
    }

    public IMongoDatabase GetMongoDatabaseObject(string databaseName = "")
    {
      var database = MongoDBClient.Client.GetDatabase(string.IsNullOrWhiteSpace(databaseName)
          ? MongoDBClient.DefaultDatabaseName
          : databaseName);
      return database;
    }

    public bool Ping()
    {
      return this.GetMongoDatabaseObject().RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(1000);
    }

    public IEnumerable<string> GetAllDataCollectionsName(string targetDatabaseName)
    {
      return this.dataSelector.GetDataCollectionList(targetDatabaseName);
    }

    public void DropCollection(string collectionName)
    {
      this.GetCollection(collectionName).Database.DropCollection(collectionName);
    }

    #endregion Utilities Operation
  }
}
