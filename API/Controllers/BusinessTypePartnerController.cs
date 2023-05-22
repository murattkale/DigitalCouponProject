using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class BusinessTypePartnerController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IBusinessTypePartnerService _IBusinessTypePartnerService;
        IBusinessTypeService _IBusinessTypeService;
        IBusinessTypeMemberShipService _IBusinessTypeMemberShipService;
        IPartnerService _IPartnerService;
        ISiteConfigService _ISiteConfigService;

        public BusinessTypePartnerController(IUnitOfWork<myDBContext> _uow, IBusinessTypePartnerService _IBusinessTypePartnerService, IBusinessTypeService ıBusinessTypeService, IBusinessTypeMemberShipService ıBusinessTypeMemberShipService, IPartnerService ıPartnerService, ISiteConfigService ıSiteConfigService)
        {
            this._uow = _uow;
            this._IBusinessTypePartnerService = _IBusinessTypePartnerService;
            _IBusinessTypeService = ıBusinessTypeService;
            _IBusinessTypeMemberShipService = ıBusinessTypeMemberShipService;
            _IPartnerService = ıPartnerService;
            _ISiteConfigService = ıSiteConfigService;
        }


        [HttpGet("setData")]
        public IActionResult setData(int id1, int id2, string type)
        {
            var dp = _IBusinessTypePartnerService.Where(o => o.PartnerId == id1 && o.BusinessTypeId == id2).Result.ToList();


            if (dp.Any())
            {
                return Ok("duplicate");
            }

            if (type == "add")
            {
                _IBusinessTypePartnerService.Add(new BusinessTypePartner() { PartnerId = id1, BusinessTypeId = id2 });
            }
            else
            {
                dp.ForEach(o =>
                {
                    _IBusinessTypePartnerService.Delete(o);
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
            var dp = _IBusinessTypePartnerService.Where(o => o.PartnerId == id1).Result.ToList();
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

        [HttpGet("GetAll")]
        public IActionResult GetAll(int? MemberShipId, int? BusinessTypeId)
        {
            var relation = _IBusinessTypeMemberShipService.WhereList(o => o.MemberShipId == MemberShipId, true, false, o => o.MemberShip, o => o.BusinessType);
            var Ids = relation.ResultList.Where(oo => BusinessTypeId > 0 ? oo.BusinessType.ParentId == BusinessTypeId : true).Select(o => o.BusinessTypeId).Distinct().ToList();

            var partnerRelationList = _IBusinessTypePartnerService.WhereList(o => Ids.Contains(o.BusinessTypeId), true, false);

            var partnerIds = partnerRelationList.ResultList.Select(o => o.PartnerId).Distinct().ToList();
            var BusinessTypeIds = partnerRelationList.ResultList.Select(o => o.BusinessTypeId).Distinct().ToList();
            var bt = _IBusinessTypeService.WhereList(o => BusinessTypeIds.Contains(o.Id), true, false, o => o.LangText).ResultList;

            var result = _IPartnerService.WhereList(o => o.IsState == IsState.Active && partnerIds.Contains(o.Id), true, false,
                o => o.Country, o => o.City, o => o.PartnerDocument, o => o.UserPartnerFavorites, o => o.LangText);
            result.ResultList = result.ResultList.Where(o => o.LASTEXPIREDDATE > 0).ToList();


            var config = _ISiteConfigService.Where().Result.FirstOrDefault();

            result.ResultList.ForEach(oo =>
            {
                var BusinessTypeIdsList = partnerRelationList.ResultList.Where(o => o.PartnerId == oo.Id).Select(o => o.BusinessTypeId).Distinct().ToList();
                oo.BusinessType = bt.Where(o =>  BusinessTypeIdsList.Contains(o.Id))
                .Select(l => new ParentChild
                {
                    Id = l.Id,
                    ParentId = l.ParentId,
                    //Childs = bt.Where(o => o.ParentId == l.Id).Select(c => new BusinessTypeLangText()
                    //{
                    //    Id = c.Id,
                    //    LangText = c.LangText.Select(i => new LangText() { LangId = i.LangId, TextValue = i.TextValue, TextDesc = i.TextDesc }).ToList()
                    //}).ToList(),
                    //LangText = l.LangText.Select(i => new LangText() { LangId = i.LangId, TextValue = i.TextValue, TextDesc = i.TextDesc }).ToList()
                }).ToList();

                var dd = oo.PartnerDocument?.Select(o => config.ImageUrl + "fileupload/UserFiles/Folders/" + o.Link).ToList();
                oo.Documents = dd;
                oo.MemberShipName = relation.ResultList.FirstOrDefault().MemberShip.Name;
            });


            return Ok(result);
        }




    }
}
