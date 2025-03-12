using Microsoft.AspNetCore.Mvc;

namespace Coolicky.TrailerPacking.DemoApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}