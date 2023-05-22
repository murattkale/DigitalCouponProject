using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class CountryController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        ICountryService _ICountryService;
        public CountryController(IUnitOfWork<myDBContext> _uow, ICountryService _ICountryService)
        {
            this._uow = _uow;
            this._ICountryService = _ICountryService;
        }



        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(Country postModel)
        {
            var result = _ICountryService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            result.RType = saveResult.RType;
            result.RTypeEnumDesc = saveResult.RTypeEnumDesc;
            result.Message = saveResult.Message;
            result.MessageList = saveResult.MessageList;
            return Ok(result);
        }


        [HttpGet("GetSelect")]
        public IActionResult GetSelect()
        {
            var rModel = new RModel<EnumModel>();
            var result = _ICountryService.Where().Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }


        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<Country> param)
        {
            var result = _ICountryService.GetPaging(null, true, param, false);
            return Ok(result);
        }


        [HttpPost("GetAll")]
        public IActionResult GetAll()
        {
            var result = _ICountryService.WhereList();
            return Ok(result);
        }


        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _ICountryService.Get(o => o.Id == id);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _ICountryService.Delete(id);
            var saveResult = _uow.SaveChanges();
            result.RType = saveResult.RType;
            result.RTypeEnumDesc = saveResult.RTypeEnumDesc;
            result.Message = saveResult.Message;
            result.MessageList = saveResult.MessageList;
            return Ok(result);
        }



    }
}
