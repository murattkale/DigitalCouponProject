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
    public class CouponController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        ICouponService _ICouponService;
        ICouponMemberShipService _ICouponMemberShipService;
        ILangTextService _ILangTextService;
        public CouponController(IUnitOfWork<myDBContext> _uow, ICouponService _ICouponService, ICouponMemberShipService ıCouponMemberShipService, ILangTextService ıLangTextService)
        {
            this._uow = _uow;
            this._ICouponService = _ICouponService;
            _ICouponMemberShipService = ıCouponMemberShipService;
            _ILangTextService = ıLangTextService;
        }



        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(Coupon postModel)
        {
            var result = _ICouponService.InsertOrUpdate(postModel);
            postModel.LangText.ToList().ForEach(o =>
            {
                o.CouponId = result.ResultRow.Id;
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
            var result = _ICouponService.Where().Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<Coupon> param)
        {
            var result = _ICouponService.GetPaging(null, true, param, false);
            var Ids = result.ResultPaging.data.Select(o => o.Id).ToList();
            var rel = _ICouponMemberShipService.WhereList(o => Ids.Contains(o.CouponId), true, false, o => o.MemberShip);
            result.ResultPaging.data.ForEach(o =>
            {
                o.CouponMemberShip = rel.ResultList.Where(oo => oo.CouponId == o.Id).ToList();
            });

            return Ok(result);
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll(int PartnerId)
        {
            var result = _ICouponService.WhereList(o => o.PartnerCoupon.Any(oo => oo.PartnerId == PartnerId), true, false, o => o.PartnerCoupon, o => o.CouponMemberShip,o=>o.LangText);
            var Ids = result.ResultList.Select(o => o.Id).ToList();
            var rel = _ICouponMemberShipService.WhereList(o => Ids.Contains(o.CouponId), true, false, o => o.MemberShip);
            result.ResultList.ForEach(o =>
            {
                o.CouponMemberShip = rel.ResultList.Where(oo => oo.CouponId == o.Id).ToList();
            });
            return Ok(result);
        }

        [HttpGet("GetCouponUsers")]
        public IActionResult GetCouponUsers(int UserId)
        {
            var result = _ICouponService.WhereList(o => o.UserCoupon.Any(oo => oo.UserId == UserId), true, false, o => o.UserCoupon, o => o.CouponMemberShip, o => o.LangText);
            var Ids = result.ResultList.Select(o => o.Id).ToList();
            var rel = _ICouponMemberShipService.WhereList(o => Ids.Contains(o.CouponId), true, false, o => o.MemberShip);
            result.ResultList.ForEach(o =>
            {
                o.CouponMemberShip = rel.ResultList.Where(oo => oo.CouponId == o.Id).ToList();
            });
            return Ok(result);
        }


        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _ICouponService.Get(o => o.Id == id,true,false, o => o.LangText);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _ICouponService.Delete(id);
            var saveResult = _uow.SaveChanges();
            result.RType = saveResult.RType;
            result.RTypeEnumDesc = saveResult.RTypeEnumDesc;
            result.Message = saveResult.Message;
            result.MessageList = saveResult.MessageList;
            return Ok(result);
        }




        [HttpGet("GetCouponType")]
        public IActionResult GetCouponType()
        {
            var rModel = new RModel<EnumModel>();
            var list = Enum.GetValues(typeof(CouponType)).Cast<int>()
                .Select(x => new EnumModel { name = ((CouponType)x).ToStr(), value = x.ToString(), text = ((CouponType)x).ExGetDescription() }).ToList();
            rModel.ResultList = list;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }


        [HttpGet("GetCode")]
        public IActionResult GetCode(string QrCode)
        {
            var result = _ICouponService.Get(o => o.Code == QrCode, true, false);
            return Ok(result);
        }


    }
}
