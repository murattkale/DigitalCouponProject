
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public static class SessionRequest
{
    static IHttpContextAccessor _IHttpContextAccessor;

    public static void Configure(IHttpContextAccessor __IHttpContextAccessor)
    {
        _IHttpContextAccessor = __IHttpContextAccessor;
    }

    public static HttpContext _HttpContext => _IHttpContextAccessor.HttpContext;


    //public static User _User => _IHttpContextAccessor.HttpContext.Request.Cookies["_user"].Deserialize<User>("_user");
    public static User _User => _IHttpContextAccessor.HttpContext.Session.Get<User>("_user");
    public static string Error => _IHttpContextAccessor.HttpContext.Session.Get<string>("error");

    public static SiteConfig config => _IHttpContextAccessor.HttpContext.Session.Get<SiteConfig>("config");

    public static int LanguageId => _IHttpContextAccessor.HttpContext.Session.GetInt32("LanguageId") ?? 1;

    public static List<LangDisplay> _LangDisplay => _IHttpContextAccessor.HttpContext.Session.Get<List<LangDisplay>>("_LangDisplay");
    public static List<Lang> Languages => _IHttpContextAccessor.HttpContext.Session.Get<List<Lang>>("Languages");


    public static string Trans(this string ParamName)
    {
        var lang = _LangDisplay.FirstOrDefault(o => o.LangText.Any(oo=>oo.LangId == LanguageId) && o.ParamName == ParamName);
        var text = ParamName;
        if (lang != null && lang.LangText.Any())
        {
            return lang.LangText.FirstOrDefault().TextValue;
        }
        else
        {
            //lang = new LangDisplay()
            //{
            //    ParamName = ParamName,
            //    Name_1 = text,
            //};
            ////save yazılacak

        }
        return text;
    }

    public static string SetImage(this string ImageUrl)
    {
        return !string.IsNullOrEmpty(ImageUrl) ? _IHttpContextAccessor.HttpContext.Session.Get<SiteConfig>("config").ImageUrl + "fileupload/UserFiles/Folders/" + ImageUrl : "";
    }

 

}




