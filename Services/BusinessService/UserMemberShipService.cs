public interface IUserMemberShipService : IGenericRepo<UserMemberShip> { }
public class UserMemberShipService : GenericRepo<myDBContext, UserMemberShip>, IUserMemberShipService
{

    public UserMemberShipService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

