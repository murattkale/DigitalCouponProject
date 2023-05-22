using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class ReportController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        ILangTextService _ILangTextService;
        IMemberShipService _IMemberShipService;
        IBusinessTypeMemberShipService _IBusinessTypeMemberShipService;
        IBusinessTypeService _IBusinessTypeService;
        IBusinessTypePartnerService _IBusinessTypePartnerService;
        ICouponMemberShipService _ICouponMemberShipService;
        ICouponService _ICouponService;
        IPartnerCouponService _IPartnerCouponService;
        IUserCouponService _IUserCouponService;
        IUserCardService _IUserCardService;
        IUserService _IUserService;
        IUserMemberShipService _IUserMemberShipService;
        IPartnerService _IPartnerService;

        public ReportController(
            IUnitOfWork<myDBContext> _uow,
            IMemberShipService _IMemberShipService,
            IBusinessTypeMemberShipService ıBusinessTypeMemberShipService,
            IBusinessTypeService ıBusinessTypeService,
            ILangTextService ıLangTextService,
            IBusinessTypePartnerService ıBusinessTypePartnerService,
            ICouponMemberShipService ıCouponMemberShipService,
            ICouponService ıCouponService,
            IPartnerCouponService ıPartnerCouponService,
            IUserCouponService ıUserCouponService,
            IUserCardService ıUserCardService,
            IUserService ıUserService,
            IUserMemberShipService ıUserMemberShipService,
            IPartnerService ıPartnerService)
        {
            this._uow = _uow;
            this._IMemberShipService = _IMemberShipService;
            _IBusinessTypeMemberShipService = ıBusinessTypeMemberShipService;
            _IBusinessTypeService = ıBusinessTypeService;
            _ILangTextService = ıLangTextService;
            _IBusinessTypePartnerService = ıBusinessTypePartnerService;
            _ICouponMemberShipService = ıCouponMemberShipService;
            _ICouponService = ıCouponService;
            _IPartnerCouponService = ıPartnerCouponService;
            _IUserCouponService = ıUserCouponService;
            _IUserCardService = ıUserCardService;
            _IUserService = ıUserService;
            _IUserMemberShipService = ıUserMemberShipService;
            _IPartnerService = ıPartnerService;
        }



        [HttpGet("GetMainTotals")]
        public IActionResult GetMainTotals()
        {
            var res = _IUserMemberShipService.Where(null, true, false, o => o.MemberShip);
            var totalResult = res.Result.Sum(o => o.MemberShip.PriceEuro);
            DateTime BuAy_BaslangicTarihi = DateTime.Now.AyinIlkGunu();
            DateTime BuAy_BitisTarihi = DateTime.Now.AyinSonGunu();
            var monthsResult = res.Result.Where(o => o.CreaDate >= BuAy_BaslangicTarihi && o.CreaDate <= BuAy_BitisTarihi).Sum(o => o.MemberShip.PriceEuro);
            return Ok(new { totalResult = totalResult, monthsResult = monthsResult });
        }


        [HttpGet("GetTopCoupons")]
        public IActionResult GetTopCoupons()
        {
            var res = _IUserCouponService.Where(null, true, false, o => o.Coupon.LangText);
            var result = res.Result;
            var result7Day = result
                .Where(o => o.CreaDate >= DateTime.Now.AddDays(7))
                .ToList();
            var resPercent = result7Day.GroupBy(o => o.CouponId).Select(o => new TextValueId
            {
                id = o.Key,
                text = o.FirstOrDefault().Coupon.Name,
                value = o.Count() * 100 / result7Day.Count()
            }).OrderByDescending(o => o.value).Take(3).ToList();


            var resPercentOther = result7Day.Where(o => !resPercent.Any(oo => oo.id == o.CouponId)).GroupBy(o => o.CouponId).Select(o => new TextValueId
            {
                id = o.Key,
                text = o.FirstOrDefault().Coupon.Name,
                value = o.Count() * 100 / result7Day.Count()
            }).Sum(o => o.value);


            resPercent.Add(new TextValueId() { id = 0, text = "Others", value = resPercentOther });

            return Ok(resPercent);
        }


        [HttpGet("GetTotalUsers")]
        public IActionResult GetTotalUsers()
        {
            var users = _IUserService.WhereList();
            var deviceUsers = users.ResultList.GroupBy(o => o.DeviceState).Select(o => new TextValueId
            {
                text = o.Key.ExGetDescription(),
                value = o.Count(),
            }).ToList();
            return Ok(new { deviceUsers = deviceUsers, users = users.ResultList.Count });
        }


        [HttpGet("GetTotalPartners")]
        public IActionResult GetTotalPartners()
        {
            var result = _IPartnerService.WhereList();
            var Ids = result.ResultList.Select(o => o.Id).ToList();

            var rel = _IBusinessTypePartnerService.WhereList(o => Ids.Contains(o.PartnerId), true, false, o => o.BusinessType);
            result.ResultList.ForEach(o =>
            {
                o.BusinessTypePartner = rel.ResultList.Where(oo => oo.PartnerId == o.Id).ToList();
            });

            var tUsers = result.ResultList.GroupBy(o => o.BusinessTypePartnerNames.FirstOrDefault()).Select(o => new TextValueId
            {
                text = o.Key,
                value = o.Count(),
            }).Where(o => o.text != null && o.text != "").ToList();
            return Ok(tUsers);
        }


        [HttpGet("GetMonthNewUsers")]
        public IActionResult GetMonthNewUsers()
        {
            DateTime BuAy_BaslangicTarihi = DateTime.Now.AyinIlkGunu();
            DateTime BuAy_BitisTarihi = DateTime.Now.AyinSonGunu();

            var res = _IUserService.Where(o => o.CreaDate >= BuAy_BaslangicTarihi && o.CreaDate <= BuAy_BitisTarihi);
            var tUsers = res.Result.GroupBy(o => o.DeviceState).Select(o => new TextValueId
            {
                text = o.Key.ExGetDescription(),
                value = o.Count(),
            }).ToList();
            return Ok(tUsers);
        }


    }
}
