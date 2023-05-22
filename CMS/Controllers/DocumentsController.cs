using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS.Controllers
{
    public class DocumentsController : Controller
    {
        IBaseModel _IBaseModel;
        IConfiguration _IConfiguration;
        IHostingEnvironment _IHostingEnvironment;
        IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;

        public DocumentsController(
           IBaseModel _IBaseModel,
            IHostingEnvironment _IHostingEnvironment,
            IHttpContextAccessor _IHttpContextAccessor,
            IHttpClientWrapper _client
            )
        {
            this._IBaseModel = _IBaseModel;
            this._IHostingEnvironment = _IHostingEnvironment;
            this._IHttpContextAccessor = _IHttpContextAccessor;
            this._client = _client;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Add()
        {
            return View();
        }



        public async Task<IActionResult> GetSelect(ContentTypes? ContentTypes)
        {
            var result = await _client.GetAsync<EnumModel>(new Documents().GetType().Name + "/GetSelect?ContentTypes=" + ContentTypes);
            return Json(result.ResultList);
        }

        public async Task<IActionResult> GetSelectCustom(ContentTypes? ContentTypes, string Joker, string Joker2)
        {
            var result = await _client.GetAsync<EnumModel>(new Documents().GetType().Name + "/GetSelect?ContentTypes=" + ContentTypes);
            result.Joker = Joker;
            result.Joker2 = Joker2;
            return Json(result);
        }


        [HttpPost]
        public async Task<IActionResult> GetPaging(DTParameters<Documents> param)
        {
            var result = await _client.PostAsync<Documents>(new Documents().GetType().Name + "/GetPaging", param);
            var rs = result.ResultPaging;
            return Json(rs);
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrUpdate(Documents postmodel)
        {
            var result = await _client.PostAsync<Documents>(new Documents().GetType().Name + "/InsertOrUpdate", postmodel);
            return Json(result);
        }

        public async Task<IActionResult> InsertOrUpdatePage()
        {
            var result = await _client.GetAsync<Documents>(new Documents().GetType().Name + $"/GetRow?id={Request.Query["id"].ToInt()}");
            ViewBag.postModel = result.ResultRow;
            return View();
        }

        public async Task<IActionResult> GetRow(int? id)
        {
            var result = await _client.GetAsync<Documents>(new Documents().GetType().Name + $"/GetRow?id={id}");
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _client.GetAsync<Documents>(new Documents().GetType().Name + $"/Delete?id={id}");
            return Json(result);
        }




        [HttpPost]
        public async Task<JsonResult> SaveMultiDoc(List<Documents> DocList)
        {
            try
            {
                var isErr = false;
                List<string> errMsg = new List<string>();
                DocList.ForEach(o =>
                {
                    var result = _client.Post<Documents>(new Documents().GetType().Name + "/InsertOrUpdate", o);
                    //RModel<Documents> result = _IDocumentsService.InsertOrUpdate(o);
                    if (result.RType == RType.Warning)
                    {
                        isErr = true;
                        errMsg = result.MessageList;
                    }
                });
                if (isErr)
                {
                    return Json("Err-duplicate");
                }
                else
                {
                    return Json(DocList);
                }

            }
            catch (Exception ex)
            {
                return Json("Err-try");
                throw ex;
            }

        }





    }
}
