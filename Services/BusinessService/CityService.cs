public interface ICityService : IGenericRepo<City> { }
public class CityService : GenericRepo<myDBContext, City>, ICityService
{
    public CityService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

