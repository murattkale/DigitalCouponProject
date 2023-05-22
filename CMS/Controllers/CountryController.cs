using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CMS.Controllers
{
    public class CountryController : Controller
    {
        ISendMail _ISendMail;
        IHostingEnvironment _IHostingEnvironment;
        IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;

        public CountryController(

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



        public async Task<IActionResult> GetSelect()
        {
            var result = await _client.GetAsync<EnumModel>(new Country().GetType().Name + $"/GetSelect");
            return Json(result.ResultList);
        }


        [HttpPost]
        public async Task<IActionResult> GetPaging(DTParameters<Country> param)
        {
            var result = await _client.PostAsync<Country>(new Country().GetType().Name + "/GetPaging", param);
            var rs = result.ResultPaging;
            return Json(rs);
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrUpdate(Country postmodel)
        {
            var result = await _client.PostAsync<Country>(new Country().GetType().Name + "/InsertOrUpdate", postmodel);
            return Json(result);
        }

        public async Task<IActionResult> InsertOrUpdatePage()
        {
            var result = await _client.GetAsync<Country>(new Country().GetType().Name + $"/GetRow?id={Request.Query["id"].ToInt()}");
            ViewBag.postModel = result.ResultRow;
            return View();
        }

        public async Task<IActionResult> GetRow(int? id)
        {
            var result = await _client.GetAsync<Country>(new Country().GetType().Name + $"/GetRow?id={id}");
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _client.GetAsync<Country>(new Country().GetType().Name + $"/Delete?id={id}");
            return Json(result);
        }




    }
}
