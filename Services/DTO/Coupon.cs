using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

public partial class Coupon : BaseModel
{
    public Coupon()
    {
        CouponMemberShip = new HashSet<CouponMemberShip>();
        UserCoupon = new HashSet<UserCoupon>();
        PartnerCoupon = new HashSet<PartnerCoupon>();
        LangText = new HashSet<LangText>();

    }


    [Required]
    public string Name { get; set; }

    public string Code { get; set; }
    public CouponType CouponType { get; set; }

    [DataType("number")]

    public int? Discount { get; set; }


    public DateTime? ExpireDate { get; set; }


    [DataType("text")]
    public string InfoText { get; set; }



    public virtual ICollection<LangText> LangText { get; set; }


    [NotMapped]
    public string CouponTypeName { get { return CouponType.ExGetDescription(); } }

    [NotMapped]
    public List<EnumModel> CouponTypeList = Enum.GetValues(typeof(CouponType)).Cast<int>().Select(x => new EnumModel { name = ((CouponType)x).ToStr(), value = x.ToString(), text = ((CouponType)x).ExGetDescription() }).ToList();


    [NotMapped]
    public new List<string> CouponMemberShipNames
    {
        get
        {
            var variable = CouponMemberShip.Count > 0 ? CouponMemberShip?.Select(o => o.MemberShip?.Name)?.ToList() : new List<string>();
            return variable;
        }
    }


    public virtual ICollection<CouponMemberShip> CouponMemberShip { get; set; }
    public virtual ICollection<UserCoupon> UserCoupon { get; set; }
    public virtual ICollection<PartnerCoupon> PartnerCoupon { get; set; }




}


public enum CouponType : int
{
    [Description("DISCOUNT")]
    DISCOUNT = 1,
    [Description("FREE")]
    FREE = 2,
    [Description("SAVE")]
    SAVE = 3,


}
