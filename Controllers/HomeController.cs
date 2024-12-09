using Microsoft.AspNetCore.Mvc;
using BarberShop.Data;
using Microsoft.EntityFrameworkCore;

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
                .Include(c => c.Islemler)
                .ToList();

            ViewBag.CalismaSaatleri = new Dictionary<string, string>
            {
                {"Pazartesi", "10:00 - 21:00"},
                {"Salı", "10:00 - 21:00"},
                {"Çarşamba", "10:00 - 21:00"},
                {"Perşembe", "10:00 - 21:00"},
                {"Cuma", "10:00 - 21:00"},
                {"Cumartesi", "Kapalı"},
                {"Pazar", "Kapalı"}

            };

            return View(calisanlar);
        }
    }
}
