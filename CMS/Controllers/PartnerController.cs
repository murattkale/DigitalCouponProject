using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS.Controllers
{
    public class PartnerController : Controller
    {
        ISendMail _ISendMail;
        IHostingEnvironment _IHostingEnvironment;
        IHttpClientWrapper _client;

        public PartnerController(

            ISendMail _ISendMail,
            IHostingEnvironment _IHostingEnvironment,
            IHttpClientWrapper _client
            )
        {
            this._ISendMail = _ISendMail;
            this._IHostingEnvironment = _IHostingEnvironment;
            this._client = _client;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Expire()
        {
            return View();
        }

        public IActionResult PopulerPartner()
        {
            return View();
        }



        public async Task<IActionResult> GetSelect()
        {
            var result = await _client.GetAsync<EnumModel>(new Partner().GetType().Name + $"/GetSelect");
            return Json(result.ResultList);
        }


        [HttpPost]
        public async Task<IActionResult> GetPaging(DTParameters<Partner> param)
        {
            var result = await _client.PostAsync<Partner>(new Partner().GetType().Name + "/GetPaging", param);
            var rs = result.ResultPaging;
            return Json(rs);
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrUpdate(Partner postmodel)
        {
            var result = await _client.PostAsync<Partner>(new Partner().GetType().Name + "/InsertOrUpdate", postmodel);

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> PartnerLocationUpdate(LocationModel postModel)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(postModel.Location);
            var LocationDecode = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            postModel.Location = LocationDecode;
            var result = await _client.PostAsync<Partner>(new Partner().GetType().Name + $"/PartnerLocationUpdate",postModel);
            return Json(postModel.Location);
        }


        public  IActionResult InsertOrUpdatePage()
        {
            if (Request.Query["id"].ToInt() > 0)
            {
                var result =  _client.Get<Partner>(new Partner().GetType().Name + $"/GetRow?id={Request.Query["id"].ToInt()}");
                ViewBag.postModel = result.ResultRow;
            }

            return View();
        }

        public async Task<IActionResult> GetRow(int? id)
        {
            var result = await _client.GetAsync<Partner>(new Partner().GetType().Name + $"/GetRow?id={id}");
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _client.GetAsync<Partner>(new Partner().GetType().Name + $"/Delete?id={id}");
            return Json(result);
        }






        [HttpPost]
        public async Task<IActionResult> GetPartnerDocument(DTParameters<PartnerDocument> param, int selectid)
        {
            param.selectid = selectid;
            var result = await _client.PostAsync<PartnerDocument>(new PartnerDocument().GetType().Name + "/GetPartnerDocument", param);
            return Json(result.ResultPaging);
        }


        [HttpPost]
        public JsonResult SaveSingleImage(PartnerDocument postmodel)
        {
            try
            {
                var result = _client.Post<PartnerDocument>(new PartnerDocument().GetType().Name + "/InsertOrUpdate", postmodel);
                //var result = _IDocumentsService.InsertOrUpdate(postmodel);
                return Json(result.ResultRow);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        [HttpPost]
        public async Task<IActionResult> SaveMultiDoc(List<PartnerDocument> DocList)
        {
            try
            {
                var isErr = false;
                List<string> errMsg = new List<string>();
                DocList.ForEach(o =>
                {
                    var result = _client.Post<PartnerDocument>(new PartnerDocument().GetType().Name + "/InsertOrUpdate", o);
                    //RModel<PartnerDocument> result = _IDocumentsService.InsertOrUpdate(o);
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
            var result = _client.Get<PartnerDocument>(new PartnerDocument().GetType().Name + $"/Delete?id={id}");
            return Json(result);
        }


        [HttpPost]
        public async Task<IActionResult> GetIsState(RModel<Partner> postModel)
        {
            var result = await _client.GetAsync<EnumModel>(new Partner().GetType().Name + $"/GetIsState");
            result.Joker = postModel.Joker;
            result.Joker2 = postModel.Joker2;
            return Json(result);
        }


        [HttpPost]
        public async Task<IActionResult> GetActiveMonths(RModel<Partner> postModel)
        {
            var result = await _client.GetAsync<EnumModel>(new Partner().GetType().Name + $"/GetActiveMonths");
            result.Joker = postModel.Joker;
            result.Joker2 = postModel.Joker2;
            return Json(result);
        }


    }
}
