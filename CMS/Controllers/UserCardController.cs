using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CMS.Controllers
{
    public class UserCardController : Controller
    {
        ISendMail _ISendMail;
        IHostingEnvironment _IHostingEnvironment;
        IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;

        public UserCardController(

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


        public async Task<IActionResult> GetSelect(string name, string whereCase)
        {
            var result = await _client.GetAsync<EnumModel>(new UserCard().GetType().Name + $"/GetSelect?name={name}&whereCase={whereCase}");
            return Json(result.ResultList);
        }


        [HttpPost]
        public async Task<IActionResult> GetPaging(DTParameters<UserCard> param)
        {
            var result = await _client.PostAsync<UserCard>(new UserCard().GetType().Name + "/GetPaging", param);
            var rs = result.ResultPaging;
            return Json(rs);
        }


        [HttpPost]
        public async Task<IActionResult> GetPagingNotUser(DTParameters<UserCard> param)
        {
            var result = await _client.PostAsync<UserCard>(new UserCard().GetType().Name + "/GetPagingNotUser", param);
            var rs = result.ResultPaging;
            return Json(rs);
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrUpdate(UserCard postmodel)
        {
            var result = await _client.PostAsync<UserCard>(new UserCard().GetType().Name + "/InsertOrUpdate", postmodel);
            return Json(result);
        }

        public async Task<IActionResult> InsertOrUpdatePage()
        {
            var result = await _client.GetAsync<UserCard>(new UserCard().GetType().Name + $"/GetRow?id={Request.Query["id"].ToInt()}");
            ViewBag.postModel = result.ResultRow;
            return View();
        }

        public async Task<IActionResult> GetRow(int? id)
        {
            var result = await _client.GetAsync<UserCard>(new UserCard().GetType().Name + $"/GetRow?id={id}");
            return Json(result);
        }

        public async Task<IActionResult> BulkInsert(string MemberShips, int count)
        {
            var result = await _client.GetAsync<UserCard>(new UserCard().GetType().Name + $"/BulkInsert?MemberShips={MemberShips}&count={count}");
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _client.GetAsync<UserCard>(new UserCard().GetType().Name + $"/Delete?id={id}");
            return Json(result);
        }




    }
}
