public interface IUserCardService : IGenericRepo<UserCard> { }
public class UserCardService : GenericRepo<myDBContext, UserCard>, IUserCardService
{

    public UserCardService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

