public interface IPartnerCouponService : IGenericRepo<PartnerCoupon> { }
public class PartnerCouponService : GenericRepo<myDBContext, PartnerCoupon>, IPartnerCouponService
{

    public PartnerCouponService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

