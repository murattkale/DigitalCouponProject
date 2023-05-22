using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

public partial class ContentPage : BaseModel
{
    public ContentPage()
    {
        Documents = new HashSet<Documents>();
        Gallery = new HashSet<Documents>();
        Childs = new HashSet<ContentPage>();
        OrjChild = new HashSet<ContentPage>();
        LangText = new HashSet<LangText>();
    }

    //1. Sayfa Yapısı

    [DisplayName("Name")]
    [Required()]
    public string Name { get; set; }



    [DisplayName("Parent Category")]
    public int? ParentId { get; set; }

    [DisplayName("Parent List")]
    public virtual ContentPage Parent { get; set; }


    [DisplayName("Content Type")]
    [Required()]
    public ContentTypes ContentTypes { get; set; }

    [NotMapped]
    public string ContentTypesName { get { return ContentTypes.ExGetDescription(); } }


    [NotMapped]
    public List<EnumModel> ContentTypesList = Enum.GetValues(typeof(ContentTypes)).Cast<int>().Select(x => new EnumModel { name = ((ContentTypes)x).ToStr(), value = x.ToString(), text = ((ContentTypes)x).ExGetDescription() }).ToList();



    [DisplayName("Template Type")]
    [Required()]
    public TemplateType TemplateType { get; set; }

    [NotMapped]
    public List<EnumModel> TemplateTypeList = Enum.GetValues(typeof(TemplateType)).Cast<int>().Select(x => new EnumModel { name = ((TemplateType)x).ToStr(), value = x.ToString(), text = ((TemplateType)x).ExGetDescription() }).ToList();




    [NotMapped]
    public string TemplateTypeName { get { return TemplateType.ExGetDescription(); } }



    [DisplayName("Page Url")]
    public string Link { get; set; }



    [DisplayName("External Url")]
    public string ExternalLink { get; set; }




    [DisplayName("Orj Id")]
    public int? OrjId { get; set; }


    [DisplayName("Orj")]
    public virtual ContentPage Orj { get; set; }






    [DataType("SingleDocument")]
    [DisplayName("Thumb Image")]
    public Documents ThumbImage { get; set; }

    [DataType("SingleDocument")]
    [DisplayName("Picture Image")]
    public Documents Picture { get; set; }

    [DataType("SingleDocument")]
    [DisplayName("Banner Image")]
    public Documents BannerImage { get; set; }


    [DisplayName("Banner Text")]
    public string BannerText { get; set; }


    [DisplayName("Banner Button Text")]
    public string BannerButtonText { get; set; }


    [DisplayName("Button Text")]
    public string ButtonText { get; set; }

    [DisplayName("Button Title")]
    public string ButtonTitle { get; set; }


    [DisplayName("Button Link")]
    public string ButtonLink { get; set; }


    [DataType("text")]
    [DisplayName("Description")]
    public string Description { get; set; }


    [DataType("text")]
    [DisplayName("Short Description")]
    public string ContentShort { get; set; }


    [DataType("text")]
    [DisplayName("Content Text")]
    public string ContentData { get; set; }


    public virtual ICollection<LangText> LangText { get; set; }


    [DisplayName("Video Link")]
    public string VideoLink { get; set; }


    [DisplayName("Sub Category")]
    public bool? IsSubMenu { get; set; }


    [DisplayName("Is Form")]
    public bool? IsForm { get; set; }


    [DisplayName("Gallery")]
    public bool? IsGallery { get; set; }


    [DisplayName("IsMaps")]
    public bool? IsMap { get; set; }

    [DataType("text")]
    [DisplayName("Maps")]
    public string Map { get; set; }

    //3. Sayfa Ayarları


    [DisplayName("Is Parent Menu")]
    public bool? IsHeaderMenu { get; set; }


    [DisplayName("Is Sub Menu")]
    public bool? IsFooterMenu { get; set; }


    [DisplayName("Hamburger Menü")]
    public bool? IsHamburgerMenu { get; set; }


    [DisplayName("Is Side Menu")]
    public bool? IsSideMenu { get; set; }



    [DisplayName("Meta Title")]
    public string MetaTitle { get; set; }


    [DisplayName("Meta Keywords")]
    public string MetaKeyword { get; set; }


    [DisplayName("Meta Description")]
    public string MetaDescription { get; set; }


    [DisplayName("Order No")]
    public int? ContentOrderNo { get; set; }




    [DisplayName("Start Date")]
    public DateTime? StartDate { get; set; }
    [DisplayName("End Date")]
    public DateTime? EndDate { get; set; }





    [DisplayName("Gallery")]
    public virtual ICollection<Documents> Gallery { get; set; }


    [DisplayName("Document")]
    public virtual ICollection<Documents> Documents { get; set; }


    [DisplayName("Sub Content")]
    public virtual ICollection<ContentPage> Childs { get; set; }

    [DisplayName("Orj Child")]
    public virtual ICollection<ContentPage> OrjChild { get; set; }




    [DisplayName("Is Publish")]
    public bool? IsPublish { get; set; }


    [DisplayName("Is Click")]
    public bool? IsClick { get; set; }





}






public enum TemplateType : int
{
    [Description("Default Template")]
    DefaultTemplate = 0,
    [Description("About Us")]
    AboutUs = 1,
    [Description("Terms of Use")]
    TermsofUse = 2,
    [Description("Privacy Policy")]
    PrivacyPolicy = 3,


    [Description("text area 1")]
    TextArea1 = 4,

    [Description("text area 2")]
    TextArea2 = 5,

    [Description("text area 3")]
    TextArea3 = 6,

    [Description("Reset Mail Template")]
    ResetTemplate = 7,

    [Description("Welcome Mail Template")]
    WelcomeTemplate = 8,

    [Description("PartnerShip Mail Template")]
    PartnerShipTemplate = 9,

    [Description("Distance Sales Agreement")]
    DistanceSalesAgreement = 10,

    [Description("Personal Data and Cookie Policy")]
    PersonalDataandCookiePolicy = 11,

    [Description("Individual Membership Agreement")]
    IndividualMembershipAgreement = 12,


}



public enum ContentTypes : int
{
    [Description("Default Page")]
    DefaultPage = 1,

    [Description("Static HTML")]
    HtmlRaw = 3,


}

