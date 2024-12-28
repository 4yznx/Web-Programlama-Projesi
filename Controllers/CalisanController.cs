using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BarberShop.Models;

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

        public async Task<IActionResult> Randevular()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var calisan = await _context.Calisanlar
                .FirstOrDefaultAsync(c => c.UserID == userId);

            if (calisan == null)
            {
                return NotFound("Çalışan bulunamadı.");
            }

            var randevular = await _context.Randevular
                .Include(r => r.Kullanici)
                .Include(r => r.Islem)
                .Where(r => r.CalisanID == calisan.CalisanID)
                .ToListAsync();

            return View(randevular);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptRandevu(int id)
        {
            var randevu = await _context.Randevular
                .Include(r => r.Islem)
                .FirstOrDefaultAsync(r => r.RandevuID == id);

            if (randevu == null)
            {
                return NotFound();
            }

            randevu.Durum = "Kabul Edildi";
            await _context.SaveChangesAsync();

            var existingEarnings = await _context.CalisanKazanclari
                .FirstOrDefaultAsync(c => c.CalisanID == randevu.CalisanID && c.Tarih == DateTime.Today);

            if (existingEarnings != null)
            {
                existingEarnings.ToplamKazanc += randevu.Islem.Ucret;
            }
            else
            {
                var newEarnings = new CalisanKazanclari
                {
                    CalisanID = randevu.CalisanID,
                    Tarih = DateTime.Today,
                    ToplamKazanc = randevu.Islem.Ucret
                };
                _context.CalisanKazanclari.Add(newEarnings);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Randevu başarıyla kabul edildi.";
            return RedirectToAction("Randevular");
        }


        [HttpPost]
        public async Task<IActionResult> RejectRandevu(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);

            if (randevu == null)
            {
                return NotFound("Randevu bulunamadı.");
            }

            _context.Randevular.Remove(randevu);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Randevu başarıyla reddedildi.";
            return RedirectToAction("Randevular");
        }
    }
}
