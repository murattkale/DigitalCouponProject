public interface ISiteConfigService : IGenericRepo<SiteConfig> { }
public class SiteConfigService : GenericRepo<myDBContext, SiteConfig>, ISiteConfigService
{
    public SiteConfigService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

