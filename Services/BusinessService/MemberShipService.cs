public interface IMemberShipService : IGenericRepo<MemberShip> { }
public class MemberShipService : GenericRepo<myDBContext, MemberShip>, IMemberShipService
{
    public MemberShipService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

