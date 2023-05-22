public interface ICountryService : IGenericRepo<Country> { }
public class CountryService : GenericRepo<myDBContext, Country>, ICountryService
{
    public CountryService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

