using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class MemberShipController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IMemberShipService _IMemberShipService;
        IBusinessTypeMemberShipService _IBusinessTypeMemberShipService;
        IBusinessTypeService _IBusinessTypeService;
        ILangTextService _ILangTextService;

        public MemberShipController(IUnitOfWork<myDBContext> _uow, IMemberShipService _IMemberShipService, IBusinessTypeMemberShipService ıBusinessTypeMemberShipService, IBusinessTypeService ıBusinessTypeService, ILangTextService ıLangTextService)
        {
            this._uow = _uow;
            this._IMemberShipService = _IMemberShipService;
            _IBusinessTypeMemberShipService = ıBusinessTypeMemberShipService;
            _IBusinessTypeService = ıBusinessTypeService;
            _ILangTextService = ıLangTextService;
        }



        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(MemberShip postModel)
        {
            var result = _IMemberShipService.InsertOrUpdate(postModel);
            postModel.LangText.ToList().ForEach(o =>
            {
                o.MemberShipId = result.ResultRow.Id;
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
            var result = _IMemberShipService.Where().Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name, name = o.Desc }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }



        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<MemberShip> param)
        {
            var result = _IMemberShipService.GetPaging(null, true, param, false);
            var Ids = result.ResultPaging.data.Select(o => o.Id).ToList();
            var rel = _IBusinessTypeMemberShipService.WhereList(o => Ids.Contains(o.MemberShipId), true, false, o => o.BusinessType);
            result.ResultPaging.data.ForEach(o =>
            {
                o.BusinessTypeMemberShip = rel.ResultList.Where(oo => oo.MemberShipId == o.Id).ToList();
            });

            return Ok(result);
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _IMemberShipService.WhereList(null, true, false, o => o.BusinessTypeMemberShip);
            var Ids = result.ResultList.Select(o => o.Id).ToList();
            var rel = _IBusinessTypeMemberShipService.WhereList(o => Ids.Contains(o.MemberShipId), true, false, o => o.BusinessType);
            var bt = _IBusinessTypeService.WhereList().ResultList;

            result.ResultList.ForEach(o =>
            {
                o.BusinessTypeMemberShip = rel.ResultList.Where(oo => oo.MemberShipId == o.Id).ToList();
                var btList = o.BusinessTypeMemberShip.Select(oo => oo.BusinessTypeId).ToList();
                o.BusinessTypeList = bt.Where(oo => btList.Any(c => c == oo.Id)).ToList();
            });
            return Ok(result);
        }


        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IMemberShipService.Get(o => o.Id == id,true,false, o => o.LangText);
            return Ok(result);
        }


        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IMemberShipService.Delete(id);
            var saveResult = _uow.SaveChanges();
            result.RType = saveResult.RType;
            result.RTypeEnumDesc = saveResult.RTypeEnumDesc;
            result.Message = saveResult.Message;
            result.MessageList = saveResult.MessageList;
            return Ok(result);
        }



    }
}
