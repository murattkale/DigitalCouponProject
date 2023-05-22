using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


public partial class LangText : BaseModel
{
    public LangText()
    {

    }


    [DisplayName("Dil")]
    [Required()] public int LangId { get; set; }
    public virtual Lang Lang { get; set; }



    [DisplayName("TextValue")]
    public string TextValue { get; set; } = "";

    [DisplayName("TextDesc")]
    public string TextDesc { get; set; } = "";


    public virtual BusinessType BusinessType { get; set; }
    public int? BusinessTypeId { get; set; } 

    public virtual Partner Partner { get; set; }
    public int? PartnerId { get; set; }

    public virtual Coupon Coupon { get; set; }
    public int? CouponId { get; set; }

    public virtual MemberShip MemberShip { get; set; }
    public int? MemberShipId { get; set; }

    public virtual ContentPage ContentPage { get; set; }
    public int? ContentPageId { get; set; }

    public virtual LangDisplay LangDisplay { get; set; }
    public int? LangDisplayId { get; set; }


}

