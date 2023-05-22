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
    public class DocumentsController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IDocumentsService _IDocumentsService;
        IHostingEnvironment _IHostingEnvironment;

        public DocumentsController(IUnitOfWork<myDBContext> _uow, IDocumentsService _IDocumentsService, IHostingEnvironment _IHostingEnvironment)
        {
            this._uow = _uow;
            this._IDocumentsService = _IDocumentsService;
            this._IHostingEnvironment = _IHostingEnvironment;

        }



        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(Documents postModel)
        {
            var result = _IDocumentsService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IDocumentsService.Get(o => o.Id == id, true, false);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IDocumentsService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }




        [HttpPost("GetGallery")]
        public IActionResult GetGallery(DTParameters<Documents> param)
        {
            var result = _IDocumentsService.GetPaging(o => o.GalleryId == param.selectid, true, param);
            result.ResultPaging.data = result.ResultPaging.data.OrderBy(o => o.OrderNo).ToList();
            return Ok(result);
        }

        [HttpPost("GetDocuments")]
        public IActionResult GetDocuments(DTParameters<Documents> param)
        {
            var result = _IDocumentsService.GetPaging(o => o.DocumentId == param.selectid, true, param);
            result.ResultPaging.data = result.ResultPaging.data.OrderBy(o => o.OrderNo).ToList();
            return Ok(result);
        }



        [HttpPost]
        public IActionResult DeleteImage(int id)
        {
            var result = _IDocumentsService.Where(o => o.Id == id).Result.FirstOrDefault();
            //var path = this.GetPathAndFilename(result.Link);
            //if (System.IO.File.Exists(path))
            //    System.IO.File.Delete(path);
            _IDocumentsService.Delete(result.Id);
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
