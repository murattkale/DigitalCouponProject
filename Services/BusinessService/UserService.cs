public interface IUserService : IGenericRepo<User> { }
public class UserService : GenericRepo<myDBContext, User>, IUserService
{
    public UserService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

