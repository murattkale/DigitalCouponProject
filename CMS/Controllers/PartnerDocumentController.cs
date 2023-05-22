using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CMS.Controllers
{
    public class PartnerDocumentController : Controller
    {
        IConfiguration _IConfiguration;
        IHostingEnvironment _IHostingEnvironment;
        IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;

        public PartnerDocumentController(

            IHostingEnvironment _IHostingEnvironment,
            IHttpContextAccessor _IHttpContextAccessor,
            IHttpClientWrapper _client
            )
        {
            this._IHostingEnvironment = _IHostingEnvironment;
            this._IHttpContextAccessor = _IHttpContextAccessor;
            this._client = _client;
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
                    var postmodel = o as dynamic;
                    var result = _client.Post<PartnerDocument>(new PartnerDocument().GetType().Name + $"/InsertOrUpdate", postmodel);

                    if (result.RType == RType.Warning)
                    {
                        isErr = true;
                        errMsg = null;
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

        [HttpPost]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var result = await _client.GetAsync<PartnerDocument>(new PartnerDocument().GetType().Name + $"/GetRow?id={id}");
            var path = this.GetPathAndFilename(result.ResultRow.Link);
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            var resultDelete = await _client.GetAsync<PartnerDocument>(new PartnerDocument().GetType().Name + $"/Delete?id={id}");
            return Json(resultDelete);
        }

        string GetPathAndFilename(string filename)
        {
            string path = _IHostingEnvironment.WebRootPath + "\\fileupload\\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path + filename;
        }


        [HttpPost]
        public async Task<IActionResult> GetPaging(DTParameters<PartnerDocument> param)
        {
            var result = await _client.PostAsync<PartnerDocument>(new PartnerDocument().GetType().Name + $"/GetPaging", param);
            var rs = result.ResultPaging;
            return Json(rs);
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrUpdate(PartnerDocument postmodel)
        {
            var result = await _client.PostAsync<PartnerDocument>(new PartnerDocument().GetType().Name + $"/InsertOrUpdate", postmodel);
            return Json(result);
        }



        public async Task<IActionResult> GetRow(int? id)
        {
            var result = await _client.GetAsync<PartnerDocument>(new PartnerDocument().GetType().Name + $"/GetRow?id={id}");
            return Json(result);
        }


        public async Task<IActionResult> SetDesc(int id, string Desc)
        {
            var result = await _client.GetAsync<PartnerDocument>(new PartnerDocument().GetType().Name + $"/SetDesc?id={id}&Desc={Desc}");
            return Json(result);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var result = await _client.GetAsync<PartnerDocument>(new PartnerDocument().GetType().Name + $"/Delete?id={id}");
            return Json(result);
        }


    }
}
