public interface IPartnerDocumentService : IGenericRepo<PartnerDocument> { }
public class PartnerDocumentService : GenericRepo<myDBContext, PartnerDocument>, IPartnerDocumentService
{
    public PartnerDocumentService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

