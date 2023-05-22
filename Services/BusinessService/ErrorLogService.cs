public interface IErrorLogService : IGenericRepo<ErrorLog> { }
public class ErrorLogService : GenericRepo<myDBContext, ErrorLog>, IErrorLogService
{

    public ErrorLogService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

