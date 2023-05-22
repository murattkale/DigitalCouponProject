using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        [Route("error")]
        public ActionResult Error()
        {
            var err = new ErrorType();
            err.code = $"{Response.StatusCode}";

            switch (Response.StatusCode)
            {
                case 404: err.message = "Not Found"; break;
                default: break;
            }

            var exp = HttpContext.Features.Get<IExceptionHandlerPathFeature>()?.Error;
            if (exp != null)
            {
                err.message = exp.Message;

                if (exp.Data.Contains("code"))
                {
                    err.code = (string)exp.Data["code"];
                }

                if (exp.Data.Contains("message"))
                {
                    err.message = (string)exp.Data["message"];
                }

                if (exp.Data.Contains("values"))
                {
                    err.values = (object[])exp.Data["values"];
                }

                if (exp.Data.Contains("err"))
                {
                    var err_type = exp.Data["err"] as ErrorType;
                    if (err_type != null)
                    {
                        err.code = err_type.code;
                        err.message = err_type.message;
                        err.values = err_type.values;
                    }
                }
            }

            return StatusCode(Response.StatusCode, err);
        }
    }
}
