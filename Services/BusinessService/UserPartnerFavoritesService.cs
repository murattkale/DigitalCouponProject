public interface IUserPartnerFavoritesService : IGenericRepo<UserPartnerFavorites> { }
public class UserPartnerFavoritesService : GenericRepo<myDBContext, UserPartnerFavorites>, IUserPartnerFavoritesService
{

    public UserPartnerFavoritesService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

