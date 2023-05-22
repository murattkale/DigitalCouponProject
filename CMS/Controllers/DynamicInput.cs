using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Components
{

    public class DynamicInput : ViewComponent
    {

        IHttpContextAccessor _httpContextAccessor;
        IHttpClientWrapper _client;

        public DynamicInput(
        IHttpContextAccessor httpContextAccessor,
          IHttpClientWrapper _client
            )
        {
            this._httpContextAccessor = httpContextAccessor;
            this._client = _client;
        }


        public IViewComponentResult Invoke(object postModel)
        {
            //if (postModel.PageType == "ContentPage")
            //{
            //    return View("DynamicInput", postModel);
            //}
            //else if (postModel.PageType == "DynamicInput2")
            //{
            //    return View("DynamicInput2", postModel);
            //}
            //else
            //{
            //}
            return View("DynamicInput", postModel);
        }




    }
}