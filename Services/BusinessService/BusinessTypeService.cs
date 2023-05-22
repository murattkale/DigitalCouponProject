public interface IBusinessTypeService : IGenericRepo<BusinessType> { }
public class BusinessTypeService : GenericRepo<myDBContext, BusinessType>, IBusinessTypeService
{

    public BusinessTypeService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

