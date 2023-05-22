public interface IBusinessTypePartnerService : IGenericRepo<BusinessTypePartner> { }
public class BusinessTypePartnerService : GenericRepo<myDBContext, BusinessTypePartner>, IBusinessTypePartnerService
{

    public BusinessTypePartnerService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

