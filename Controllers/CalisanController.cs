using Microsoft.AspNetCore.Mvc;
using BarberShop.Data;
using BarberShop.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BarberShop.Controllers
{
    public class CalisanController : Controller
    {
        private readonly BarberDbContext _context;

        public CalisanController(BarberDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var calisanlar = _context.Calisanlar
                .Include(c => c.Islemler)
                .ToList();
            return View(calisanlar);
        }
        public IActionResult CalisanEkle()
        {
            ViewBag.Islemler = _context.Islemler.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult CalisanEkle(Calisan calisan, List<int> SelectedIslemIds)
        {
            if (ModelState.IsValid)
            {
                calisan.Islemler = new List<Islem>();

                foreach (var id in SelectedIslemIds)
                {
                    var islem = _context.Islemler.Find(id);
                    if (islem != null)
                    {
                        calisan.Islemler.Add(islem);
                    }
                }

                _context.Calisanlar.Add(calisan);
                _context.SaveChanges();
                TempData["msj"] = $"{calisan.Adi} {calisan.Soyadi} adlı çalışan başarıyla eklendi.";
                return RedirectToAction("Index");
            }

            ViewBag.Islemler = _context.Islemler.ToList();
            return View(calisan);
        }
        public IActionResult CalisanDuzenle(int id)
        {
            var calisan = _context.Calisanlar
                .Include(c => c.Islemler)
                .FirstOrDefault(c => c.CalisanId == id);

            if (calisan == null)
            {
                return NotFound();
            }

            ViewBag.Islemler = _context.Islemler.ToList();
            return View(calisan);
        }

        [HttpPost]
        public IActionResult CalisanDuzenle(Calisan calisan, List<int> SelectedIslemIds)
        {
            if (ModelState.IsValid)
            {
                var existingCalisan = _context.Calisanlar
                    .Include(c => c.Islemler)
                    .FirstOrDefault(c => c.CalisanId == calisan.CalisanId);

                if (existingCalisan == null)
                {
                    return NotFound();
                }

                existingCalisan.Adi = calisan.Adi;
                existingCalisan.Soyadi = calisan.Soyadi;

                // Update Islemler
                existingCalisan.Islemler.Clear();
                foreach (var id in SelectedIslemIds)
                {
                    var islem = _context.Islemler.Find(id);
                    if (islem != null)
                    {
                        existingCalisan.Islemler.Add(islem);
                    }
                }

                _context.SaveChanges();
                TempData["msj"] = $"{existingCalisan.Adi} {existingCalisan.Soyadi} başarıyla güncellendi.";
                return RedirectToAction("Index");
            }

            ViewBag.Islemler = _context.Islemler.ToList();
            return View(calisan);
        }

        public IActionResult CalisanSil(int id)
        {
            var calisan = _context.Calisanlar
                .Include(c => c.Islemler)
                .FirstOrDefault(c => c.CalisanId == id);

            if (calisan == null)
            {
                return NotFound();
            }

            return View(calisan);
        }

        [HttpPost, ActionName("CalisanSil")]
        public IActionResult CalisanSilConfirmed(int id)
        {
            var calisan = _context.Calisanlar
                .Include(c => c.Islemler)
                .FirstOrDefault(c => c.CalisanId == id);

            if (calisan != null)
            {
                _context.Calisanlar.Remove(calisan);
                _context.SaveChanges();
                TempData["msj"] = $"{calisan.Adi} {calisan.Soyadi} başarıyla silindi.";
            }

            return RedirectToAction("Index");
        }
    }
}
