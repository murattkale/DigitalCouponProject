public interface ICouponMemberShipService : IGenericRepo<CouponMemberShip> { }
public class CouponMemberShipService : GenericRepo<myDBContext, CouponMemberShip>, ICouponMemberShipService
{
    public CouponMemberShipService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

