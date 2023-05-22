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
    public class PartnerController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IPartnerService _IPartnerService;
        IBusinessTypePartnerService _IBusinessTypePartnerService;
        IPartnerDocumentService _IPartnerDocumentService;
        ISiteConfigService _ISiteConfigService;
        IUserService _IUserService;
        ILangTextService _ILangTextService;

        public PartnerController(IUnitOfWork<myDBContext> _uow, IPartnerService _IPartnerService, IBusinessTypePartnerService ıBusinessTypePartnerService, IPartnerDocumentService ıPartnerDocumentService, ISiteConfigService ıSiteConfigService, IUserService ıUserService, ILangTextService ıLangTextService)
        {
            this._uow = _uow;
            this._IPartnerService = _IPartnerService;
            this._IBusinessTypePartnerService = ıBusinessTypePartnerService;
            _IPartnerDocumentService = ıPartnerDocumentService;
            _ISiteConfigService = ıSiteConfigService;
            _IUserService = ıUserService;
            _ILangTextService = ıLangTextService;
        }


        [HttpPost("PartnerLocationUpdate")]
        public IActionResult PartnerLocationUpdate(LocationModel postModel)
        {
            var result = _IPartnerService.Get(o => o.Id == postModel.Id, false);
            if (result != null)
            {
                result.ResultRow.Location = postModel.Location;
                _IPartnerService.Update(result.ResultRow);
                var saveResult = _uow.SaveChanges();
                result.RType = saveResult.RType;
                result.RTypeEnumDesc = saveResult.RTypeEnumDesc;
                result.Message = saveResult.Message;
                result.MessageList = saveResult.MessageList;
            }
            return Ok(postModel.Location);
        }


        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(Partner postModel)
        {
            //if (postModel.Id < 1)
            //    postModel.IsState = IsState.Pending;
            var result = _IPartnerService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();

            if (saveResult.RType == RType.OK)
            {
                postModel.LangText.ToList().ForEach(o =>
                {
                    o.PartnerId = result.ResultRow.Id;
                });
                var langInsert = _ILangTextService.InsertOrUpdateBulk(postModel.LangText.ToList());
                saveResult = _uow.SaveChanges();
            }

            result.RType = saveResult.RType;
            result.RTypeEnumDesc = saveResult.RTypeEnumDesc;
            result.Message = saveResult.Message;
            result.MessageList = saveResult.MessageList;
            return Ok(result);
        }

        [HttpPost("InsertOrUpdateBusiness")]
        public IActionResult InsertOrUpdateBusiness(Partner postModel)
        {
            var controlRes = _IPartnerService.Get(o => o.Name == postModel.Name || o.Mail == postModel.Mail);
            if (controlRes.ResultRow == null)
            {
                if (postModel.Id < 1)
                    postModel.IsState = IsState.Pending;

                var result = _IPartnerService.InsertOrUpdate(postModel);
                var saveResult = _uow.SaveChanges();
                result.RType = saveResult.RType;
                result.RTypeEnumDesc = saveResult.RTypeEnumDesc;
                result.Message = saveResult.Message;
                result.MessageList = saveResult.MessageList;

                var userControl = _IUserService.Get(o => o.Mail == postModel.Mail && o.Pass == postModel.Pass);

                if (userControl.ResultRow == null)
                {
                    userControl.ResultRow = new User()
                    {
                        Name = postModel.Name,
                        Mail = postModel.Mail,
                        Pass = postModel.Pass,
                        CountryId = postModel.CountryId,
                        PartnerId = result.ResultRow.Id,
                        IsState = IsState.Pending,
                        UserName = postModel.Mail,
                        Phone = postModel.Phone,
                        Adress = postModel.Adress,
                        Gender = Gender.Male,
                    };
                }
                else
                {
                    userControl.ResultRow.PartnerId = result.ResultRow.Id;
                }

                var userInsert = _IUserService.InsertOrUpdate(userControl.ResultRow);
                var saveUser = _uow.SaveChanges();



                var bid = postModel.BusinessTypeId.ToInt();
                var resultBusiness = _IBusinessTypePartnerService.Add(
                    new BusinessTypePartner()
                    {
                        BusinessTypeId = bid,
                        PartnerId = result.ResultRow.Id
                    }); ;
                var saveBusiness = _uow.SaveChanges();



                return Ok(result);
            }
            return Ok(null);
        }


        [HttpGet("GetSelect")]
        public IActionResult GetSelect()
        {
            var rModel = new RModel<EnumModel>();
            var result = _IPartnerService.Where().Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }


        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<Partner> param)
        {
            var result = _IPartnerService.GetPaging(o => param.IsBool != null ? param.IsBool == o.IsPopuler : true, true, param, false, o => o.Country, o => o.City);
            var Ids = result.ResultPaging.data.Select(o => o.Id).ToList();
            var rel = _IBusinessTypePartnerService.WhereList(o => Ids.Contains(o.PartnerId), true, false, o => o.BusinessType);
            result.ResultPaging.data.ForEach(o =>
            {
                o.BusinessTypePartner = rel.ResultList.Where(oo => oo.PartnerId == o.Id).ToList();
            });

            return Ok(result);
        }




        [HttpGet("GetSearch")]
        public IActionResult GetSearch(string term)
        {
            term = term.Trim();

            var result = _IPartnerService.WhereList(o => o.IsState == IsState.Active && o.Name.Contains(term), true, false,
                o => o.Country, o => o.City, o => o.PartnerDocument);
            result.ResultList = result.ResultList.Where(o => o.LASTEXPIREDDATE > 0).ToList();
            var config = _ISiteConfigService.Where().Result.FirstOrDefault();

            result.ResultList.ForEach(oo =>
            {
                if (oo.PartnerDocument.Count > 0)
                {
                    var dd = oo.PartnerDocument?.Select(o => o.Link.SetImage(config.ImageUrl)).ToList();
                    oo.Documents = dd;
                }
            });


            return Ok(result);
        }


        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var config = _ISiteConfigService.Where().Result.FirstOrDefault();
            var result = _IPartnerService.Get(o => o.Id == id, true, false,
                o => o.Country, o => o.City, o => o.PartnerDocument, o => o.UserPartnerFavorites, o => o.LangText);
            if (result.ResultRow.PartnerDocument.Count > 0)
            {
                var dd = result.ResultRow.PartnerDocument.Select(o => o.Link.SetImage(config.ImageUrl)).ToList();
                result.ResultRow.Documents = dd;
            }

            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IPartnerService.Delete(id);
            var saveResult = _uow.SaveChanges();
            result.RType = saveResult.RType;
            result.RTypeEnumDesc = saveResult.RTypeEnumDesc;
            result.Message = saveResult.Message;
            result.MessageList = saveResult.MessageList;
            return Ok(result);
        }






        [HttpPost("GetPartnerDocument")]
        public IActionResult GetPartnerDocument(DTParameters<PartnerDocument> param, int selectid)
        {
            var result = _IPartnerDocumentService.GetPaging(o => o.PartnerId == selectid, true, param);
            result.ResultPaging.data = result.ResultPaging.data.OrderBy(o => o.OrderNo).ToList();
            return Ok(result);
        }



        [HttpPost]
        public IActionResult DeleteImage(int id)
        {
            var result = _IPartnerDocumentService.Where(o => o.Id == id).Result.FirstOrDefault();
            //var path = this.GetPathAndFilename(result.Link);
            //if (System.IO.File.Exists(path))
            //    System.IO.File.Delete(path);
            _IPartnerDocumentService.Delete(result.Id);
            var res = _uow.SaveChanges();
            return Ok(res);
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


        [HttpGet("GetActiveMonths")]
        public IActionResult GetActiveMonths()
        {
            var rModel = new RModel<EnumModel>();
            var list = Enum.GetValues(typeof(ActiveMonths)).Cast<int>()
                .Select(x => new EnumModel { name = ((ActiveMonths)x).ToStr(), value = x.ToString(), text = ((ActiveMonths)x).ExGetDescription() }).ToList();
            rModel.ResultList = list;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }



        [HttpGet("GetPopuler")]
        public IActionResult GetPopuler()
        {
            var result = _IPartnerService.WhereList(o => o.IsPopuler == true).ResultList.Select(o => new { PartnerId = o.Id }).ToList();
            return Ok(result);
        }


    }
}
