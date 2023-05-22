using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class CityController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        ICityService _ICityService;
        public CityController(IUnitOfWork<myDBContext> _uow, ICityService _ICityService)
        {
            this._uow = _uow;
            this._ICityService = _ICityService;
        }



        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(City postModel)
        {
            var result = _ICityService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            result.RType = saveResult.RType;
            result.RTypeEnumDesc = saveResult.RTypeEnumDesc;
            result.Message = saveResult.Message;
            result.MessageList = saveResult.MessageList;
            return Ok(result);
        }


        [HttpGet("GetSelect")]
        public IActionResult GetSelect(int CountryId)
        {
            var rModel = new RModel<EnumModel>();
            var result = _ICityService.Where(o => o.CountryId == CountryId).Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }


        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<City> param)
        {
            var result = _ICityService.GetPaging(o => o.CountryId == param.selectid, true, param, false, o => o.Country);
            return Ok(result);
        }



        [HttpPost("GetAll")]
        public IActionResult GetAll(int CountryId)
        {
            var result = _ICityService.WhereList(o => o.CountryId == CountryId, true, false, o => o.Country);
            return Ok(result);
        }


        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _ICityService.Get(o => o.Id == id);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _ICityService.Delete(id);
            var saveResult = _uow.SaveChanges();
            result.RType = saveResult.RType;
            result.RTypeEnumDesc = saveResult.RTypeEnumDesc;
            result.Message = saveResult.Message;
            result.MessageList = saveResult.MessageList;
            return Ok(result);
        }



    }
}
