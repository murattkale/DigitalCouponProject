using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class SiteConfigController : ControllerBase
    {
        ISiteConfigService _ISiteConfigService;
        IUnitOfWork<myDBContext> _uow;
        public SiteConfigController(IUnitOfWork<myDBContext> _uow, ISiteConfigService _ISiteConfigService)
        {
            this._ISiteConfigService = _ISiteConfigService;
            this._uow = _uow;
        }

        [HttpGet("GetBy")]
        public IActionResult GetBy()
        {
            var result = _ISiteConfigService.Get();
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(SiteConfig postModel)
        {
            var result = _ISiteConfigService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            result.RType = saveResult.RType;
            result.Message = saveResult.Message;
            result.MessageList = saveResult.MessageList;
            return Ok(result);
        }


    }
}
