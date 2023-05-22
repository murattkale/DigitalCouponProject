using System;


public interface IUnitOfWork<C> : IDisposable
{
    RModel<C> SaveChanges();
}

public class UnitOfWork<C> : IUnitOfWork<C>
{
    private readonly myDBContext _context;

    public UnitOfWork(myDBContext _context)
    {
        this._context = _context;
    }
    public RModel<C> SaveChanges()
    {
        RModel<C> res = new RModel<C>();
        try
        {
            var save = _context.SaveChanges();
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



