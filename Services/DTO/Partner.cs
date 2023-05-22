using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


public partial class PartnerBusinessTypeFavories
{
    public List<Partner> PartnerList { get; set; }
    public BusinessType BusinessType { get; set; }
}
public partial class Partner : BaseModel
{
    public Partner()
    {
        PartnerDocument = new HashSet<PartnerDocument>();
        BusinessTypePartner = new HashSet<BusinessTypePartner>();
        PartnerCoupon = new HashSet<PartnerCoupon>();
        UserPartnerFavorites = new HashSet<UserPartnerFavorites>();
        User = new HashSet<User>();
        LangText = new HashSet<LangText>();

    }



    [Description("Status")]
    public IsState IsState { get; set; }

    public bool? IsPopuler { get; set; }


    [NotMapped]
    public string IsStateName { get { return IsState.ExGetDescription(); } }

    [NotMapped]
    public List<EnumModel> IsStateList = Enum.GetValues(typeof(IsState)).Cast<int>().Select(x => new EnumModel { name = ((IsState)x).ToStr(), value = x.ToString(), text = ((IsState)x).ExGetDescription() }).ToList();



    public ActiveMonths ActiveMonths { get; set; }


    [NotMapped]
    public DateTime EXPIREDDATE
    {
        get
        {
            var addMonths = CreaDate.AddMonths((int)ActiveMonths);
            return addMonths;
        }
    }
    [NotMapped]
    public int? LASTEXPIREDDATE
    {
        get
        {
            var result = EXPIREDDATE < DateTime.Now || EXPIREDDATE == null ? 0 : DateTime.Now.GetMonthsBetween(EXPIREDDATE);
            return result;
        }
    }


    [Required]
    public string Name { get; set; }

    [Required]
    public string BusinessLegalName { get; set; }

    [Required]
    public string Phone { get; set; }

    [Required]
    public string Mail { get; set; }
    public string ExecutiveName { get; set; }



    public string TaxAdmin { get; set; }
    public string TaxNumber { get; set; }
    public string Website { get; set; }
    public string Instagram { get; set; }
    public string Facebook { get; set; }
    public string Youtube { get; set; }


    public int? CountryId { get; set; }
    public virtual Country Country { get; set; }

    public int? CityId { get; set; }
    public virtual City City { get; set; }


    [DataType("text")]
    public string AboutText { get; set; }


    public virtual ICollection<LangText> LangText { get; set; }


    [DataType("Location")]
    public string Location { get; set; }


    public string Adress { get; set; }

    [NotMapped]
    public string Pass { get; set; }


    [NotMapped]
    public List<EnumModel> ActiveMonthsList = Enum.GetValues(typeof(ActiveMonths)).Cast<int>().Select(x => new EnumModel { name = ((ActiveMonths)x).ToStr(), value = x.ToString(), text = ((ActiveMonths)x).ExGetDescription() }).ToList();


    [NotMapped]
    public new List<string> BusinessTypePartnerNames
    {
        get
        {
            var variable = BusinessTypePartner?.Count > 0 ? BusinessTypePartner?.Select(o => o.BusinessType?.Name)?.ToList() : new List<string>();
            return variable;
        }
    }



    [NotMapped]
    public string ImageUrl
    {
        get
        {
            var variable = PartnerDocument?.Count > 0 ? PartnerDocument.FirstOrDefault()?.Link : "";
            return variable;
        }
    }

    [NotMapped]
    public string MemberShipName { get; set; }

    [NotMapped]
    public List<string> Documents { get; set; }

    [NotMapped]
    public int? BusinessTypeId { get; set; }

    [NotMapped]
    public List<ParentChild> BusinessType = new List<ParentChild>();





    public virtual ICollection<PartnerDocument> PartnerDocument { get; set; }
    public virtual ICollection<BusinessTypePartner> BusinessTypePartner { get; set; }
    public virtual ICollection<PartnerCoupon> PartnerCoupon { get; set; }
    public virtual ICollection<UserPartnerFavorites> UserPartnerFavorites { get; set; }
    public virtual ICollection<User> User { get; set; }



}

//public class BusinessTypeLangText
//{
//    public int Id { get; set; }
//    public TextValue ParentId { get; set; }

//    public List<LangText> LangText { get; set; }


//}




public enum ActiveMonths
{
    [Description("3")]
    Tree = 3,
    [Description("6")]
    Six = 6,
    [Description("9")]
    Nine = 9,
    [Description("12")]
    Twelve = 12


}