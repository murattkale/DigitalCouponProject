public interface IContentPageService : IGenericRepo<ContentPage> { }
public class ContentPageService : GenericRepo<myDBContext, ContentPage>, IContentPageService
{
    public ContentPageService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

