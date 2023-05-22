using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class BusinessTypeController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IBusinessTypeService _IBusinessTypeService;
        ILangTextService _ILangTextService;

        public BusinessTypeController(IUnitOfWork<myDBContext> _uow, IBusinessTypeService _IBusinessTypeService, ILangTextService ıLangTextService)
        {
            this._uow = _uow;
            this._IBusinessTypeService = _IBusinessTypeService;
            _ILangTextService = ıLangTextService;
        }



        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(BusinessType postModel)
        {
            var result = _IBusinessTypeService.InsertOrUpdate(postModel);
            postModel.LangText.ToList().ForEach(o =>
            {
                o.BusinessTypeId = result.ResultRow.Id;
            });
            var langInsert = _ILangTextService.InsertOrUpdateBulk(postModel.LangText.ToList());
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
            var result = _IBusinessTypeService.Where(o => o.ParentId == null).Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _IBusinessTypeService.WhereList(o => o.ParentId == null, true, false, o => o.LangText);
            return Ok(result);
        }




        [HttpGet("GetParent")]
        public IActionResult GetParent()
        {
            var rModel = new RModel<EnumModel>();
            var result = _IBusinessTypeService.Where(o => o.ParentId == null).Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        [HttpGet("GetChild")]
        public IActionResult GetChild(int ParentId)
        {
            var rModel = new RModel<EnumModel>();
            var result = _IBusinessTypeService.Where(o => o.ParentId == ParentId).Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        [HttpGet("GetChildLang")]
        public IActionResult GetChildLang(int ParentId)
        {
            var result = _IBusinessTypeService.WhereList(o => o.ParentId == ParentId, true, false, o => o.LangText);
            return Ok(result);
        }


        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<BusinessType> param)
        {
            var result = _IBusinessTypeService.GetPaging(null, true, param, false, o => o.Parent);
            return Ok(result);
        }


        //[HttpPost("GetAll")]
        //public IActionResult GetAll(int MemberShipId)
        //{
        //    var result = _IBusinessTypeService.WhereList(o => o.Id == MemberShipId && o.ParentId == null);
        //    return Ok(result);
        //}

        //[HttpPost("GetAll")]
        //public IActionResult GetAllChild(int MemberShipId) 
        //{
        //    var result = _IBusinessTypeService.WhereList(o => o.Id == MemberShipId && o.ParentId != null, true, false, o => o.Parent, o => o.Childs);
        //    return Ok(result);
        //}



        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IBusinessTypeService.Get(o => o.Id == id, true, false, o => o.LangText);
            return Ok(result);
        }

        [HttpGet("GetRowParent")]
        public IActionResult GetRowParent(int? id)
        {
            var result = _IBusinessTypeService.Get(o => o.Id == id, true, false, o => o.Parent);
            return Ok(result);
        }


        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IBusinessTypeService.Delete(id);
            var saveResult = _uow.SaveChanges();
            result.RType = saveResult.RType;
            result.RTypeEnumDesc = saveResult.RTypeEnumDesc;
            result.Message = saveResult.Message;
            result.MessageList = saveResult.MessageList;
            return Ok(result);
        }



    }
}
