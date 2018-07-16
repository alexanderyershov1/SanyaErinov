using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreChatRoom.Controllers
{
    public class HomeController : Controller
    {
        //[HttpGet]
        //public IActionResult Index()
        //{
        //    return View("InsertUserName");
        //}

        [HttpGet]
        public IActionResult Index(string stationID)
        {
            return View("Index", stationID);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
