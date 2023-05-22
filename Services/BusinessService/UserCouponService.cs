public interface IUserCouponService : IGenericRepo<UserCoupon> { }
public class UserCouponService : GenericRepo<myDBContext, UserCoupon>, IUserCouponService
{

    public UserCouponService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

