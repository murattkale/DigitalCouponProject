using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class LangDisplayController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        ILangDisplayService _ILangDisplayService;
        ILangService _ILangService;
        ILangTextService _ILangTextService;

        public LangDisplayController(IUnitOfWork<myDBContext> _uow, ILangDisplayService _ILangDisplayService, ILangService ıLangService = null, ILangTextService ıLangTextService = null)
        {
            this._uow = _uow;
            this._ILangDisplayService = _ILangDisplayService;
            _ILangService = ıLangService;
            _ILangTextService = ıLangTextService;
        }

        //[HttpGet("GetSelect")]
        //public IActionResult GetSelect(string name, string whereCase)
        //{
        //    var rModel = new RModel<EnumModel>();
        //    var result = _ILangDisplayService.Where().Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name_1 }).ToList();
        //    rModel.ResultList = result;
        //    rModel.Result = null;
        //    rModel.RType = RType.OK;
        //    return Ok(rModel);
        //}

        [HttpGet("GetLangDisplay")]
        public IActionResult GetLangDisplay()
        {
            var res = _ILangDisplayService.WhereList(null, true, false, o => o.LangText);
            return Ok(res);
        }

        [HttpGet("GetLangDisplayBy")]
        public IActionResult GetLangDisplayBy(int LangId)
        {
            var res = _ILangTextService.WhereList(o => o.LangId == LangId && o.LangDisplay != null, true, false, o => o.LangDisplay);

            //var res = _ILangDisplayService.WhereList(o => o.LangText.Any(oo => oo.LangId == LangId), true, false, o => o.LangText);
            var result = res.ResultList.Select(o => new
            {
                o.LangDisplay.ParamName,
                o.TextValue
            }).ToDictionary(x => x.ParamName, o => o.TextValue);

            return Ok(result);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<LangDisplay> param)
        {
            var result = _ILangDisplayService.GetPaging(null, true, param, false, o => o.LangText);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(LangDisplay postModel)
        {
            var dup = _ILangDisplayService.Get(o => o.Id != postModel.Id && o.ParamName == postModel.ParamName);
            if (dup.ResultRow != null)
            {
                return Ok("duplicate");
            }

            var result = _ILangDisplayService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();

            if (saveResult.RType == RType.OK)
            {
                postModel.LangText.ToList().ForEach(o =>
                {
                    o.LangDisplayId = result.ResultRow.Id;
                });
                var langInsert = _ILangTextService.InsertOrUpdateBulk(postModel.LangText.ToList());
                saveResult = _uow.SaveChanges();
            }

            result.RType = saveResult.RType;
            result.Message = saveResult.Message;
            result.MessageList = saveResult.MessageList;
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _ILangDisplayService.Get(o => o.Id == id, true, false, o => o.LangText);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _ILangDisplayService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }



    }
}
