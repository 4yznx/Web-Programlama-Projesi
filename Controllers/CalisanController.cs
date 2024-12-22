using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberShop.Controllers
{
    [Authorize(Roles = "Calisan")]
    public class CalisanController : Controller
    {
        private readonly BarberDbContext _context;

        public CalisanController(BarberDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewData["Layout"] = "_LayoutCalisan.cshtml";
            return View();
        }
    }
}
