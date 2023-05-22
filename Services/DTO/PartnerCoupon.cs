using System.ComponentModel.DataAnnotations;

public partial class PartnerCoupon : BaseModel
{
    public PartnerCoupon()
    {

    }

    [Required]
    public int PartnerId { get; set; }
    public virtual Partner Partner { get; set; }

    [Required]
    public int CouponId { get; set; }
    public virtual Coupon Coupon { get; set; }



}
