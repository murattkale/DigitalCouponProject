public interface IBusinessTypeMemberShipService : IGenericRepo<BusinessTypeMemberShip> { }
public class BusinessTypeMemberShipService : GenericRepo<myDBContext, BusinessTypeMemberShip>, IBusinessTypeMemberShipService
{

    public BusinessTypeMemberShipService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

