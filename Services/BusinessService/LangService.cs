public interface ILangService : IGenericRepo<Lang> { }
public class LangService : GenericRepo<myDBContext, Lang>, ILangService
{
    public LangService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

