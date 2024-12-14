using BarberShop.Data;
using BarberShop.Models;
using BarberShop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberShop.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly BarberDbContext _context;
        private readonly UserManager<Kullanici> _userManager;

        public AdminController(BarberDbContext context, UserManager<Kullanici> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            ViewData["Layout"] = "_LayoutAdmin.cshtml";
            return View();
        }

        public IActionResult CalisanListesi()
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
                return RedirectToAction("CalisanListesi");
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
                return RedirectToAction("CalisanListesi");
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

            return RedirectToAction("CalisanListesi");
        }

        public IActionResult IslemListesi()
        {
            var islemler = _context.Islemler.ToList();
            return View(islemler);
        }

        public IActionResult IslemEkle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult IslemEkle(Islem islem)
        {
            if (ModelState.IsValid)
            {
                _context.Islemler.Add(islem);
                _context.SaveChanges();
                TempData["msj"] = $"{islem.Adi} işlemi başarıyla eklendi.";
                return RedirectToAction("IslemListesi");
            }
            return View(islem);
        }

        public IActionResult IslemDuzenle(int id)
        {
            var islem = _context.Islemler.Find(id);
            if (islem == null)
            {
                return NotFound();
            }
            return View(islem);
        }

        [HttpPost]
        public IActionResult IslemDuzenle(Islem islem)
        {
            if (ModelState.IsValid)
            {
                _context.Islemler.Update(islem);
                _context.SaveChanges();
                TempData["msj"] = $"{islem.Adi} işlemi başarıyla güncellendi.";
                return RedirectToAction("IslemListesi");
            }
            return View(islem);
        }

        public IActionResult IslemSil(int id)
        {
            var islem = _context.Islemler.Find(id);
            if (islem == null)
            {
                return NotFound();
            }
            return View(islem);
        }

        [HttpPost, ActionName("IslemSil")]
        public IActionResult IslemSilConfirmed(int id)
        {
            var islem = _context.Islemler.Find(id);
            if (islem != null)
            {
                _context.Islemler.Remove(islem);
                _context.SaveChanges();
                TempData["msj"] = $"{islem.Adi} işlemi başarıyla silindi.";
            }
            return RedirectToAction("IslemListesi");
        }
    }
}
