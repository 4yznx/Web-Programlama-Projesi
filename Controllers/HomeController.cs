using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace BarberShop.Controllers
{
    public class HomeController : Controller
    {

        private readonly BarberDbContext _context;

        public HomeController(BarberDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var calisanlar = _context.Calisanlar
                .Include(c => c.CalisanIslemler)
                .ThenInclude(ci => ci.Islem)
                .ToList();

            return View(calisanlar);
        }
    }
}