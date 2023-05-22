using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CMS.Controllers
{
    public class CityController : Controller
    {
        ISendMail _ISendMail;
        IHostingEnvironment _IHostingEnvironment;
        IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;

        public CityController(

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



        public async Task<IActionResult> GetSelect(int CountryId)
        {
            var result = await _client.GetAsync<EnumModel>(new City().GetType().Name + $"/GetSelect?CountryId={CountryId}");
            return Json(result.ResultList);
        }


        [HttpPost]
        public async Task<IActionResult> GetPaging(DTParameters<City> param)
        {
            var result = await _client.PostAsync<City>(new City().GetType().Name + "/GetPaging", param);
            var rs = result.ResultPaging;
            return Json(rs);
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrUpdate(City postmodel)
        {
            var result = await _client.PostAsync<City>(new City().GetType().Name + "/InsertOrUpdate", postmodel);
            return Json(result);
        }

        public async Task<IActionResult> InsertOrUpdatePage()
        {
            var result = await _client.GetAsync<City>(new City().GetType().Name + $"/GetRow?id={Request.Query["id"].ToInt()}");
            ViewBag.postModel = result.ResultRow;
            return View();
        }

        public async Task<IActionResult> GetRow(int? id)
        {
            var result = await _client.GetAsync<City>(new City().GetType().Name + $"/GetRow?id={id}");
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _client.GetAsync<City>(new City().GetType().Name + $"/Delete?id={id}");
            return Json(result);
        }




    }
}
