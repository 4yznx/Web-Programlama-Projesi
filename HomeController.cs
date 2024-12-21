using Microsoft.AspNetCore.Mvc;

namespace BarberShop.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}