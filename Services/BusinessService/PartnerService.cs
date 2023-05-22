public interface IPartnerService : IGenericRepo<Partner> { }
public class PartnerService : GenericRepo<myDBContext, Partner>, IPartnerService
{
    public PartnerService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

