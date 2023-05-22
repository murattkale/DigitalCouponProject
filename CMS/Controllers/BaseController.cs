using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Controllers
{
    public class BaseController : Controller
    {
        ISendMail _ISendMail;
        IBaseModel _IBaseModel;
        IHostingEnvironment _IHostingEnvironment;
        //IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;

        public BaseController(
            ISendMail _ISendMail,
           IBaseModel _IBaseModel,
            IHostingEnvironment _IHostingEnvironment,
            //IHttpContextAccessor _IHttpContextAccessor,
            IHttpClientWrapper _client
            )
        {

            this._ISendMail = _ISendMail;
            this._IBaseModel = _IBaseModel;
            this._IHostingEnvironment = _IHostingEnvironment;
            //this._IHttpContextAccessor = _IHttpContextAccessor;
            this._client = _client;
        }






        public async Task<IActionResult> getDtLang()
        {
            var LangDt = new
            {
                sDecimal = ",",
                sEmptyTable = ("Table Data not fpund".Trans()),
                sInfo = ("_TOTAL_ rows _START_ - _END_ tange row display".Trans()),
                sInfoEmpty = ("Data not found".Trans()),
                sInfoFiltered = ("(_MAX_ row filter)".Trans()),
                sInfoPostFix = "",
                sInfoThousands = ".",
                sLengthMenu = ("Page _MENU_ row display".Trans()),
                sLoadingRecords = ("Loading".Trans()) + "...",
                sProcessing = ("Process".Trans()) + "...",
                sSearch = ("Search".Trans()),
                sZeroRecords = ("Data not found".Trans()),
                oPaginate = new
                {
                    sFirst = ("First".Trans()),
                    sLast = ("Last".Trans()),
                    sNext = ("Next".Trans()),
                    sPrevious = ("Prew".Trans())
                },
                oAria = new
                {
                    sSortAscending = ("Sort ASC".Trans()),
                    sSortDescending = ("Sort DESC".Trans())
                }
            };

            return Json(LangDt);
        }


        public async Task<IActionResult> Error()
        {
            await GetConfLang();

            var result = await _client.GetAsync<SiteConfig>("SiteConfig/GetBy");
            HttpContext.Session.Set("config", result.ResultRow);

            return View();
        }

        //[SessionTimeout]
        [Route("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            await GetConfLang();
            return View();
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Base");
        }
        public IActionResult Location()
        {
            return View();
        }



        //[SessionTimeout]
        //[Route("Login")]
        public async Task<IActionResult> Login()
        {
            //var browserLang = Request.Headers["Accept-Language"].ToString().Split(";").FirstOrDefault()?.Split(",").FirstOrDefault().ExToLower().Contains("tr") == true ? 1 : 2;
            //HttpContext.Session.SetInt32("LanguageId", browserLang);

            var result = await _client.GetAsync<SiteConfig>("SiteConfig/GetBy");

            //if (_IHostingEnvironment.IsDevelopment())
            //    result.ResultRow.layoutUrl = "";


            HttpContext.Session.Set("config", result.ResultRow);

            var _LangDisplay = await _client.GetAsync<LangDisplay>($"LangDisplay/GetLangDisplay");
            HttpContext.Session.Set("_LangDisplay", _LangDisplay.ResultList);

            if (SessionRequest._User != null)
                return Redirect("/Dashboard");
            else
                return View();
        }

        public async Task<IActionResult> Validate(string user, string pass)
        {
            var result = await _client.GetAsync<User>($"User/AdminValidate?user={user}&pass={pass}");

            if (result.RType == RType.OK && result.ResultRow != null)
            {
                //HttpContext.Session.Set("LanguageId", SessionRequest.LanguageId);
                //result.ResultRow.LanguageId = SessionRequest.LanguageId;
                _IBaseModel.LanguageId = SessionRequest.LanguageId;
                _IBaseModel.CreaUser = result.ResultRow.Id;
                result.ResultRow.LanguageId = SessionRequest.LanguageId;
                HttpContext.Session.Set("_user", result.ResultRow);


                await GetConfLang();


                return Json("/");
            }
            else
            {
                return Json("");
            }
        }



        public async Task GetConfLang()
        {
            var Languages = await _client.PostAsync<Lang>($"Lang/GetPaging", new DTParameters<Lang>());
            HttpContext.Session.Set("Languages", Languages.ResultPaging.data);
            var _LangDisplay = await _client.GetAsync<LangDisplay>($"LangDisplay/GetLangDisplay");
            HttpContext.Session.Set("_LangDisplay", _LangDisplay.ResultList);
        }

        public async  Task<JsonResult> SetLanguage(int id)
        {
            HttpContext.Session.SetInt32("LanguageId", id);
            var result = await _client.GetAsync<User>($"User/GetRow?id={SessionRequest._User.Id}");
            result.ResultRow.LanguageId = SessionRequest.LanguageId;
            _IBaseModel.LanguageId = SessionRequest.LanguageId;
            _IBaseModel.CreaUser = result.ResultRow.Id;
            HttpContext.Session.Set("_user", result.ResultRow);
            return Json("OK");
        }


        [HttpGet("forgot/{fstkcshp}")]
        public async Task<IActionResult> forgot(string fstkcshp)
        {
            var Id = 0;
            try
            {
                Id = fstkcshp.Base64Decode().ToInt();
            }
            catch (Exception ex)
            {

                return Redirect("/");
            }

            var result = await _client.GetAsync<User>($"User/GetRow?Id={Id}");
            if (result.RType == RType.OK && result.ResultRow != null)
            {
                return View(result.ResultRow.Id);
            }
            else
            {
                return Redirect("/");
            }
        }

        public async Task<JsonResult> ChangePass(int Id, string pass)
        {
            var result = await _client.GetAsync<User>($"User/GetRow?Id={Id}");
            result.ResultRow.Pass = pass;
            var resultSave = await _client.PostAsync<User>(new User().GetType().Name + "/InsertOrUpdate", result.ResultRow);
            return Json(resultSave);
        }


        //public async Task<JsonResult> SenMail(string email)
        //{

        //    var result = await _client.GetAsync<User>($"User/UserMailSearch?email={email}");

        //    if (result.RType == RType.OK && result.ResultRow != null)
        //    {
              

        //        string mailSend = await _ISendMail.Send(new MailModelCustom
        //        {
        //            Alicilar = new string[] { result.ResultRow.Mail },
        //            //bcc = new string[] { SessionRequest.config.SmtpMail },
        //            //cc = null,
        //            Icerik = html,
        //            Konu = "Reset Password".Trans(),
        //            MailGorunenAd = SessionRequest.config.MailGorunenAd,
        //            SmtpHost = SessionRequest.config.SmtpHost,
        //            SmtpMail = SessionRequest.config.SmtpMail,
        //            SmtpMailPass = SessionRequest.config.SmtpMailPass,
        //            SmtpPort = SessionRequest.config.SmtpPort,
        //            SmtpSSL = SessionRequest.config.SmtpSsl,
        //            SmtpUseDefaultCredentials = false,


        //        });


        //    }
        //    else
        //    {
        //        return Json("notfound");
        //    }



        //    return Json("OK");
        //}



    }


}
