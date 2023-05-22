using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class PartnerCouponController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IPartnerCouponService _IPartnerCouponService;
        ICouponService _ICouponService;
        public PartnerCouponController(IUnitOfWork<myDBContext> _uow, IPartnerCouponService _IPartnerCouponService, ICouponService ıCouponService)
        {
            this._uow = _uow;
            this._IPartnerCouponService = _IPartnerCouponService;
            _ICouponService = ıCouponService;
        }


        [HttpGet("setData")]
        public IActionResult setData(int id1, int id2, string type)
        {
            var dp = _IPartnerCouponService.Where(o => o.PartnerId == id1 && o.CouponId == id2).Result.ToList();

            if (dp.Any())
            {
                return Ok("duplicate");
            }

            if (type == "add")
            {
                _IPartnerCouponService.Add(new PartnerCoupon() { PartnerId = id1, CouponId = id2 });
            }
            else
            {
                dp.ForEach(o =>
                {
                    _IPartnerCouponService.Delete(o);
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
            var dp = _IPartnerCouponService.Where(o => o.PartnerId == id1).Result.ToList();
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


        [HttpGet("GetAll")]
        public IActionResult GetAll(int PartnerId)
        {
            var result = _IPartnerCouponService.Where(o => o.PartnerId == PartnerId, true, false,
                o => o.Coupon.CouponMemberShip).Result.ToList();
            //var rel = _ICouponMemberShipService.WhereList(o => Ids.Contains(o.CouponId), true, false, o => o.MemberShip);

            //result.ResultPaging.data.ForEach(o =>
            //{
            //    o.CouponMemberShip = rel.ResultList.Where(oo => oo.CouponId == o.Id).ToList();
            //});
            return Ok(result);
        }

        [HttpGet("GetAllCoupon")]
        public IActionResult GetAllCoupon(int PartnerId)
        {
            var result = _IPartnerCouponService.Where(o => o.PartnerId == PartnerId, true, false,
                o => o.Coupon).Result.Select(x => x.Coupon).ToList();
            //var rel = _ICouponMemberShipService.WhereList(o => Ids.Contains(o.CouponId), true, false, o => o.MemberShip);

            //result.ResultPaging.data.ForEach(o =>
            //{
            //    o.CouponMemberShip = rel.ResultList.Where(oo => oo.CouponId == o.Id).ToList();
            //});
            return Ok(result);
        }




    }
}
