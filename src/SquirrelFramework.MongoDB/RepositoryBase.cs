namespace SquirrelFramework.Repository
{
  #region using directives

  using Domain.Model;
  using MongoDB.Driver.Linq;
  using System;
  using System.Collections.Generic;
  using System.Linq.Expressions;
  using System.Threading.Tasks;

  #endregion using directives

  public abstract class RepositoryBase<TDomain> : CustomizedRepositoryBase<TDomain> where TDomain : DomainModel
  {
    #region GetNearBy

    //redius unit: Meters
    /// <summary>
    ///     Get Near by data
    /// </summary>
    /// <param name="centerPoint"></param>
    /// <param name="redius"></param>
    /// <returns></returns>
    public IEnumerable<TDomain> GetNearBy(Geolocation centerPoint, double redius /* Meter */)
    {
      return base.GetNearBy(null, centerPoint, redius);
    }

    public IEnumerable<TDomain> GetNearBy(
        string collectionName,
        Expression<Func<TDomain, object>> field,
        Geolocation centerPoint,
        double maxRadius /* Meter */,
        double minRadius /* Meter */)
    {
      return base.GetNearBy(null, field, centerPoint, maxRadius, minRadius);
    }

    #endregion GetNearBy

    #region GetCount

    public long GetCount()
    {
      return base.GetCount(null);
    }

    public long GetCount(Expression<Func<TDomain, bool>> filter)
    {
      return base.GetCount(null, filter);
    }

    #endregion GetCount

    #region GetAll

    public IEnumerable<TDomain> GetAll()
    {
      return base.GetAll(null);
    }

    public IEnumerable<TDomain> GetAll(Expression<Func<TDomain, object>> sortBy, bool isSortByDescending = false)
    {
      return base.GetAll(null, sortBy, isSortByDescending);
    }

    public IEnumerable<TDomain> GetAll(Expression<Func<TDomain, bool>> filter)
    {
      return base.GetAll(null, filter);
    }

    public IEnumerable<TDomain> GetAll(Expression<Func<TDomain, bool>> filter,
        Expression<Func<TDomain, object>> sortBy, bool isSortByDescending = false)
    {
      return base.GetAll(null, filter, sortBy, isSortByDescending);
    }

    #endregion GetAll

    #region Pagination

    public int GetPageCount(int pageSize)
    {
      return base.GetPageCount(null, pageSize);
    }

    public int GetPageCount(int pageSize, Expression<Func<TDomain, bool>> filter)
    {
      return base.GetPageCount(null, pageSize, filter);
    }

    public IEnumerable<TDomain> GetAllByPage(int pageIndex, int pageSize)
    {
      return base.GetAllByPage(null, pageIndex, pageSize);
    }

    public IEnumerable<TDomain> GetAllByPageSortBy(int pageIndex, int pageSize,
        Expression<Func<TDomain, object>> sortBy, bool isSortByDescending = false)
    {
      return base.GetAllByPageSortBy(null, pageIndex, pageSize, sortBy, isSortByDescending);
    }

    public IEnumerable<TDomain> GetAllByPage(int pageIndex, int pageSize,
        Expression<Func<TDomain, bool>> filter)
    {
      return base.GetAllByPage(null, pageIndex, pageSize, filter);
    }

    public IEnumerable<TDomain> GetAllByPageSortBy(int pageIndex, int pageSize,
        Expression<Func<TDomain, bool>> filter, Expression<Func<TDomain, object>> sortBy,
        bool isSortByDescending = false)
    {
      return base.GetAllByPageSortBy(null, pageIndex, pageSize, filter, sortBy);
    }

    #endregion Pagination

    #region GetTop

    public IEnumerable<TDomain> GetTop(
        int resultNumber,
        Expression<Func<TDomain, object>> sortBy,
        bool isSortByDescending = false)
    {
      return base.GetTop(
          null,
          resultNumber,
          sortBy,
          isSortByDescending);
    }

    public IEnumerable<TDomain> GetTop(
        double percent,
        Expression<Func<TDomain, object>> sortBy,
        bool isSortByDescending = false)
    {
      return base.GetTop(
          null,
          percent,
          sortBy,
          isSortByDescending);
    }

    public IEnumerable<TDomain> GetTop(
        int resultNumber,
        Expression<Func<TDomain, bool>> filter,
        Expression<Func<TDomain, object>> sortBy,
        bool isSortByDescending = false)
    {
      return base.GetTop(
          null,
          resultNumber,
          filter,
          sortBy,
          isSortByDescending);
    }

    public IEnumerable<TDomain> GetTop(
        double percent,
        Expression<Func<TDomain, bool>> filter,
        Expression<Func<TDomain, object>> sortBy,
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

    public TDomain Get(string id)
    {
      return base.Get(null, id);
    }

    public TDomain Get(Expression<Func<TDomain, bool>> filter)
    {
      return base.Get(null, filter);
    }

    public void Add(TDomain model)
    {
      base.Add(null, model);
    }

    public Task AddAsync(TDomain model)
    {
      return base.AddAsync(null, model);
    }

    public void AddMany(IEnumerable<TDomain> models)
    {
      base.AddMany(null, models);
    }

    public Task AddManyAsync(IEnumerable<TDomain> models)
    {
      return base.AddManyAsync(null, models);
    }

    public long Update(TDomain model)
    {
      return base.Update(null, model);
    }

    public Task UpdateAsync(TDomain model)
    {
      return base.UpdateAsync(null, model);
    }

    public void UpdateMany(IEnumerable<TDomain> items)
    {
      base.UpdateMany(null, items);
    }

    public void UpdateManyAsync(IEnumerable<TDomain> items)
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

    public long DeleteMany(Expression<Func<TDomain, bool>> filter)
    {
      return base.DeleteMany(null, filter);
    }

    public Task DeleteManyAsync(Expression<Func<TDomain, bool>> filter)
    {
      return base.DeleteManyAsync(null, filter);
    }

    #endregion CRUD

    #region Utilities Operation

    public IMongoQueryable<TDomain> AsQueryable()
    {
      return base.AsQueryable(null);
    }

    #endregion Utilities Operation
  }
}