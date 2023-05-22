
using Microsoft.AspNetCore.Http;


public class BaseSession : IBaseSession
{
    IHttpContextAccessor _IHttpContextAccessor;
    public BaseSession(IHttpContextAccessor __IHttpContextAccessor)
    {
        _IHttpContextAccessor = __IHttpContextAccessor;
    }
    public BaseModel _BaseModel
    {
        get
        {
            var val = _IHttpContextAccessor.HttpContext.Session.Get<User>("_user");
            return val;
        }
        set
        {
            this._BaseModel = value;
        }
    }

}
