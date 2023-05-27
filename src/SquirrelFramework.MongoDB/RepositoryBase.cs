using MongoDB.Driver.Linq;
using SquirrelFramework.Model;
using System.Linq.Expressions;

namespace SquirrelFramework.Repository
{
    public abstract class RepositoryBase<TDocument> : CustomizedRepositoryBase<TDocument> where TDocument : Document
    {
        #region GetNearBy

        /// <summary>
        /// Get Near by data
        /// </summary>
        /// <param name="centerPoint"></param>
        /// <param name="redius">Unit: Meters</param>
        /// <returns></returns>
        public IEnumerable<TDocument> GetNearBy(
            Geolocation centerPoint,
            double redius /* Meter */)
        {
            return base.GetNearBy(null, centerPoint, redius);
        }

        public IEnumerable<TDocument> GetNearBy(
            Expression<Func<TDocument, object>> field,
            Geolocation centerPoint,
            double maxRadius /* Meter */,
            double minRadius /* Meter */)
        {
            return base.GetNearBy(null, field, centerPoint, maxRadius, minRadius);
        }

        #endregion GetNearBy

        #region GetCount

        public long? GetCount()
        {
            return base.GetCount(null);
        }

        public long? GetCount(Expression<Func<TDocument, bool>> filter)
        {
            return base.GetCount(null, filter);
        }

        #endregion GetCount

        #region GetAll

        public IEnumerable<TDocument> GetAll()
        {
            return base.GetAll(null);
        }

        public IEnumerable<TDocument> GetAll(Expression<Func<TDocument, object>> sortBy, bool isSortByDescending = false)
        {
            return base.GetAll(null, sortBy, isSortByDescending);
        }

        public IEnumerable<TDocument> GetAll(Expression<Func<TDocument, bool>> filter)
        {
            return base.GetAll(null, filter);
        }

        public IEnumerable<TDocument> GetAll(Expression<Func<TDocument, bool>> filter,
            Expression<Func<TDocument, object>> sortBy, bool isSortByDescending = false)
        {
            return base.GetAll(null, filter, sortBy, isSortByDescending);
        }

        #endregion GetAll

        #region Pagination

        public long GetPageCount(int pageSize)
        {
            return base.GetPageCount(null, pageSize);
        }

        public int GetPageCount(int pageSize, Expression<Func<TDocument, bool>> filter)
        {
            return base.GetPageCount(null, pageSize, filter);
        }

        public IEnumerable<TDocument> GetAllByPage(int pageIndex, int pageSize)
        {
            return base.GetAllByPage(null, pageIndex, pageSize);
        }

        public IEnumerable<TDocument> GetAllByPageSortBy(int pageIndex, int pageSize,
            Expression<Func<TDocument, object>> sortBy, bool isSortByDescending = false)
        {
            return base.GetAllByPageSortBy(null, pageIndex, pageSize, sortBy, isSortByDescending);
        }

        public IEnumerable<TDocument> GetAllByPage(int pageIndex, int pageSize,
            Expression<Func<TDocument, bool>> filter)
        {
            return base.GetAllByPage(null, pageIndex, pageSize, filter);
        }

        public IEnumerable<TDocument> GetAllByPageSortBy(int pageIndex, int pageSize,
            Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, object>> sortBy,
            bool isSortByDescending = false)
        {
            return base.GetAllByPageSortBy(null, pageIndex, pageSize, filter, sortBy);
        }

        #endregion Pagination

        #region GetTop

        public IEnumerable<TDocument> GetTop(
            int resultNumber,
            Expression<Func<TDocument, object>> sortBy,
            bool isSortByDescending = false)
        {
            return base.GetTop(
                null,
                resultNumber,
                sortBy,
                isSortByDescending);
        }

        public IEnumerable<TDocument> GetTop(
            double percent,
            Expression<Func<TDocument, object>> sortBy,
            bool isSortByDescending = false)
        {
            return base.GetTop(
                null,
                percent,
                sortBy,
                isSortByDescending);
        }

        public IEnumerable<TDocument> GetTop(
            int resultNumber,
            Expression<Func<TDocument, bool>> filter,
            Expression<Func<TDocument, object>> sortBy,
            bool isSortByDescending = false)
        {
            return base.GetTop(
                null,
                resultNumber,
                filter,
                sortBy,
                isSortByDescending);
        }

        public IEnumerable<TDocument> GetTop(
            double percent,
            Expression<Func<TDocument, bool>> filter,
            Expression<Func<TDocument, object>> sortBy,
            bool isSortByDescending = false)
        {
            return base.GetTop(
                null,
                percent,
                filter,
                sortBy,
                isSortByDescending);
        }

        #endregion GetTop

        #region CRUD

        public TDocument Get(string id)
        {
            return base.Get(null, id);
        }

        public TDocument Get(Expression<Func<TDocument, bool>> filter)
        {
            return base.Get(null, filter);
        }

        public void Add(TDocument model)
        {
            base.Add(null, model);
        }

        public Task AddAsync(TDocument model)
        {
            return base.AddAsync(null, model);
        }

        public void AddMany(IEnumerable<TDocument> models)
        {
            base.AddMany(null, models);
        }

        public Task AddManyAsync(IEnumerable<TDocument> models)
        {
            return base.AddManyAsync(null, models);
        }

        public long Update(TDocument model)
        {
            return base.Update(null, model);
        }

        public Task UpdateAsync(TDocument model)
        {
            return base.UpdateAsync(null, model);
        }

        public void UpdateMany(IEnumerable<TDocument> items)
        {
            base.UpdateMany(null, items);
        }

        public void UpdateManyAsync(IEnumerable<TDocument> items)
        {
            base.UpdateManyAsync(null, items);
        }

        public long Delete(string id)
        {
            return base.Delete(null, id);
        }

        public Task DeleteAsync(string id)
        {
            return base.DeleteAsync(null, id);
        }

        public long DeleteMany(Expression<Func<TDocument, bool>> filter)
        {
            return base.DeleteMany(null, filter);
        }

        public Task DeleteManyAsync(Expression<Func<TDocument, bool>> filter)
        {
            return base.DeleteManyAsync(null, filter);
        }

        #endregion CRUD

        #region Utilities Operation

        public IMongoQueryable<TDocument> AsQueryable()
        {
            return base.AsQueryable(null);
        }

        public void DropCollection()
        {
            base.DropCollection(null);
        }

        #endregion Utilities Operation
    }
}