using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

public interface IGenericRepo<T> where T : class
{
    RModel<T> InsertOrUpdate(T model);
    RModel<T> GetPaging(
        Expression<Func<T, bool>> filter = null
       , bool AsNoTracking = true
       , DTParameters<T> param = null
       , bool IsDeletedShow = false
       , params Expression<Func<T, object>>[] includes
       );


    RModel<T> Where(
          Expression<Func<T, bool>> filter = null
          , bool AsNoTracking = true
          , bool IsDeletedShow = false
          , params Expression<Func<T, object>>[] includes
          );
    RModel<T> WhereList(
         Expression<Func<T, bool>> filter = null
         , bool AsNoTracking = true
         , bool IsDeletedShow = false
         , params Expression<Func<T, object>>[] includes
         );
    RModel<T> Get(
         Expression<Func<T, bool>> filter = null
         , bool AsNoTracking = true
         , bool IsDeletedShow = false
         , params Expression<Func<T, object>>[] includes
         );

    RModel<T> Add(T t);
    RModel<T> Delete(int id);
    RModel<T> DeleteBulk(List<T> tList);
    RModel<T> Delete(T t);
    RModel<T> Update(T t);
    void Dispose();

}

public class GenericRepo<C, T> : IGenericRepo<T> where T : class, IBaseModel where C : DbContext, new()
{

    private C _context = new C();
    public C Context
    {

        get { return _context; }
        set { _context = value; }
    }


    protected IBaseModel sessionInfo;

    public GenericRepo(C _context, IBaseModel sessionInfo)
    {
        this.sessionInfo = sessionInfo;
        this._context = _context;
    }


    public RModel<T> InsertOrUpdate(T model)
    {
        RModel<T> res = new RModel<T>();
        if (model.Id > 0)
            res = Update(model);
        else
            res = Add(model);



        return res;
    }




    public RModel<T> GetPaging(
        Expression<Func<T, bool>> filter = null
       , bool AsNoTracking = true
       , DTParameters<T> param = null
       , bool IsDeletedShow = false
       , params Expression<Func<T, object>>[] includes
       )
    {

        RModel<T> res = new RModel<T>();
        try
        {

            var query = Where(filter, AsNoTracking, IsDeletedShow, includes).Result;

            var GlobalSearchFilteredData = query.ToGlobalSearchInAllColumn<T>(param);
            var IndividualColSearchFilteredData = GlobalSearchFilteredData.ToIndividualColumnSearch(param);
            var SortedFilteredData = IndividualColSearchFilteredData.ToSorting(param);
            var SortedData = SortedFilteredData.ToPagination(param);

            var rSortedData = SortedData.ToList();

            int Count = query.Count();

            var resultData = new DTResult<T>
            {
                draw = param.Draw,
                data = rSortedData,
                recordsFiltered = Count,
                recordsTotal = Count,
                selectedItem = param.Start + 1
            };

            res.ResultPaging = resultData;
            res.RType = RType.OK;

        }
        catch (Exception ex)
        {
            res.Message = ex?.InnerException?.Message + "\n" + "\n" + ex?.Message + "\n" + "\n" + ex?.StackTrace;
            res.Ex = ex;
            res.RType = RType.Error;
        }

        return res;
    }


    public RModel<T> WhereList(
        Expression<Func<T, bool>> filter = null
        , bool AsNoTracking = true
        , bool IsDeletedShow = false
        , params Expression<Func<T, object>>[] includes
        )
    {
        RModel<T> res = new RModel<T>();
        var result = Where(filter, AsNoTracking, IsDeletedShow, includes);
        res.ResultList = result.Result.ToList();
        res.Result = null;
        res.RType = result.RType;
        res.Message = result.Message;
        res.MessageList = result.MessageList;
        res.MessageListJson = result.MessageListJson;
        res.RedirectUrl = result.RedirectUrl;
        res.Ex = result.Ex;


        return res;
    }

    public RModel<T> Where(
       Expression<Func<T, bool>> filter = null
       , bool AsNoTracking = true
       , bool IsDeletedShow = false
       , params Expression<Func<T, object>>[] includes
       )
    {
        RModel<T> res = new RModel<T>();
        try
        {
            var query = _context.Set<T>() as IQueryable<T>;

            if (AsNoTracking)
                query = query.AsNoTracking();

            if (IsDeletedShow)
                query = query.IgnoreQueryFilters(); //Disable global query filters

            if (filter != null)
                query = query.Where(filter);

            //if (sessionInfo.LanguageId > 0 && (query.ElementType.GetProperties().Any(o => o.Name == "LangId")))
            //    query = query.Where("LangId==" + sessionInfo.LanguageId.ToStr());


            query = query.OrderBy(o => o.OrderNo).AsQueryable();

            if (includes != null && includes.Count() > 0)
            {
                if (IsDeletedShow)
                {
                    if (AsNoTracking)
                        query = includes.Aggregate(query, (current, include) => current.AsNoTracking().Include(include).IgnoreQueryFilters());
                    else
                        query = includes.Aggregate(query, (current, include) => current.Include(include).IgnoreQueryFilters());
                }
                else
                {
                    if (AsNoTracking)
                        query = includes.Aggregate(query, (current, include) => current.AsNoTracking().Include(include));
                    else
                        query = includes.Aggregate(query, (current, include) => current.Include(include));
                }

            }

            res.SqlQuery = query.ToQueryString();
            res.Result = query;
            res.RType = RType.OK;
        }
        catch (Exception ex)
        {
            res.Message = ex?.InnerException?.Message + "\n" + "\n" + ex?.Message + "\n" + "\n" + ex?.StackTrace;
            res.Ex = ex;
            res.RType = RType.Error;
        }
        return res;
    }


    public RModel<T> Get(
       Expression<Func<T, bool>> filter = null
       , bool AsNoTracking = true
       , bool IsDeletedShow = false
       , params Expression<Func<T, object>>[] includes
       )
    {
        RModel<T> res = new RModel<T>();
        try
        {
            var result = Where(filter, AsNoTracking, IsDeletedShow, includes);
            res.RType = result.RType;
            res.Message = result.Message;
            res.MessageList = result.MessageList;
            res.MessageListJson = result.MessageListJson;
            res.Ex = result.Ex;
            if (result.RType == RType.OK)
            {
                //res.SqlQuery = result.SqlQuery;
                res.ResultRow = result.Result.FirstOrDefault();
            }

        }
        catch (Exception ex)
        {
            res.Message = ex?.InnerException?.Message + "\n" + "\n" + ex?.Message + "\n" + "\n" + ex?.StackTrace;
            res.Ex = ex;
            res.RType = RType.Error;
        }
        return res;
    }



    public RModel<T> Add(T t)
    {
        RModel<T> res = new RModel<T>();
        try
        {
            t.CreaUser = sessionInfo.CreaUser;
            t.CreaDate = DateTime.Now;
            _context.Set<T>().Add(t);
            res.ResultRow = t;
            res.RType = RType.OK;
        }
        catch (Exception ex)
        {
            res.Message = ex?.InnerException?.Message + "\n" + "\n" + ex?.Message + "\n" + "\n" + ex?.StackTrace;
            res.Ex = ex;
            res.RType = RType.Error;
        }

        return res;
    }
    public RModel<T> Delete(int id)
    {
        RModel<T> res = new RModel<T>();
        try
        {
            var t = Where(o => o.Id == id).Result.FirstOrDefault();
            t.IsDeleted = DateTime.Now;
            var upd = Update(t);
            res.ResultRow = t;
            res.RType = RType.OK;
        }
        catch (Exception ex)
        {
            res.Message = ex?.InnerException?.Message + "\n" + "\n" + ex?.Message + "\n" + "\n" + ex?.StackTrace;
            res.Ex = ex;
            res.RType = RType.Error;
        }
        return res;
    }
    public RModel<T> Delete(T t)
    {
        RModel<T> res = new RModel<T>();
        try
        {
            t.IsDeleted = DateTime.Now;
            var upd = Update(t);
            res.ResultRow = t;
            res.RType = RType.OK;
        }
        catch (Exception ex)
        {
            res.Message = ex?.InnerException?.Message + "\n" + "\n" + ex?.Message + "\n" + "\n" + ex?.StackTrace;
            res.Ex = ex;
            res.RType = RType.Error;
        }

        return res;
    }

    public RModel<T> DeleteBulk(List<T> tList)
    {
        RModel<T> res = new RModel<T>();

        tList.ForEach(o =>
        {
            try
            {
                var delete = Delete(o);
                res.ResultList.Add(delete.ResultRow);
                res.RTypeList.Add(delete.RType);
            }
            catch (Exception ex)
            {
                res.Message = ex?.InnerException?.Message + "\n" + "\n" + ex?.Message + "\n" + "\n" + ex?.StackTrace;
                res.Ex = ex;
                res.RType = RType.Error;
            }
        });

        return res;
    }
    public RModel<T> Update(T t)
    {
        RModel<T> res = new RModel<T>();
        try
        {
            var dbEntityEntry = _context.Entry(t);
            dbEntityEntry.State = EntityState.Modified;
            dbEntityEntry.Property(o => o.CreaDate).IsModified = false;
            dbEntityEntry.Property(o => o.CreaUser).IsModified = false;
            t.ModUser = sessionInfo.CreaUser;
            t.ModDate = DateTime.Now;
            res.ResultRow = t;
            res.RType = RType.OK;
        }
        catch (Exception ex)
        {
            res.Message = ex?.InnerException?.Message + "\n" + "\n" + ex?.Message + "\n" + "\n" + ex?.StackTrace;
            res.Ex = ex;
            res.RType = RType.Error;
        }
        return res;
    }
    private bool disposed = false;
    protected void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            this.disposed = true;
        }
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}


