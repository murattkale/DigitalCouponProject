public interface ILangTextService : IGenericRepo<LangText> { }
public class LangTextService : GenericRepo<myDBContext, LangText>, ILangTextService
{
    public LangTextService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

