using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;


public static class SessionExtensions
{
    public static void Set(this ISession session, string key, object value)
    {
        session.SetString(key, JsonConvert.SerializeObject(value, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
    }

    public static T Get<T>(this ISession session, string key)
    {
        var value = session.GetString(key);

        return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
    }

    //public static void Set(this IResponseCookies cookies, string key, object value)
    //{
    //    cookies.Append(key, value.ToJson(), new CookieOptions() { Expires = DateTimeOffset.Now.AddHours(12), HttpOnly = false, Path = "/", Secure = false });
    //}


}

public class SessionTimeout : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.Request.Path.Value.ExToLower().Contains("/login") && context.HttpContext.Request.Path.Value != "/" && SessionRequest._User == null)
        {
            context.Result =
                new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Base",
                    action = "Login"
                }));
        }

        base.OnActionExecuting(context);
    }
}

