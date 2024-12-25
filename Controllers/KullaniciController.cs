using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BarberShop.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BarberShop.Controllers
{
    [Authorize]
    public class KullaniciController : Controller
    {
        private readonly BarberDbContext _context;

        public KullaniciController(BarberDbContext context)
        {
            _context = context;
        }

        // Display all appointments
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

        // Load the appointment creation form
        public IActionResult RandevuAl()
        {
            ViewData["CalisanID"] = new SelectList(_context.Calisanlar, "CalisanID", "FullName");
            ViewData["IslemID"] = new SelectList(_context.Islemler, "IslemID", "Adi");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RandevuAl(Randevu randevu)
        {
            var calisan = await _context.Calisanlar.FindAsync(randevu.CalisanID);
            var islem = await _context.Islemler.FindAsync(randevu.IslemID);

            if (calisan == null || islem == null)
                return BadRequest("Invalid Çalışan or İşlem selected.");

            var appointments = await _context.Randevular
                .Where(r => r.CalisanID == randevu.CalisanID && r.RandevuSaati.Date == randevu.RandevuSaati.Date)
                .ToListAsync();

            var availableSlots = calisan.GetAvailableTimeSlots(calisan, randevu.RandevuSaati.Date, appointments, islem.Sure);

            if (!availableSlots.Contains(randevu.RandevuSaati))
            {
                ModelState.AddModelError("RandevuSaati", "The selected time slot is no longer available.");
                return View(randevu);
            }

            randevu.UserID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _context.Add(randevu);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Randevu başarıyla alındı!";
            return RedirectToAction(nameof(Index));
        }


        // Display confirmation page for deleting an appointment
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

        // Process the deletion of an appointment
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RandevuSilConfirmed(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);

            if (randevu != null)
            {
                _context.Randevular.Remove(randevu);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
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
                .Where(r => r.CalisanID == calisanId && r.RandevuSaati.Date == date.Date)
                .ToListAsync();

            var availableSlots = calisan.GetAvailableTimeSlots(calisan, date, appointments, islem.Sure);

            return Json(availableSlots); // Return the list of available time slots as JSON
        }

    }
}
