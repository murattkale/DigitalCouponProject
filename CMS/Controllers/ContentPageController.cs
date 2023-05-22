using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS.Controllers
{
    public class ContentPageController : Controller
    {
        IBaseModel _IBaseModel;
        IConfiguration _IConfiguration;
        IHostingEnvironment _IHostingEnvironment;
        IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;

        public ContentPageController(
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


        public async Task<IActionResult> GetSelect(ContentTypes? ContentTypes, int? LangId)
        {
            var result = await _client.GetAsync<EnumModel>(new ContentPage().GetType().Name + "/GetSelect?ContentTypes=" + ContentTypes + $"&LangId={LangId}");
            return Json(result.ResultList);
        }


        //public async Task<IActionResult> GetSelect(ContentTypes? ContentTypes, string Joker, string Joker2)
        //{
        //    var result = await _client.GetAsync<EnumModel>(new ContentPage().GetType().Name + "/GetSelect?ContentTypes=" + ContentTypes + $"&LangId={SessionRequest.LanguageId}");
        //    result.Joker = Joker;
        //    result.Joker2 = Joker2;
        //    return Json(result);
        //}


        [HttpPost]
        public async Task<IActionResult> GetPaging(DTParameters<ContentPage> param)
        {
            var result = await _client.PostAsync<ContentPage>(new ContentPage().GetType().Name + "/GetPaging", param);
            var rs = result.ResultPaging;
            return Json(rs);
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrUpdate(ContentPage postmodel)
        {
            var result = await _client.PostAsync<ContentPage>(new ContentPage().GetType().Name + "/InsertOrUpdate", postmodel);
            return Json(result);
        }

        public async Task<IActionResult> InsertOrUpdatePage()
        {
            var result = await _client.GetAsync<ContentPage>(new ContentPage().GetType().Name + $"/GetRow?id={Request.Query["id"].ToInt()}");
            ViewBag.postModel = result.ResultRow;
            return View();
        }

        public async Task<IActionResult> GetRow(int? id)
        {
            var result = await _client.GetAsync<ContentPage>(new ContentPage().GetType().Name + $"/GetRow?id={id}");
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _client.GetAsync<ContentPage>(new ContentPage().GetType().Name + $"/Delete?id={id}");
            return Json(result);
        }


        public async Task<IActionResult> GetContentTypes()
        {
            var result = await _client.GetAsync<EnumModel>(new ContentPage().GetType().Name + $"/GetContentTypes");
            return Json(result.ResultList);
        }
        public async Task<IActionResult> GetTemplateType()
        {
            var result = await _client.GetAsync<EnumModel>(new ContentPage().GetType().Name + $"/GetTemplateType");
            return Json(result.ResultList);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrder(List<OrderUpdateModel> postModel)
        {
            var result = await _client.PostAsync<RModel<OrderUpdateModel>>(new ContentPage().GetType().Name + $"/UpdateOrder", postModel);
            return Json(result);
        }



        [HttpPost]
        public async Task<IActionResult> GetGallery(DTParameters<Documents> param, int selectid)
        {
            param.selectid = selectid;
            var result = await _client.PostAsync<Documents>(new Documents().GetType().Name + "/GetGallery", param);
            return Json(result.ResultPaging);
        }

        [HttpPost]
        public async Task<IActionResult> GetDocuments(DTParameters<Documents> param, int selectid)
        {
            param.selectid = selectid;
            var result = await _client.PostAsync<Documents>(new Documents().GetType().Name + "/GetDocuments", param);
            return Json(result.ResultPaging);
        }


        [HttpPost]
        public JsonResult SaveSingleImage(Documents postmodel)
        {
            try
            {
                var result = _client.Post<Documents>(new Documents().GetType().Name + "/InsertOrUpdate", postmodel);
                //var result = _IDocumentsService.InsertOrUpdate(postmodel);
                return Json(result.ResultRow);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        //[HttpPost]
    


        [HttpPost]
        public async Task<IActionResult> SaveMultiDoc(List<Documents> DocList)
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

        public ActionResult DeleteImage(int id)
        {
            var result = _client.Get<Documents>(new Documents().GetType().Name + $"/Delete?id={id}");
            return Json(result);
        }




    }
}
