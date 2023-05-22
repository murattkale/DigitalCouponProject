using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Net;
using System.Threading.Tasks;


public class ErrorMid
{
    private static readonly Serilog.ILogger _logger = Log.ForContext<ErrorMid>();
    private readonly RequestDelegate next;
    private readonly IWebHostEnvironment _HostEnvironment;

    public ErrorMid(RequestDelegate next, IWebHostEnvironment _HostEnvironment)
    {
        this.next = next;
        this._HostEnvironment = _HostEnvironment;
    }

    public async Task Invoke(HttpContext context /* other dependencies */)
    {
        try
        {
            await next(context);
        }
        catch (HttpStatusCodeException ex)
        {
            _logger.Error($"{DateTime.Now.ToString("HH:mm:ss")} : {ex}");
            HandleExceptionAsync(context, ex);
        }
        catch (Exception exceptionObj)
        {
            _logger.Error($"{DateTime.Now.ToString("HH:mm:ss")} : {exceptionObj}");
            HandleExceptionAsync(context, exceptionObj);
        }

    }

    private void HandleExceptionAsync(HttpContext context, HttpStatusCodeException exception)
    {
        string result = null;
        context.Response.ContentType = "application/json";
        if (exception is HttpStatusCodeException)
        {
            result = new ErrorDetails() { Message = exception.ToString(), StatusCode = (int)exception.StatusCode }.ToString();
            context.Response.StatusCode = (int)exception.StatusCode;
        }
        else
        {
            result = new ErrorDetails() { Message = exception.ToString(), StatusCode = (int)HttpStatusCode.BadRequest }.ToString();
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        if (_HostEnvironment.IsDevelopment())
        {
            context.Response.WriteAsync(result);
        }
        else
        {
            context.Response.HttpContext.Session.Set("error", result);
            context.Response.Redirect("/Base/Error");
        }

    }

    private void HandleExceptionAsync(HttpContext context, Exception exception)
    {
        string result = new ErrorDetails() { Message = exception.ToString() + "\n" + "\n" + exception.Message, StatusCode = (int)HttpStatusCode.InternalServerError }.ToString();
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        if (_HostEnvironment.IsDevelopment())
        {
            context.Response.WriteAsync(result);
        }
        else
        {
            context.Response.HttpContext.Session.Set("error", result);
            context.Response.Redirect("/Base/Error");
        }
    }
}
public class HttpStatusCodeException : Exception
{
    public HttpStatusCode StatusCode { get; set; }
    public string ContentType { get; set; } = @"text/plain";

    public HttpStatusCodeException(HttpStatusCode statusCode)
    {
        this.StatusCode = statusCode;
    }

    public HttpStatusCodeException(HttpStatusCode statusCode, string message) : base(message)
    {
        this.StatusCode = statusCode;
    }

    public HttpStatusCodeException(HttpStatusCode statusCode, Exception inner) : this(statusCode, inner.ToString()) { }

    public HttpStatusCodeException(HttpStatusCode statusCode, JObject errorObject) : this(statusCode, errorObject.ToString())
    {
        this.ContentType = @"application/json";
    }

}

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string Message { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}


