using Microsoft.AspNetCore.Mvc;

namespace BarberShop.Controllers
{
    public class KullaniciController : Controller
    {
        public IActionResult Index ()
        {
            return View();
        } 
    }
}