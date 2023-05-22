using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class ContentPageController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IContentPageService _IContentPageService;
        IDocumentsService _IDocumentsService;
        IHostingEnvironment _IHostingEnvironment;
        ILangTextService _ILangTextService;
        ISendMail ISendMail;
        IUserService IUserService;
        ISiteConfigService _ISiteConfigService;

        public ContentPageController(IUnitOfWork<myDBContext> _uow, IContentPageService _IContentPageService, IDocumentsService _IDocumentsService, IHostingEnvironment _IHostingEnvironment, ILangTextService ıLangTextService, ISendMail ıSendMail, IUserService ıUserService, ISiteConfigService ıSiteConfigService)
        {
            this._uow = _uow;
            this._IContentPageService = _IContentPageService;
            this._IDocumentsService = _IDocumentsService;
            this._IHostingEnvironment = _IHostingEnvironment;
            _ILangTextService = ıLangTextService;
            ISendMail = ıSendMail;
            IUserService = ıUserService;
            _ISiteConfigService = ıSiteConfigService;
        }


        [HttpGet("SendMail")]

        public IActionResult SendMail(int UserId, TemplateType TemplateType)
        {
            var content = _IContentPageService.Get(o => o.TemplateType == TemplateType, true, false, o => o.LangText);
            if (content.ResultRow != null)
            {
                var user = IUserService.Get(o => o.Id == UserId, false, false);
                if (user.ResultRow != null)
                {
                    var contentText = user.ResultRow.CountryId == 1 ?
                        content.ResultRow.LangText.FirstOrDefault(o => o.LangId == 2).TextDesc
                      : content.ResultRow.LangText.FirstOrDefault(o => o.LangId == 1).TextDesc;

                    if (TemplateType == TemplateType.ResetTemplate)
                    {
                        user.ResultRow.Pass = Guid.NewGuid().ToString().Substring(0, 6);
                        IUserService.Update(user.ResultRow);
                        _uow.SaveChanges();
                    }

                    contentText = contentText.Replace("[Name]", user.ResultRow.Name);
                    contentText = contentText.Replace("[Surname]", user.ResultRow.Name);
                    contentText = contentText.Replace("[Pass]", user.ResultRow.Pass);
                    contentText = contentText.Replace("[TCKN]", user.ResultRow.TCKN);
                    contentText = contentText.Replace("[Adress]", user.ResultRow.Adress);
                    contentText = contentText.Replace("[Phone]", user.ResultRow.Phone);
                    contentText = contentText.Replace("[Mail]", user.ResultRow.Mail);

                    var config = _ISiteConfigService.WhereList().ResultList.FirstOrDefault();
                    ISendMail.Send(new MailModelCustom
                    {
                        Alicilar = new string[] { user.ResultRow.Mail },
                        //bcc = new string[] { SessionRequest.config.SmtpMail },
                        //cc = null,
                        Icerik = contentText,
                        Konu = (TemplateType).ExGetDescription(),
                        MailGorunenAd = config.MailGorunenAd,
                        SmtpHost = config.SmtpHost,
                        SmtpMail = config.SmtpMail,
                        SmtpMailPass = config.SmtpMailPass,
                        SmtpPort = config.SmtpPort,
                        SmtpSSL = config.SmtpSsl,
                        SmtpUseDefaultCredentials = false,
                    });

                    return Ok("success");
                }
                else
                {
                    return Ok("User Not Found");
                }
            }
            else
            {
                return Ok("Template Not Found");
            }
        }

        //[HttpGet("SearchFilter/{term}")]
        //public IActionResult SearchFilter(string term)
        //{
        //    var result = _IContentPageService.WhereList(o => o.IsPublish == true 
        //    && (o.Name.Contains(term) || o.Description.Contains(term) || o.ContentShort.Contains(term) || o.ContentData.Contains(term)), true, false);
        //    return Ok(result);
        //}




        [HttpGet("GetTemplateAll")]
        public IActionResult GetTemplateAll(int ParentId)
        {
            var result = _IContentPageService.WhereList(o =>
             o.IsPublish == true
            && o.ParentId == ParentId,

            true, false,
                o => o.Gallery,
                o => o.Documents,
                o => o.ThumbImage,
                o => o.Picture,
                o => o.BannerImage,
                o => o.Parent
                );
            return Ok(result);
        }




        [HttpGet("GetAllType")]
        public IActionResult GetAllType(ContentTypes ContentTypes)
        {
            var result = _IContentPageService.WhereList(o => o.IsPublish == true && o.ContentTypes == ContentTypes);
            return Ok(result);
        }

        [HttpGet("GetContent")]
        public IActionResult GetContent(ContentTypes ContentTypes, int LangId)
        {
            var result = _IContentPageService.Get(o => o.LangText.Any(oo => oo.LangId == LangId) && o.IsPublish == true && o.ContentTypes == ContentTypes, true, false, o => o.LangText);
            return Ok(result);
        }



        [HttpGet("GetId")]
        public IActionResult GetId(int Id)
        {
            var result = _IContentPageService.Get(o => o.Id == Id && o.IsPublish == true);
            return Ok(result);
        }

        [HttpGet("GetIdR")]
        public IActionResult GetIdR(int Id)
        {
            var result = _IContentPageService.Get(o => o.Id == Id && o.IsPublish == true, true, false,
                o => o.Childs,
                o => o.Parent,
                o => o.Gallery,
                o => o.Documents,
                o => o.ThumbImage,
                o => o.Picture,
                o => o.BannerImage
                );
            result.ResultRow.Childs = result.ResultRow.Childs.OrderBy(o => o.OrderNo).ToList();
            return Ok(result);
        }

        [HttpGet("GetOrj")]
        public IActionResult GetOrj(int OrjId)
        {
            var result = _IContentPageService.Get(o => o.OrjId == OrjId && o.IsPublish == true);
            return Ok(result);
        }

        [HttpGet("GetOrjR")]
        public IActionResult GetOrjR(int OrjId)
        {
            var result = _IContentPageService.Get(o => o.OrjId == OrjId && o.IsPublish == true, true, false,
                o => o.Childs,
                o => o.Parent,
                o => o.Gallery,
                o => o.Documents,
                o => o.ThumbImage,
                o => o.Picture,
                o => o.BannerImage,
                o => o.Orj
                );
            result.ResultRow.Childs = result.ResultRow.Childs.OrderBy(o => o.OrderNo).ToList();
            return Ok(result);
        }

        [HttpGet("GetLink")]
        public IActionResult GetLink(string link)
        {
            var result = _IContentPageService.Get(o => o.Link == link && o.IsPublish == true);
            return Ok(result);
        }

        [HttpGet("GetLinkR")]
        public IActionResult GetLinkR(string link)
        {
            var result = _IContentPageService.Get(o => o.Link == link && o.IsPublish == true, true, false,
                o => o.Childs,
                o => o.Parent,
                o => o.Gallery,
                o => o.Documents,
                o => o.ThumbImage,
                o => o.Picture,
                o => o.BannerImage,
                o => o.Orj.BannerImage,
                o => o.Orj.Gallery
                );
            if (result.ResultRow.Childs.Count > 0 && result.ResultRow.Orj != null && result.ResultRow.Orj.Childs.Count > 0)
            {
                var resultChild = _IContentPageService.Where(o => o.ParentId == result.ResultRow.OrjId && o.IsPublish == true, true, false,
                o => o.Gallery,
                o => o.Documents,
                o => o.ThumbImage,
                o => o.Picture,
                o => o.BannerImage
                );

                if (resultChild.Result.Count() > 0)
                    result.ResultRow.Childs.ToList().ForEach(o =>
                {
                    o.BannerImage = resultChild.Result.FirstOrDefault(oo => oo.Id == o.OrjId).BannerImage;
                    o.Picture = resultChild.Result.FirstOrDefault(oo => oo.Id == o.OrjId).Picture;
                    o.ThumbImage = resultChild.Result.FirstOrDefault(oo => oo.Id == o.OrjId).ThumbImage;
                    o.Gallery = resultChild.Result.FirstOrDefault(oo => oo.Id == o.OrjId) == null ? new List<Documents>() : resultChild.Result.FirstOrDefault(oo => oo.Id == o.OrjId)?.Gallery;
                    o.Documents = resultChild.Result.FirstOrDefault(oo => oo.Id == o.OrjId) == null ? new List<Documents>() : resultChild.Result.FirstOrDefault(oo => oo.Id == o.OrjId)?.Documents;

                });

                //result.ResultRow.Childs = resultChild.ResultList.OrderBy(o => o.OrderNo).ToList();
            }
            return Ok(result);
        }



        [HttpGet("GetTemplateByEnum")]
        public IActionResult GetTemplateByEnum(ContentTypes? ContentTypes, TemplateType TemplateType, int LanguageId)
        {
            var _ContentTypes = ContentTypes == null ? 0 : ContentTypes;
            var result = _IContentPageService.Get(o =>
            (_ContentTypes > 0 ? o.ContentTypes == _ContentTypes : true)
            && o.IsPublish == true
            && o.TemplateType == TemplateType
            && o.LangText.Any(oo => oo.LangId == LanguageId),

            true, false, o => o.LangText);


            return Ok(result);
        }

        [HttpGet("GetTemplate")]
        public IActionResult GetTemplate(ContentTypes? ContentTypes, TemplateType TemplateType, int ParentId)
        {
            var _ContentTypes = ContentTypes == null ? 0 : ContentTypes;
            var result = _IContentPageService.WhereList(o =>
            (_ContentTypes > 0 ? o.ContentTypes == _ContentTypes : true)
            && o.IsPublish == true
            && o.TemplateType == TemplateType
            && o.ParentId == ParentId,

            true, false,
                o => o.Gallery,
                o => o.Documents,
                o => o.ThumbImage,
                o => o.Picture,
                o => o.BannerImage,
                o => o.Parent
                );



            if (result.ResultList.Count > 0)
            {

                var listId = result.ResultList.Select(o => (int?)o.OrjId).ToList();

                var resultChild = _IContentPageService.Where(o => listId.Contains(o.Id) && o.IsPublish == true, true, false,
               o => o.Gallery,
               o => o.Documents,
               o => o.ThumbImage,
               o => o.Picture,
               o => o.BannerImage

               );



                if (resultChild.Result.Count() > 0)
                    result.ResultList.ForEach(o =>
                    {
                        o.BannerImage = resultChild.Result.FirstOrDefault(oo => oo.Id == o.OrjId)?.BannerImage;
                        o.Picture = resultChild.Result.FirstOrDefault(oo => oo.Id == o.OrjId)?.Picture;
                        o.ThumbImage = resultChild.Result.FirstOrDefault(oo => oo.Id == o.OrjId)?.ThumbImage;
                        o.Gallery = resultChild.Result.FirstOrDefault(oo => oo.Id == o.OrjId) == null ? new List<Documents>() : resultChild.Result.FirstOrDefault(oo => oo.Id == o.OrjId)?.Gallery;
                        o.Documents = resultChild.Result.FirstOrDefault(oo => oo.Id == o.OrjId) == null ? new List<Documents>() : resultChild.Result.FirstOrDefault(oo => oo.Id == o.OrjId)?.Documents;
                    });

                //result.ResultRow.Childs = resultChild.ResultList.OrderBy(o => o.OrderNo).ToList();
            }


            return Ok(result);
        }



        [HttpGet("GetPartialById")]
        public IActionResult GetPartialById(int Id)
        {
            var result = _IContentPageService.Get(o =>
             o.IsPublish == true
            && o.Id == Id,

            true, false,
                o => o.Gallery,
                o => o.Documents,
                o => o.ThumbImage,
                o => o.Picture,
                o => o.BannerImage,
                o => o.Parent
                );
            return Ok(result);
        }


        [HttpPost("ChildDocuments")]
        public IActionResult ChildDocuments(List<int> ids)
        {
            var result = _IContentPageService.WhereList(o => ids.Contains(o.Id) && o.IsPublish == true, true, false,
                o => o.Parent,
                o => o.Gallery,
                o => o.Documents,
                o => o.ThumbImage,
                o => o.Picture,
                o => o.BannerImage,
                o => o.Orj
                );
            return Ok(result);
        }


        [HttpGet("GetHeaderFooter")]
        public IActionResult GetHeaderFooter(bool? IsHeaderMenu, bool? IsFooterMenu)
        {
            var result = _IContentPageService.WhereList(o => o.IsPublish == true
            && (IsHeaderMenu == true ? o.IsHeaderMenu == IsHeaderMenu : true)
            && (IsFooterMenu == true ? o.IsFooterMenu == IsFooterMenu : true)
            , true, false,
                o => o.Childs,
                o => o.Parent
                );
            return Ok(result);
        }




        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _IContentPageService.WhereList(o => o.IsPublish == true, true, false,
                o => o.Childs,
                o => o.Parent,
                o => o.Gallery,
                o => o.Documents,
                o => o.ThumbImage,
                o => o.Picture,
                o => o.BannerImage
                );

            return Ok(result);
        }

        [HttpGet("GetSelect")]
        public IActionResult GetSelect(ContentTypes? ContentTypes, int? LangId)
        {
            var rModel = new RModel<EnumModel>();
            var result = _IContentPageService.Where(o => (LangId != null && LangId > 0 ? o.LangText.Any(oo => oo.LangId == LangId) : true)
            && o.IsPublish == true && (ContentTypes > 0 && ContentTypes != null ? o.ContentTypes == ContentTypes : true), true, false, o => o.LangText).Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        //[HttpGet("GetSelect")]
        //public IActionResult GetSelect(ContentTypes? ContentTypes)
        //{
        //    var rModel = new RModel<EnumModel>();
        //    var result = _IContentPageService.Where(o => o.IsPublish == true && (ContentTypes > 0 ? o.ContentTypes == ContentTypes : true)).Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name }).ToList();
        //    rModel.ResultList = result;
        //    rModel.Result = null;
        //    rModel.RType = RType.OK;
        //    return Ok(rModel);
        //}

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<ContentPage> param)
        {
            var type = param.selectid > 0 ? (ContentTypes)param.selectid : 0;
            var result = _IContentPageService.GetPaging(o => (type > 0 ? o.ContentTypes == type : true), true, param, false, o => o.Parent, o => o.LangText);
            return Ok(result);
        }

        [HttpPost("GetContentPage")]
        public IActionResult GetContentPage()
        {
            var result = _IContentPageService.Where(null, true, false, o => o.ContentTypes).Result.Select(o => new { value = o.Id, text = o.Name }).ToList();
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(ContentPage postModel)
        {
            var result = _IContentPageService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();


            if (saveResult.RType == RType.OK)
            {
                postModel.LangText.ToList().ForEach(o =>
                {
                    o.ContentPageId = result.ResultRow.Id;
                });
                var langInsert = _ILangTextService.InsertOrUpdateBulk(postModel.LangText.ToList());
                saveResult = _uow.SaveChanges();
            }

            result.Message = saveResult.Message;
            result.RType = saveResult.RType;
            result.MessageList = saveResult.MessageList;
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IContentPageService.Get(o => o.Id == id, true, false,
                o => o.Childs,
                o => o.Parent,
                o => o.Gallery,
                o => o.Documents,
                o => o.ThumbImage,
                o => o.Picture,
                o => o.BannerImage,
                o => o.LangText
                );
            if (result.ResultRow != null)
            {
                result.ResultRow.Childs = result.ResultRow.Childs.OrderBy(o => o.OrderNo).ToList();
            }
            return Ok(result);
        }



        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IContentPageService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }

        [HttpPost("UpdateOrder")]
        public IActionResult UpdateOrder(List<OrderUpdateModel> postModel)
        {
            var rModel = new RModel<OrderUpdateModel>();
            if (postModel.Count > 0)
            {
                var type = (ContentTypes)postModel.FirstOrDefault().dataid;
                var rows = _IContentPageService.Where(o => o.ContentTypes == type, false, false).Result.ToList();
                postModel.ForEach(o =>
                {
                    var row = rows.FirstOrDefault(r => r.Id == o.Id);
                    if (row != null)
                    {
                        row.OrderNo = o.OrderNo;
                        _IContentPageService.Update(row);
                        _uow.SaveChanges();
                    }
                });

                rModel.ResultList = postModel;
                rModel.Result = null;
            }

            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        [HttpGet("GetTemplateType")]
        public IActionResult GetTemplateType()
        {
            var rModel = new RModel<EnumModel>();
            var list = Enum.GetValues(typeof(TemplateType)).Cast<int>()
                .Select(x => new EnumModel { name = ((TemplateType)x).ToStr(), value = x.ToString(), text = ((TemplateType)x).ExGetDescription() }).ToList();
            rModel.ResultList = list;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        [HttpGet("GetContentTypes")]
        public IActionResult GetContentTypes()
        {
            var rModel = new RModel<EnumModel>();
            var list = Enum.GetValues(typeof(ContentTypes)).Cast<int>()
                .Select(x => new EnumModel { name = ((ContentTypes)x).ToStr(), value = x.ToString(), text = ((ContentTypes)x).ExGetDescription() }).ToList();
            rModel.ResultList = list;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }




        [HttpPost("GetGallery")]
        public IActionResult GetGallery(DTParameters<Documents> param, int selectid)
        {
            var result = _IDocumentsService.GetPaging(o => o.GalleryId == selectid, true, param);
            result.ResultPaging.data = result.ResultPaging.data.OrderBy(o => o.OrderNo).ToList();
            return Ok(result);
        }

        [HttpPost("GetDocuments")]
        public IActionResult GetDocuments(DTParameters<Documents> param, int selectid)
        {
            var result = _IDocumentsService.GetPaging(o => o.DocumentId == selectid, true, param);
            result.ResultPaging.data = result.ResultPaging.data.OrderBy(o => o.OrderNo).ToList();
            return Ok(result);
        }



        [HttpPost]
        public IActionResult DeleteImage(int id)
        {
            var result = _IDocumentsService.Where(o => o.Id == id).Result.FirstOrDefault();
            //var path = this.GetPathAndFilename(result.Link);
            //if (System.IO.File.Exists(path))
            //    System.IO.File.Delete(path);
            _IDocumentsService.Delete(result.Id);
            var res = _uow.SaveChanges();
            return Ok(res);
        }


        string GetPathAndFilename(string filename)
        {
            string path = this._IHostingEnvironment.WebRootPath + "\\uploads\\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path + filename;
        }




    }
}
