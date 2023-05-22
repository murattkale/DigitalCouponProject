using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;


namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class PartnerDocumentController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IPartnerDocumentService _IPartnerDocumentService;
        IHostingEnvironment _IHostingEnvironment;

        public PartnerDocumentController(IUnitOfWork<myDBContext> _uow, IPartnerDocumentService _IPartnerDocumentService, IHostingEnvironment _IHostingEnvironment)
        {
            this._uow = _uow;
            this._IPartnerDocumentService = _IPartnerDocumentService;
            this._IHostingEnvironment = _IHostingEnvironment;

        }



        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(PartnerDocument postModel)
        {
            var result = _IPartnerDocumentService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IPartnerDocumentService.Get(o => o.Id == id, true, false);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IPartnerDocumentService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }





        [HttpPost("GetPartnerDocument")]
        public IActionResult GetPartnerDocument(DTParameters<PartnerDocument> param)
        {
            var result = _IPartnerDocumentService.GetPaging(o => o.PartnerId == param.selectid, true, param);
            result.ResultPaging.data = result.ResultPaging.data.OrderBy(o => o.OrderNo).ToList();
            return Ok(result);
        }



        [HttpPost]
        public IActionResult DeleteImage(int id)
        {
            var result = _IPartnerDocumentService.Where(o => o.Id == id).Result.FirstOrDefault();
            //var path = this.GetPathAndFilename(result.Link);
            //if (System.IO.File.Exists(path))
            //    System.IO.File.Delete(path);
            _IPartnerDocumentService.Delete(result.Id);
            var res = _uow.SaveChanges();
            return Ok(res);
        }


        string GetPathAndFilename(string filename)
        {
            string path = this._IHostingEnvironment.WebRootPath + "\\uploads\\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path + filename;
        }




    }
}
