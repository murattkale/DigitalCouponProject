using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class UserCardController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IUserCardService _IUserCardService;
        IMemberShipService _IMemberShipService;
        IBusinessTypeService _IBusinessTypeService;
        IBusinessTypeMemberShipService _IBusinessTypeMemberShipService;
        IUserCouponService _IUserCouponService;
        ICouponService _ICouponService;
        IPartnerCouponService _IPartnerCouponService;
        public UserCardController(IUnitOfWork<myDBContext> _uow, IUserCardService _IUserCardService, IMemberShipService ıMemberShipService, IBusinessTypeMemberShipService ıBusinessTypeMemberShipService, IBusinessTypeService ıBusinessTypeService, IUserCouponService ıUserCouponService, ICouponService ıCouponService, IPartnerCouponService ıPartnerCouponService)
        {
            this._uow = _uow;
            this._IUserCardService = _IUserCardService;
            _IMemberShipService = ıMemberShipService;
            _IBusinessTypeService = ıBusinessTypeService;
            _IBusinessTypeMemberShipService = ıBusinessTypeMemberShipService;
            _IUserCouponService = ıUserCouponService;
            _ICouponService = ıCouponService;
            _IPartnerCouponService = ıPartnerCouponService;
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<UserCard> param)
        {
            var result = _IUserCardService.GetPaging(o => (param.selectid > 0 ? param.selectid == o.MemberShipId : true) && o.UserId != null, true, param, false, o => o.MemberShip, o => o.User);
            return Ok(result);
        }

        [HttpPost("GetPagingNotUser")]
        public IActionResult GetPagingNotUser(DTParameters<UserCard> param)
        {
            var result = _IUserCardService.GetPaging(o => (param.selectid > 0 ? param.selectid == o.MemberShipId : true) && o.UserId == null, true, param, false, o => o.MemberShip, o => o.User);
            return Ok(result);
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll(int UserId)
        {
            var result = _IUserCardService.WhereList(o => o.UserId == UserId, true, false, o => o.User, o => o.MemberShip.BusinessTypeMemberShip);
            var bt = _IBusinessTypeService.WhereList().ResultList;


            var couponList = _IUserCouponService.WhereList(oo => oo.UserId == UserId, true, false, o => o.Coupon.CouponMemberShip);

            var cp = _IPartnerCouponService.WhereList(o => couponList.ResultList.Select(oo => oo.CouponId).Any(ooo => ooo == o.CouponId), true, false, o => o.Partner, o => o.Coupon).ResultList;


            result.ResultList.ForEach(o =>
            {
                var btList = o.MemberShip.BusinessTypeMemberShip.Select(oo => oo.BusinessTypeId).ToList();
                o.MemberShip.BusinessTypeList = bt.Where(oo => btList.Any(c => c == oo.Id)).ToList();
                o.MemberShip.UserMemberShipPartnerCouponHistory = couponList.ResultList.Select(o => new UserMemberShipPartnerCouponHistory()
                {
                    UserCouponCreaDate = o.CreaDate,
                    CouponName = o.Coupon.Name,
                    Discount = o.Coupon.Discount,
                    CouponTypeName = o.Coupon.CouponTypeName,
                    InfoText = o.Coupon.InfoText,
                    PartnerName = cp.FirstOrDefault(oo => oo.CouponId == o.CouponId).Partner.Name

                }).ToList();
            });



            return Ok(result);
        }



        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(UserCard postModel)
        {
            var result = _IUserCardService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            result.RType = saveResult.RType;
            result.RTypeEnumDesc = saveResult.RTypeEnumDesc;
            result.Message = saveResult.Message;
            result.MessageList = saveResult.MessageList;
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IUserCardService.Get(o => o.Id == id);
            return Ok(result);
        }

        [HttpGet("GetCode")]
        public IActionResult GetCode(string QrCode)
        {
            var result = _IUserCardService.Get(o => o.QrCode == QrCode, true, false, o => o.MemberShip);
            return Ok(result);
        }


        [HttpGet("UserCardUpdate")]
        public IActionResult UserCardUpdate(int UserId, int UserCardId)
        {
            var result = _IUserCardService.Get(o => o.Id == UserCardId, false);
            result.ResultRow.UserId = UserId;
            var saveResult = _uow.SaveChanges();
            result.RType = saveResult.RType;
            result.RTypeEnumDesc = saveResult.RTypeEnumDesc;
            result.Message = saveResult.Message;
            result.MessageList = saveResult.MessageList;
            return Ok(result);
        }


        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IUserCardService.Delete(id);
            var saveResult = _uow.SaveChanges();
            result.RType = saveResult.RType;
            result.RTypeEnumDesc = saveResult.RTypeEnumDesc;
            result.Message = saveResult.Message;
            result.MessageList = saveResult.MessageList;
            return Ok(result);
        }



        [HttpGet("BulkDelete")]
        public IActionResult BulkDelete(int MemberShipId)
        {
            var result = _IUserCardService.WhereList(o => o.MemberShipId == MemberShipId, false);
            _IUserCardService.DeleteBulk(result.ResultList);
            _uow.SaveChanges();
            return Ok(result);
        }


        [HttpGet("BulkInsert")]
        public IActionResult BulkInsert(string MemberShips, int count)
        {
            var uc = new List<UserCard>();

            MemberShips.Split('-').ToList().ForEach(o =>
            {
                for (int i = 0; i < count; i++)
                {
                    var mid = o.ToInt();
                    _IUserCardService.InsertOrUpdate(new UserCard()
                    {
                        MemberShipId = mid,
                        QrCode =
                       (mid == 1 ? 1004 : mid == 2 ? 1003 : mid == 3 ? 1002 : 1001)
                        + "-" + (new Random().Next(1000, 9999) + 1).ToStr()
                        + "-" + (new Random().Next(1000, 9999) + 1).ToStr()
                        + "-" + (new Random().Next(1000, 9999) + 1).ToStr()
                    });

                }
            });

            var result = _uow.SaveChanges();

            RModel<EnumModel> res = new RModel<EnumModel>();
            res.RType = result.RType;
            res.Message = result.Message;

            return Ok(res);
        }


    }
}
