using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class ErrorLogController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IErrorLogService _IErrorLogService;
        public ErrorLogController(IUnitOfWork<myDBContext> _uow, IErrorLogService _IErrorLogService)
        {
            this._uow = _uow;
            this._IErrorLogService = _IErrorLogService;
        }



        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(ErrorLog postModel)
        {
            var result = _IErrorLogService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            result.RType = saveResult.RType;
            result.RTypeEnumDesc = saveResult.RTypeEnumDesc;
            result.Message = saveResult.Message;
            result.MessageList = saveResult.MessageList;
            return Ok(result);
        }



        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<ErrorLog> param)
        {
            var result = _IErrorLogService.GetPaging(null, true, param, false);
            return Ok(result);
        }



        [HttpPost("GetAll")]
        public IActionResult GetAll()
        {
            var result = _IErrorLogService.WhereList();
            return Ok(result);
        }


        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IErrorLogService.Get(o => o.Id == id);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IErrorLogService.Delete(id);
            var saveResult = _uow.SaveChanges();
            result.RType = saveResult.RType;
            result.RTypeEnumDesc = saveResult.RTypeEnumDesc;
            result.Message = saveResult.Message;
            result.MessageList = saveResult.MessageList;
            return Ok(result);
        }



    }
}
