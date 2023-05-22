public interface ILangDisplayService : IGenericRepo<LangDisplay> { }
public class LangDisplayService : GenericRepo<myDBContext, LangDisplay>, ILangDisplayService
{
    public LangDisplayService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

