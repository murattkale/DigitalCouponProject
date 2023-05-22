public interface IDocumentsService : IGenericRepo<Documents> { }
public class DocumentsService : GenericRepo<myDBContext, Documents>, IDocumentsService
{
    public DocumentsService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

