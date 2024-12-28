using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BarberShop.Models;

namespace BarberShop.Controllers
{
    [Authorize(Roles = "Kullanici")]
    public class KullaniciController : Controller
    {
        private readonly BarberDbContext _context;

        public KullaniciController(BarberDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var randevular = await _context.Randevular
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .ToListAsync();

            return View(randevular);
        }
        public async Task<IActionResult> Randevularim()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var appointments = await _context.Randevular
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .Where(r => r.UserID == userId)
                .ToListAsync();

            return View(appointments);
        }

        public IActionResult RandevuAl()
        {
            ViewData["CalisanID"] = new SelectList(_context.Calisanlar, "CalisanID", "FullName");
            ViewData["IslemID"] = new SelectList(_context.Islemler, "IslemID", "Adi");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RandevuAl(Randevu randevu)
        {
            var calisan = await _context.Calisanlar.FindAsync(randevu.CalisanID);
            var islem = await _context.Islemler.FindAsync(randevu.IslemID);

            if (calisan == null || islem == null)
                return BadRequest("Geçersiz Çalışan veya İşlem seçildi.");

            var appointments = await _context.Randevular
                .Include(r => r.Islem)
                .Where(r => r.CalisanID == randevu.CalisanID && r.RandevuSaati.Date == randevu.RandevuSaati.Date)
                .ToListAsync();

            var availableSlots = calisan.GetAvailableTimeSlots(calisan, randevu.RandevuSaati.Date, appointments, islem.Sure);

            if (!availableSlots.Contains(randevu.RandevuSaati))
            {
                ModelState.AddModelError("RandevuSaati", "Seçilen zaman aralığı artık mevcut değil.");
                return View(randevu);
            }

            randevu.UserID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _context.Add(randevu);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Randevu başarıyla alındı!";
            return RedirectToAction("Randevularim");
        }

        [HttpGet]
        public async Task<IActionResult> GetIslemDetails(int islemId)
        {
            var islem = await _context.Islemler.FindAsync(islemId);
            if (islem == null)
                return NotFound();

            return Json(new { ucret = islem.Ucret, sure = islem.Sure });
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableSlots(int calisanId, int islemId, DateTime date)
        {
            var calisan = await _context.Calisanlar.FindAsync(calisanId);
            if (calisan == null)
                return NotFound();

            var islem = await _context.Islemler.FindAsync(islemId);
            if (islem == null)
                return NotFound();

            var appointments = await _context.Randevular
                .Include(r => r.Islem)
                .Where(r => r.CalisanID == calisanId && r.RandevuSaati.Date == date.Date)
                .ToListAsync();

            var availableSlots = calisan.GetAvailableTimeSlots(calisan, date, appointments, islem.Sure);

            if (date.Date == DateTime.Now.Date)
            {
                var currentTime = DateTime.Now;
                availableSlots = availableSlots.Where(slot => slot > currentTime).ToList();
            }

            return Json(availableSlots);
        }
        public async Task<IActionResult> RandevuSil(int? id)
        {
            if (id == null)
                return NotFound();

            var randevu = await _context.Randevular
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .FirstOrDefaultAsync(r => r.RandevuID == id);

            if (randevu == null)
                return NotFound();

            return View(randevu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RandevuSilConfirmed(int id)
        {
            var randevu = await _context.Randevular
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .FirstOrDefaultAsync(r => r.RandevuID == id);

            if (randevu != null)
            {
                var kazancKaydi = await _context.CalisanKazanclari
                    .FirstOrDefaultAsync(k => k.CalisanID == randevu.CalisanID && k.Tarih == randevu.RandevuSaati.Date);

                if (kazancKaydi != null)
                {
                    kazancKaydi.ToplamKazanc -= randevu.Islem.Ucret;

                    if (kazancKaydi.ToplamKazanc <= 0)
                    {
                        _context.CalisanKazanclari.Remove(kazancKaydi);
                    }
                    else
                    {
                        _context.CalisanKazanclari.Update(kazancKaydi);
                    }
                }

                _context.Randevular.Remove(randevu);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Randevu başarıyla iptal edildi.";
            return RedirectToAction("Randevularim");
        }
    }
}