using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class UserCouponController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IUserCouponService _IUserCouponService;
        ICouponService _ICouponService;
        public UserCouponController(IUnitOfWork<myDBContext> _uow, IUserCouponService _IUserCouponService, ICouponService ıCouponService)
        {
            this._uow = _uow;
            this._IUserCouponService = _IUserCouponService;
            _ICouponService = ıCouponService;
        }



        [HttpGet("setData")]
        public IActionResult setData(int id1, int id2, string type)
        {
            if (type == "add")
            {
                var control = _IUserCouponService.Get(o => o.UserId == id1 && o.CouponId == id2);
                if (control.ResultRow != null)
                {
                    return Ok("duplicate");
                }
                _IUserCouponService.Add(new UserCoupon() { UserId = id1, CouponId = id2 });
            }
            else
            {
                var dp = _IUserCouponService.Where(o => o.UserId == id1 && o.CouponId == id2).Result.ToList();
                dp.ForEach(o =>
                {
                    _IUserCouponService.Delete(o);
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
            var dp = _IUserCouponService.Where(o => o.UserId == id1).Result.ToList();
            var rows = _ICouponService.Where().Result.ToList().Select(o =>
               new EnumModel
               {
                   value = o.Id.ToStr(),
                   text = o.Name,
                   selected = dp.Any(oo => oo.CouponId == o.Id)
               }).ToList();
            RModel<EnumModel> res = new RModel<EnumModel>();
            res.ResultList = rows;
            res.RType = RType.OK;
            return Ok(res);
        }

        [HttpGet("getDataInterval")]
        public IActionResult getDataInterval(int UserId, int CouponId)
        {
            var res = _IUserCouponService.Get(o => o.UserId == UserId && o.CouponId == CouponId);
            return Ok(res);
        }

        [HttpGet("getUserCoupon")]
        public IActionResult getUserCoupon(int UserId)
        {
            var res = _IUserCouponService.WhereList(o => o.UserId == UserId );
            return Ok(res);
        }

    }
}
