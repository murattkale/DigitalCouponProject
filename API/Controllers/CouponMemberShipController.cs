using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class CouponMemberShipController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        ICouponMemberShipService _ICouponMemberShipService;
        IMemberShipService _IMemberShipService;
        public CouponMemberShipController(IUnitOfWork<myDBContext> _uow, ICouponMemberShipService _ICouponMemberShipService, IMemberShipService ıMemberShipService)
        {
            this._uow = _uow;
            this._ICouponMemberShipService = _ICouponMemberShipService;
            _IMemberShipService = ıMemberShipService;
        }


        [HttpGet("setData")]
        public IActionResult setData(int id1, int id2, string type)
        {
            RModel<EnumModel> res = new RModel<EnumModel>();


          
            if (type == "add")
            {
                var controlCM = _ICouponMemberShipService.Where(o => o.CouponId == id1).Result.Any();
                if (controlCM)
                {
                    res.RType = RType.Error;
                    res.Message = "Maximum 1 membership can be added";
                    return Ok(res);
                }
                _ICouponMemberShipService.Add(new CouponMemberShip() { CouponId = id1, MemberShipId = id2 });
            }
            else
            {
                var dp = _ICouponMemberShipService.Where(o => o.CouponId == id1 && o.MemberShipId == id2).Result.ToList();
                dp.ForEach(o =>
                {
                    _ICouponMemberShipService.Delete(o);
                });
            }
            var result = _uow.SaveChanges();


            res.RType = result.RType;
            res.Message = result.Message;

            return Ok(res);

        }

        [HttpGet("getData")]
        public IActionResult getData(int id1)
        {
            var dp = _ICouponMemberShipService.Where(o => o.CouponId == id1).Result.ToList();
            var rows = _IMemberShipService.Where().Result.ToList().Select(o =>
               new EnumModel
               {
                   value = o.Id.ToStr(),
                   text = o.Name,
                   selected = dp.Any(oo => oo.MemberShipId == o.Id)
               }).ToList();
            RModel<EnumModel> res = new RModel<EnumModel>();
            res.ResultList = rows;
            res.RType = RType.OK;
            return Ok(res);
        }



    }
}
