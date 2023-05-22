using System.ComponentModel.DataAnnotations;

public partial class UserCoupon : BaseModel
{
    public UserCoupon()
    {

    }

    [Required]
    public int UserId { get; set; }
    public virtual User User { get; set; }

    [Required]
    public int CouponId { get; set; }
    public virtual Coupon Coupon { get; set; }



}
