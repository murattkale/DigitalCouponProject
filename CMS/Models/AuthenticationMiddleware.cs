using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;


public class AuthenticationMiddleware
{
    RequestDelegate _next;
    IHttpContextAccessor _IHttpContextAccessor;

    public AuthenticationMiddleware(
        RequestDelegate next,
        IHttpContextAccessor _IHttpContextAccessor
        )
    {
        this._next = next;
        this._IHttpContextAccessor = _IHttpContextAccessor;
    }

    public Task Invoke(HttpContext httpContext)
    {
        if (SessionRequest._User == null)
        {
            httpContext.Response.Redirect("/Login");
            //_IHttpContextAccessor.HttpContext.Session.Set("_user", _user);
        }
        else
        {

        }

        return _next(httpContext);
    }
}

public static class AuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthenticationMiddleware>();
    }
}
