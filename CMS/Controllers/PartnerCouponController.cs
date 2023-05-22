using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CMS.Controllers
{
    public class PartnerCouponController : Controller
    {
        ISendMail _ISendMail;
        IHostingEnvironment _IHostingEnvironment;
        IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;

        public PartnerCouponController(

            ISendMail _ISendMail,
            IHostingEnvironment _IHostingEnvironment,
            IHttpContextAccessor _IHttpContextAccessor,
            IHttpClientWrapper _client
            )
        {
            this._ISendMail = _ISendMail;
            this._IHostingEnvironment = _IHostingEnvironment;
            this._IHttpContextAccessor = _IHttpContextAccessor;
            this._client = _client;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> setData(int id1, int id2, string type)
        {
            var result = await _client.GetAsync<PartnerCoupon>(new PartnerCoupon().GetType().Name + $"/setData?id1={id1}&id2={id2}&type={type}");
            return Json(result);
        }

        public async Task<IActionResult> getData(int id1)
        {
            var result = await _client.GetAsync<EnumModel>(new PartnerCoupon().GetType().Name + $"/getData?id1={id1}");
            return Json(result);
        }





    }
}
