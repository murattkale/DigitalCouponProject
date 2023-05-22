using System.ComponentModel.DataAnnotations;

public partial class CouponMemberShip : BaseModel
{
    public CouponMemberShip()
    {



    }


    [Required]
    public int CouponId { get; set; }
    public virtual Coupon Coupon { get; set; }

    [Required]
    public int MemberShipId { get; set; }
    public virtual MemberShip MemberShip { get; set; }


}
