using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

public partial class MemberShip : BaseModel
{
    public MemberShip()
    {
        BusinessTypeMemberShip = new HashSet<BusinessTypeMemberShip>();
        CouponMemberShip = new HashSet<CouponMemberShip>();
        UserMemberShip = new HashSet<UserMemberShip>();
        UserCard = new HashSet<UserCard>();
        LangText = new HashSet<LangText>();



    }

    [Required]
    public string Name { get; set; }

    //[Required]
    public string DisplayText { get; set; }

    public string Desc { get; set; }

    public virtual ICollection<LangText> LangText { get; set; }


    [Description("Price Euro")]
    public decimal PriceEuro { get; set; }

    [Description("Price TL")]
    public decimal PriceTR { get; set; }


    //public string Currency { get; set; }



    [DataType("number")]
    public int? ValidMonths { get; set; }


    [NotMapped]
    public new List<string> BusinessTypeMemberShipNames
    {
        get
        {
            var variable = BusinessTypeMemberShip.Count > 0 ? BusinessTypeMemberShip?.Select(o => o.BusinessType?.Name)?.ToList() : new List<string>();
            return variable;
        }
    }

    [NotMapped]
    public new List<string> BusinessTypeMemberShipIcon
    {
        get
        {
            var variable = BusinessTypeMemberShip.Count > 0 ? BusinessTypeMemberShip?.Select(o => o.BusinessType?.IconMain)?.ToList() : new List<string>();
            return variable;
        }
    }

    [NotMapped]
    public List<BusinessType> BusinessTypeList { get; set; }

    [DataType("SingleDocument")]
    public string ImageUrl { get; set; }

    public string BackColor1 { get; set; }

    public string BackColor2 { get; set; }

    [NotMapped]
    public List<UserMemberShipPartnerCouponHistory> UserMemberShipPartnerCouponHistory { get; set; }


    public virtual ICollection<BusinessTypeMemberShip> BusinessTypeMemberShip { get; set; }
    public virtual ICollection<CouponMemberShip> CouponMemberShip { get; set; }
    public virtual ICollection<UserMemberShip> UserMemberShip { get; set; }
    public virtual ICollection<UserCard> UserCard { get; set; }



}
public class UserMemberShipPartnerCouponHistory
{
    public string PartnerName { get; set; }
    public string CouponName { get; set; }
    public string CouponTypeName { get; set; }
    public DateTime UserCouponCreaDate { get; set; }
    public DateTime CouponCreaDate { get; set; }
    public int? Discount { get; set; }
    public string InfoText { get; set; }

}


