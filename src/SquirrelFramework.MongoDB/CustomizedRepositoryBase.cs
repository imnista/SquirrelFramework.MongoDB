using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SquirrelFramework.Model;
using SquirrelFramework.Repository.Internal;
using SquirrelFramework.Utility.Coding;
using System.Linq.Expressions;

namespace SquirrelFramework.Repository
{
    public abstract class CustomizedRepositoryBase<TDocument> where TDocument : Document
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
            // BUT when registering the sub class of the Document,
            // the BsonClassMap gets the fields from the Type.GetFields(),
            // and it only contains the fields of itself,
            // refer to https://stackoverflow.com/questions/9201859/why-doesnt-type-getfields-return-backing-fields-in-a-base-class
            // but not the fields of its base class.
            // So it means that we must register the base class (Document) either.
            // If we register both the sub model and the base model,
            // that would automatically add a new database field "_t" for recording
            // the inheritance relationship between base class and the sub-model class.
            // However, the storage and execution cost will increase.
            // So we disable this feature.
            /*
            var type = typeof(TDocument);
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
            dataSelector = new DataCollectionSelector(MongoDBClient.Client, MongoDBClient.DefaultDatabaseName);
        }

        #region GetNearBy

        //redius unit: Meters
        public IEnumerable<TDocument> GetNearBy(
            string? collectionName,
            Geolocation centerPoint,
            double radius /* Meter */)
        {
            var collection = GetCollection(collectionName);

            // 建立空间搜索 索引
            // collection.Indexes.List()
            // [Obsolete]
            // collection.Indexes.CreateOneAsync(Builders<TDocument>.IndexKeys.Geo2DSphere(m => m.Geolocation)).Wait();
            var createIndexModel = new CreateIndexModel<TDocument>(
                Builders<TDocument>.IndexKeys.Geo2DSphere(m => m.Geolocation));
            collection.Indexes.CreateOneAsync(createIndexModel).Wait();

            // 空间搜索算法
            var rangeInKm = radius / 1000; /* KM */
            var radians = rangeInKm / EarthRadius;

            var filter = Builders<TDocument>.Filter.NearSphere(m => m.Geolocation, centerPoint.Longitude,
                centerPoint.Latitude,
                radians);
            return collection.Find(filter).ToList();
        }

        public IEnumerable<TDocument> GetNearBy(
            string? collectionName,
            Expression<Func<TDocument, object>> field,
            Geolocation centerPoint,
            double maxRadius /* Meter */,
            double minRadius /* Meter */)
        {
            var collection = GetCollection(collectionName);

            // TODO: 首先检查是不是已经有了这个索引！
            // https://stackoverflow.com/questions/35019313/checking-if-an-index-exists-in-mongodb
            var geo2DSphereIndexKeyName =
                $"{collectionName}_{LambdaExpressionHelper.RetrievePropertyNameByLambdaExpression(field)}";

            // [Obsolete]
            // collection.Indexes.CreateOneAsync(Builders<TDocument>.IndexKeys.Geo2DSphere(field), new CreateIndexOptions
            // {
            //     Name = geo2DSphereIndexKeyName
            // }).Wait();
            // 建立空间搜索 索引
            var createIndexModel = new CreateIndexModel<TDocument>(
                Builders<TDocument>.IndexKeys.Geo2DSphere(field),
                new CreateIndexOptions
                {
                    Name = geo2DSphereIndexKeyName
                });
            collection.Indexes.CreateOneAsync(createIndexModel).Wait();

            // 空间搜索算法
            var maxDistanceRangeInKm = maxRadius / 1000; /* KM */
            var maxDistanceRadians = maxDistanceRangeInKm / EarthRadius;

            var minDistanceRangeInKm = minRadius / 1000; /* KM */
            var minDistanceRadians = minDistanceRangeInKm / EarthRadius;

            // Add Filter
            var filter = Builders<TDocument>.Filter.NearSphere(
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

        public long? GetCount(string? collectionName)
        {
            // Differences between mongodb EstimatedDocumentCount() and CountDocuments()
            // https://programmer.ink/think/differences-between-mongodb-estimateddocumentcount-and-countdocuments.html
            // The countDocuments are counted by sum, which is suitable for accurate conditions,
            // and the count instruction is suitable for collecting the overall count.
            return GetCollection(collectionName)?.EstimatedDocumentCount();
        }

        public long? GetCount(string? collectionName, Expression<Func<TDocument, bool>> filter)
        {
            return GetCollection(collectionName)?.CountDocuments(filter);
        }

        #endregion GetCount

        #region GetAll

        public IEnumerable<TDocument> GetAll(string? collectionName)
        {
            return GetCollection(collectionName).Find(_ => true).ToList();
        }

        public IEnumerable<TDocument> GetAll(
            string? collectionName,
            Expression<Func<TDocument, object>> sortBy,
            bool isSortByDescending = false)
        {
            return isSortByDescending
                ? GetCollection(collectionName).Find(_ => true).SortByDescending(sortBy).ToList()
                : GetCollection(collectionName).Find(_ => true).SortBy(sortBy).ToList();
        }

        public IEnumerable<TDocument> GetAll(
            string? collectionName,
            Expression<Func<TDocument, bool>> filter)
        {
            return GetCollection(collectionName).Find(filter).ToList();
        }

        public IEnumerable<TDocument> GetAll(
            string? collectionName,
            Expression<Func<TDocument, bool>> filter,
            Expression<Func<TDocument, object>> sortBy,
            bool isSortByDescending = false)
        {
            return isSortByDescending
                ? GetCollection(collectionName).Find(filter).SortByDescending(sortBy).ToList()
                : GetCollection(collectionName).Find(filter).SortBy(sortBy).ToList();
        }

        #endregion GetAll

        #region Pagination

        public int GetPageCount(string? collectionName, int pageSize)
        {
            return Convert.ToInt32(
                Math.Ceiling(GetCollection(collectionName).EstimatedDocumentCount() * 1.0
                             / pageSize));
        }

        public int GetPageCount(string? collectionName, int pageSize, Expression<Func<TDocument, bool>> filter)
        {
            return Convert.ToInt32(
                Math.Ceiling(GetCollection(collectionName).CountDocuments(filter) * 1.0
                             / pageSize));
        }

        public IEnumerable<TDocument> GetAllByPage(string? collectionName, int pageIndex, int pageSize)
        {
            return GetCollection(collectionName)
                .Find(_ => true)
                .Skip(pageIndex * pageSize)
                .Limit(pageSize)
                .ToList();
        }

        public IEnumerable<TDocument> GetAllByPageSortBy(
            string? collectionName,
            int pageIndex,
            int pageSize,
            Expression<Func<TDocument, object>> sortBy,
            bool isSortByDescending = false)
        {
            return isSortByDescending
                ? GetCollection(collectionName).Find(_ => true).SortByDescending(sortBy).Skip(pageIndex * pageSize)
                    .Limit(pageSize).ToList()
                : GetCollection(collectionName).Find(_ => true).SortBy(sortBy).Skip(pageIndex * pageSize)
                    .Limit(pageSize).ToList();
        }

        public IEnumerable<TDocument> GetAllByPage(
            string? collectionName,
            int pageIndex,
            int pageSize,
            Expression<Func<TDocument, bool>> filter)
        {
            return GetCollection(collectionName).Find(filter).Skip(pageIndex * pageSize).Limit(pageSize).ToList();
        }

        public IEnumerable<TDocument> GetAllByPageSortBy(
            string? collectionName,
            int pageIndex,
            int pageSize,
            Expression<Func<TDocument, bool>> filter,
            Expression<Func<TDocument, object>> sortBy,
            bool isSortByDescending = false)
        {
            return isSortByDescending
                ? GetCollection(collectionName).Find(filter).SortByDescending(sortBy).Skip(pageIndex * pageSize)
                    .Limit(pageSize).ToList()
                : GetCollection(collectionName).Find(filter).SortBy(sortBy).Skip(pageIndex * pageSize)
                    .Limit(pageSize).ToList();
        }

        #endregion Pagination

        #region GetTop

        public IEnumerable<TDocument> GetTop(
            string? collectionName,
            int resultNumber,
            Expression<Func<TDocument, object>> sortBy,
            bool isSortByDescending = false)
        {
            return GetAllByPageSortBy(collectionName, 0, resultNumber, sortBy, isSortByDescending);
        }

        public IEnumerable<TDocument> GetTop(
            string? collectionName,
            double percent,
            Expression<Func<TDocument, object>> sortBy,
            bool isSortByDescending = false)
        {
            if (percent < 0d || percent > 1d)
            {
                throw new Exception("Please specific a valid percent value from 0 to 1, e.g. 0.45 (means 45%)");
            }
            var pageCount = GetCount(collectionName);
            var resultNumber = Convert.ToInt32(pageCount * percent);
            // Caution the loss of accuracy (long to int)
            return GetAllByPageSortBy(collectionName, 0, resultNumber, sortBy, isSortByDescending);
        }

        public IEnumerable<TDocument> GetTop(
            string? collectionName,
            int resultNumber,
            Expression<Func<TDocument, bool>> filter,
            Expression<Func<TDocument, object>> sortBy,
            bool isSortByDescending = false)
        {
            return GetAllByPageSortBy(collectionName, 0, resultNumber, filter, sortBy, isSortByDescending);
        }

        public IEnumerable<TDocument> GetTop(
            string? collectionName,
            double percent,
            Expression<Func<TDocument, bool>> filter,
            Expression<Func<TDocument, object>> sortBy,
            bool isSortByDescending = false)
        {
            if (percent < 0d || percent > 1d)
            {
                throw new Exception("Please specific a valid percent value from 0 to 1, e.g. 0.45 (means 45%)");
            }
            var pageCount = GetCount(collectionName, filter);
            var resultNumber = Convert.ToInt32(pageCount * percent);
            // Caution the loss of accuracy (long to int)
            return GetAllByPageSortBy(collectionName, 0, resultNumber, filter, sortBy, isSortByDescending);
        }

        #endregion GetTop

        #region CRUD

        public TDocument Get(string? collectionName, string id)
        {
            return GetCollection(collectionName).Find(doc => doc.Id == id).FirstOrDefault();
        }

        public TDocument Get(string? collectionName, Expression<Func<TDocument, bool>> filter)
        {
            return GetCollection(collectionName).Find(filter).FirstOrDefault();
        }

        public void Add(string? collectionName, TDocument model)
        {
            GetCollection(collectionName).InsertOne(model);
        }

        public Task AddAsync(string? collectionName, TDocument model)
        {
            return GetCollection(collectionName).InsertOneAsync(model);
        }

        public void AddMany(string? collectionName, IEnumerable<TDocument> models)
        {
            GetCollection(collectionName).InsertMany(models);
        }

        public Task AddManyAsync(string? collectionName, IEnumerable<TDocument> models)
        {
            return GetCollection(collectionName).InsertManyAsync(models);
        }

        public long Update(string? collectionName, TDocument model)
        {
            var executeResult = GetCollection(collectionName).ReplaceOne(doc => doc.Id == model.Id, model);
            if (executeResult.IsModifiedCountAvailable)
            {
                return executeResult.ModifiedCount;
            }
            return -1;
        }

        public Task UpdateAsync(string? collectionName, TDocument model)
        {
            return GetCollection(collectionName).ReplaceOneAsync(doc => doc.Id == model.Id, model);
        }

        public void UpdateMany(string? collectionName, IEnumerable<TDocument> items)
        {
            var collection = GetCollection(collectionName);
            foreach (var item in items)
            {
                collection.ReplaceOne(doc => doc.Id == item.Id, item);
            }
        }

        public void UpdateManyAsync(string? collectionName, IEnumerable<TDocument> items)
        {
            var collection = GetCollection(collectionName);
            foreach (var item in items)
            {
                collection.ReplaceOneAsync(doc => doc.Id == item.Id, item);
            }
        }

        public long Delete(string? collectionName, string id)
        {
            return GetCollection(collectionName).DeleteOne(doc => doc.Id == id).DeletedCount;
        }

        public Task DeleteAsync(string? collectionName, string id)
        {
            return GetCollection(collectionName).DeleteOneAsync(doc => doc.Id == id);
        }

        public long DeleteMany(string? collectionName, Expression<Func<TDocument, bool>> filter)
        {
            return GetCollection(collectionName).DeleteMany(filter).DeletedCount;
        }

        public Task DeleteManyAsync(string? collectionName, Expression<Func<TDocument, bool>> filter)
        {
            return GetCollection(collectionName).DeleteManyAsync(filter);
        }

        #endregion CRUD

        #region Utilities Operation

        public string GenerateNewId()
        {
            return ObjectId.GenerateNewId().ToString();
        }

        protected IMongoCollection<TDocument> GetCollection(string? collectionName)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
            {
                return dataSelector.GetDataCollection<TDocument>();
            }
            return dataSelector.GetDataCollection<TDocument>(collectionName);
        }

        public IMongoQueryable<TDocument> AsQueryable(string? collectionName)
        {
            return GetCollection(collectionName).AsQueryable();
        }

        public IMongoDatabase GetMongoDatabaseObject(string? databaseName = "")
        {
            var database = MongoDBClient.Client.GetDatabase(string.IsNullOrWhiteSpace(databaseName)
                ? MongoDBClient.DefaultDatabaseName
                : databaseName);
            return database;
        }

        public bool Ping()
        {
            return GetMongoDatabaseObject().RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(1000);
        }

        public IEnumerable<string> GetAllDataCollectionsName(string targetDatabaseName)
        {
            return dataSelector.GetDataCollectionList(targetDatabaseName);
        }

        public void DropCollection(string? collectionName)
        {
            var collection = GetCollection(collectionName);
            collectionName = collection.CollectionNamespace.CollectionName;
            collection.Database.DropCollection(collectionName);
        }

        #endregion Utilities Operation
    }
}