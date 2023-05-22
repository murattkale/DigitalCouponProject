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
    public class UserMemberShipController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IUserMemberShipService _IUserMemberShipService;
        IMemberShipService _IMemberShipService;
        public UserMemberShipController(IUnitOfWork<myDBContext> _uow, IUserMemberShipService _IUserMemberShipService, IMemberShipService ıMemberShipService)
        {
            this._uow = _uow;
            this._IUserMemberShipService = _IUserMemberShipService;
            _IMemberShipService = ıMemberShipService;
        }


        [HttpGet("setData")]
        public IActionResult setData(int id1, int id2, string type)
        {
            RModel<UserMemberShip> res = new RModel<UserMemberShip>();
            var dp = _IUserMemberShipService.Where(o => o.UserId == id1 && o.MemberShipId == id2, false, false, o => o.MemberShip).Result.ToList();
            if (!dp.Any() || type == "remove")
            {

                if (type == "add")
                {
                    _IUserMemberShipService.Add(new UserMemberShip() { UserId = id1, MemberShipId = id2 });
                }
                else
                {
                    dp.ForEach(o =>
                    {
                        _IUserMemberShipService.Delete(o);
                    });
                }
                var result = _uow.SaveChanges();

                res.RType = result.RType;
                res.Message = result.Message;
            }
            else
            {
                if (res.ResultList.Any(o => DateTime.Now > o.CreaDate.AddMonths(o.MemberShip.ValidMonths.Value)))
                {
                    res.Message = "Expire ValidMonths";
                }
                else
                {
                    res.Message = "duplicate";
                }
                res.RType = RType.Warning;


            }


            return Ok(res);
        }

        [HttpGet("getData")]
        public IActionResult getData(int id1)
        {
            var dp = _IUserMemberShipService.Where(o => o.UserId == id1).Result.ToList();
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


        [HttpGet("GetAll")]
        public IActionResult GetAll(int UserId)
        {
            var result = _IUserMemberShipService.WhereList(o => o.UserId == UserId, true, false, o => o.User, o => o.MemberShip);
            return Ok(result);
        }

    }
}
