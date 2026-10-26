using Microsoft.AspNetCore.Mvc;

namespace CinemaSystem.Areas.Start.Controllers
{
    [Area("Start")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
