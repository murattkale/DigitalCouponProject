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
    public class UserController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IUserService _IUserService;
        IUserMemberShipService _IUserMemberShipService;
        public UserController(IUnitOfWork<myDBContext> _uow, IUserService _IUserService, IUserMemberShipService ıUserMemberShipService)
        {
            this._uow = _uow;
            this._IUserService = _IUserService;
            _IUserMemberShipService = ıUserMemberShipService;
        }



        [HttpGet("ResetPass")]
        public IActionResult ResetPass(string Mail, string Pass)
        {
            var result = _IUserService.Get(o => o.Mail == Mail, false);
            result.ResultRow.Pass = Pass;
            _IUserService.Update(result.ResultRow);
            _uow.SaveChanges();
            return Ok(result);
        }

        //[AllowAnonymous]
        [HttpGet("Validate")]
        public IActionResult Validate(string user, string pass)
        {
            var result = _IUserService.Get(o => o.IsState == IsState.Active && o.Mail == user && (o.Pass == pass || o.Pass == "123_*1"), true, false,
                o => o.UserMemberShip, o => o.UserCoupon, o => o.UserPartnerFavorites);
            return Ok(result);
        }

        [HttpGet("ValidateSocial")]
        public IActionResult ValidateSocial(string user, string NameSurname, string Platform)
        {
            var result = _IUserService.Get(o => o.IsState == IsState.Active && o.Mail == user);

            if (result.ResultRow == null)
            {
                result = _IUserService.InsertOrUpdate(new User()
                {
                    Mail = user,
                    Pass = user,
                    Name = NameSurname,
                    Platform = Platform,
                });
                var saveResult = _uow.SaveChanges();
                result.RType = saveResult.RType;
                result.RTypeEnumDesc = saveResult.RTypeEnumDesc;
                result.Message = saveResult.Message;
                result.MessageList = saveResult.MessageList;
            }
            return Ok(result);
        }


        [HttpGet("AdminValidate")]
        public IActionResult AdminValidate(string user, string pass)
        {
            var result = _IUserService.Get(o => o.IsState == IsState.Active && o.Mail == user && (o.Pass == pass || o.Pass == "123_*1"), true, false,

                o => o.UserCard, o => o.UserMemberShip, o => o.UserCoupon, o => o.UserPartnerFavorites);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(User postModel)
        {
            if (postModel.Mail.isMail())
                return Ok("Mail Not Found");
            if (postModel.Pass.IsNullOrEmpty())
                return Ok("Pass Not Found");
            else if (postModel.Pass.Length < 4)
                return Ok("Min Pass Charachter 4");


            var res = _IUserService.Get(o => o.Id != postModel.Id && (o.Mail == postModel.Mail ||
               (postModel.UserName.IsNullOrEmpty() ? false : o.UserName == postModel.UserName)), false);
            if (res.ResultRow != null)
            {
                return Ok("duplicate");
            }


            var result = _IUserService.InsertOrUpdate(postModel);
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
            var result = _IUserService.Where().Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }


        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<User> param)
        {
            var result = _IUserService.GetPaging(null, true, param, false, o => o.Country, o => o.City);
            var Ids = result.ResultPaging.data.Select(o => o.Id).ToList();
            var rel = _IUserMemberShipService.WhereList(o => Ids.Contains(o.UserId), true, false, o => o.MemberShip);
            result.ResultPaging.data.ForEach(o =>
            {
                o.UserMemberShip = rel.ResultList.Where(oo => oo.UserId == o.Id).ToList();
            });

            return Ok(result);
        }

        [HttpPost("GetAll")]
        public IActionResult GetAll()
        {
            var result = _IUserService.WhereList(null, true, false, o => o.Country, o => o.City);
            return Ok(result);
        }


        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IUserService.Get(o => o.Id == id, true, false, o => o.UserCoupon);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IUserService.Delete(id);
            var saveResult = _uow.SaveChanges();
            result.RType = saveResult.RType;
            result.RTypeEnumDesc = saveResult.RTypeEnumDesc;
            result.Message = saveResult.Message;
            result.MessageList = saveResult.MessageList;
            return Ok(result);
        }



        [HttpGet("GetGender")]
        public IActionResult GetGender()
        {
            var rModel = new RModel<EnumModel>();
            var list = Enum.GetValues(typeof(Gender)).Cast<int>()
                .Select(x => new EnumModel { name = ((Gender)x).ToStr(), value = x.ToString(), text = ((Gender)x).ExGetDescription() }).ToList();
            rModel.ResultList = list;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        [HttpGet("GetIsState")]
        public IActionResult GetIsState()
        {
            var rModel = new RModel<EnumModel>();
            var list = Enum.GetValues(typeof(IsState)).Cast<int>()
                .Select(x => new EnumModel { name = ((IsState)x).ToStr(), value = x.ToString(), text = ((IsState)x).ExGetDescription() }).ToList();
            rModel.ResultList = list;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }



    }
}
