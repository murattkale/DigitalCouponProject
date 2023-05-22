using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class UserPartnerFavoritesController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IUserPartnerFavoritesService _IUserPartnerFavoritesService;
        IPartnerService _IPartnerService;
        ISiteConfigService _ISiteConfigService;
        IBusinessTypeService _IBusinessTypeService;

        public UserPartnerFavoritesController(IUnitOfWork<myDBContext> _uow, IUserPartnerFavoritesService _IUserPartnerFavoritesService, IPartnerService ıPartnerService, ISiteConfigService ıSiteConfigService, IBusinessTypeService ıBusinessTypeService)
        {
            this._uow = _uow;
            this._IUserPartnerFavoritesService = _IUserPartnerFavoritesService;
            _IPartnerService = ıPartnerService;
            _ISiteConfigService = ıSiteConfigService;
            _IBusinessTypeService = ıBusinessTypeService;
        }


        [HttpGet("setData")]
        public IActionResult setData(int id1, int id2, string type)
        {
            if (type == "add")
            {
                _IUserPartnerFavoritesService.Add(new UserPartnerFavorites() { UserId = id1, PartnerId = id2 });
            }
            else
            {
                var dp = _IUserPartnerFavoritesService.Where(o => o.UserId == id1 && o.PartnerId == id2).Result.ToList();
                _IUserPartnerFavoritesService.DeleteBulk(dp);
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
            var dp = _IUserPartnerFavoritesService.Where(o => o.UserId == id1, true, false, o => o.Partner).Result.ToList();
            var rows = _IPartnerService.Where().Result.ToList().Select(o =>
               new EnumModel
               {
                   value = o.Id.ToStr(),
                   text = o.Name,
                   selected = dp.Any(oo => oo.PartnerId == o.Id)
               }).ToList();
            RModel<EnumModel> res = new RModel<EnumModel>();
            res.ResultList = rows;
            res.RType = RType.OK;
            return Ok(res);
        }


        [HttpGet("getFavories")]
        public IActionResult getFavories(int UserId)
        {
            var res = _IPartnerService.WhereList(o => o.UserPartnerFavorites.Any(oo => oo.UserId == UserId), true, false,
                o => o.BusinessTypePartner, o => o.PartnerDocument);

            var btype = _IBusinessTypeService.WhereList().ResultList;

            var config = _ISiteConfigService.Where().Result.FirstOrDefault();


            res.ResultList.ForEach(oo =>
            {
                var dd = oo.PartnerDocument?.Select(o => o.Link).ToList();
                oo.Documents = dd;
            });

            var result = res.ResultList.GroupBy(oo => oo.BusinessTypePartner.FirstOrDefault().BusinessTypeId).Select(o => new PartnerBusinessTypeFavories
            {
                BusinessType = btype.FirstOrDefault(oo => oo.Id == o.Key.ToInt()),
                PartnerList = o.ToList(),
            }).ToList();


            var list = result.Select(o => new
            {
                title = o.BusinessType.Name + "&" + o.BusinessType.IconFavorite,
                data = o.PartnerList.Select(oo => oo.Name + "&" + oo.ImageUrl + "&" + oo.Id).ToArray(),
                dataCustom = o.PartnerList.Select(oo => new { PartnerId = oo.Id, ImageUrl = oo.ImageUrl, Name = oo.Name, BusBusinessLegalName = oo.BusinessLegalName }).ToList(),
            }).ToList();

            return Ok(list);
        }




    }
}
