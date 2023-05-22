public interface ICouponService : IGenericRepo<Coupon> { }
public class CouponService : GenericRepo<myDBContext, Coupon>, ICouponService
{
    public CouponService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

