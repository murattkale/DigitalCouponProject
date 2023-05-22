using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;



public class FileUpload : ViewComponent
{

    IHttpContextAccessor _httpContextAccessor;

    public FileUpload(
    IHttpContextAccessor httpContextAccessor
        )
    {
        this._httpContextAccessor = httpContextAccessor;
    }


    public IViewComponentResult Invoke(PartnerDocument _PartnerDocument)
    {
        return View("FileUpload", _PartnerDocument);
    }




}
