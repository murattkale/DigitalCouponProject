using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class BusinessTypeMemberShipController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IBusinessTypeMemberShipService _IBusinessTypeMemberShipService;
        IBusinessTypeService _IBusinessTypeService;
        public BusinessTypeMemberShipController(IUnitOfWork<myDBContext> _uow,
                            IBusinessTypeMemberShipService ıBusinessTypeMemberShipService,
                            IBusinessTypeService ıBusinessTypeService)
        {
            this._uow = _uow;
            _IBusinessTypeMemberShipService = ıBusinessTypeMemberShipService;
            _IBusinessTypeService = ıBusinessTypeService;
        }


        [HttpGet("setData")]
        public IActionResult setData(int id1, int id2, string type)
        {
            if (type == "add")
            {
                _IBusinessTypeMemberShipService.Add(new BusinessTypeMemberShip() { MemberShipId = id1, BusinessTypeId = id2 });
            }
            else
            {
                var dp = _IBusinessTypeMemberShipService.Where(o => o.MemberShipId == id1 && o.BusinessTypeId == id2).Result.ToList();
                dp.ForEach(o =>
                {
                    _IBusinessTypeMemberShipService.Delete(o);
                });
            }
            var result = _uow.SaveChanges();

            RModel<EnumModel> res = new RModel<EnumModel>();
            res.RType = result.RType;
            res.Message = result.Message;

            return Ok(res);
        }

        [HttpGet("getData")]
        public IActionResult getData(int id1)
        {
            var dp = _IBusinessTypeMemberShipService.Where(o => o.MemberShipId == id1).Result.ToList();
            var rows = _IBusinessTypeService.Where(o => o.ParentId != null, true, false, o => o.Parent).Result.ToList().Select(o =>
               new EnumModel
               {
                   value = o.Id.ToStr(),
                   text = o.Parent != null ? o.Parent.Name + " > " + o.Name : o.Name,
                   selected = dp.Any(oo => oo.BusinessTypeId == o.Id)
               }).ToList();
            RModel<EnumModel> res = new RModel<EnumModel>();
            res.ResultList = rows;
            res.RType = RType.OK;
            return Ok(res);
        }


        [HttpPost("GetAll")]
        public IActionResult GetAll(int MemberShipId)
        {
            var res = _IBusinessTypeService.WhereList(null, true, false, o => o.Childs);
            var relation = _IBusinessTypeMemberShipService.WhereList(o => o.MemberShipId == MemberShipId, true, false, o => o.MemberShip);

            var list = new List<BusinessTypeMemberShip>();
            res.ResultList.ForEach(o =>
            {
                list.Add(new BusinessTypeMemberShip()
                {
                    BusinessType = o,
                    MemberShip = relation.ResultList.FirstOrDefault(oo => oo.BusinessTypeId == o.Id)?.MemberShip
                });
            });

            var result = new RModel<BusinessTypeMemberShip>();
            result.ResultList = list;
            result.RType = RType.OK;

            return Ok(result);
        }

      


    }
}
