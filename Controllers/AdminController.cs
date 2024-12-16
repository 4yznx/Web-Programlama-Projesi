using BarberShop.Data;
using BarberShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberShop.Controllers
{
    public class AdminController : Controller
    {
        private readonly BarberDbContext _context;

        public AdminController(BarberDbContext context)
        {
            _context = context;
        }

        // --- INDEX ---
        public IActionResult Index()
        {
            ViewData["Layout"] = "_LayoutAdmin.cshtml";
            return View();
        }

        // --- CALISAN LISTESI ---
        public IActionResult CalisanListesi()
        {
            var calisanlar = _context.Calisanlar
                .Include(c => c.CalisanIslemler)
                .ThenInclude(ci => ci.Islem)
                .ToList();
            return View(calisanlar);
        }

        // --- CALISAN EKLE ---
        public IActionResult CalisanEkle()
        {
            ViewBag.Islemler = _context.Islemler.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult CalisanEkle(Calisan calisan, List<int> SecilenIslemIds)
        {
            if (ModelState.IsValid)
            {
                // Add Calisan
                _context.Calisanlar.Add(calisan);
                _context.SaveChanges();

                // Add selected Islem entries in CalisanIslem
                foreach (var islemId in SecilenIslemIds)
                {
                    var calisanIslem = new CalisanIslem
                    {
                        CalisanId = calisan.CalisanID,
                        IslemID = islemId
                    };
                    _context.Add(calisanIslem);
                }

                _context.SaveChanges();
                TempData["msj"] = $"{calisan.Adi} {calisan.Soyadi} başarıyla eklendi.";
                return RedirectToAction("CalisanListesi");
            }

            ViewBag.Islemler = _context.Islemler.ToList();
            return View(calisan);
        }

        // --- CALISAN DUZENLE ---
        public IActionResult CalisanDuzenle(int id)
        {
            var calisan = _context.Calisanlar
                .Include(c => c.CalisanIslemler)
                .FirstOrDefault(c => c.CalisanID == id);

            if (calisan == null)
                return NotFound();

            ViewBag.Islemler = _context.Islemler.ToList();
            ViewBag.SelectedIslemler = calisan.CalisanIslemler.Select(ci => ci.IslemID).ToList();

            return View(calisan);
        }

        [HttpPost]
        public IActionResult CalisanDuzenle(Calisan calisan, List<int> SelectedIslemIds)
        {
            if (ModelState.IsValid)
            {
                var existingCalisan = _context.Calisanlar
                    .Include(c => c.CalisanIslemler)
                    .FirstOrDefault(c => c.CalisanID == calisan.CalisanID);

                if (existingCalisan == null)
                    return NotFound();

                // Update basic details
                existingCalisan.Adi = calisan.Adi;
                existingCalisan.Soyadi = calisan.Soyadi;

                // Update CalisanIslem entries
                existingCalisan.CalisanIslemler.Clear();
                foreach (var islemId in SelectedIslemIds)
                {
                    existingCalisan.CalisanIslemler.Add(new CalisanIslem
                    {
                        CalisanId = calisan.CalisanID,
                        IslemID = islemId
                    });
                }

                _context.SaveChanges();
                TempData["msj"] = $"{existingCalisan.Adi} {existingCalisan.Soyadi} başarıyla güncellendi.";
                return RedirectToAction("CalisanListesi");
            }

            ViewBag.Islemler = _context.Islemler.ToList();
            return View(calisan);
        }

        // --- CALISAN SIL ---
        public IActionResult CalisanSil(int id)
        {
            var calisan = _context.Calisanlar
                .Include(c => c.CalisanIslemler)
                .ThenInclude(ci => ci.Islem)
                .FirstOrDefault(c => c.CalisanID == id);

            if (calisan == null)
                return NotFound();

            return View(calisan);
        }

        [HttpPost, ActionName("CalisanSil")]
        public IActionResult CalisanSilConfirmed(int id)
        {
            var calisan = _context.Calisanlar
                .Include(c => c.CalisanIslemler)
                .FirstOrDefault(c => c.CalisanID == id);

            if (calisan != null)
            {
                _context.Calisanlar.Remove(calisan);
                _context.SaveChanges();
                TempData["msj"] = $"{calisan.Adi} {calisan.Soyadi} başarıyla silindi.";
            }

            return RedirectToAction("CalisanListesi");
        }

        // --- ISLEM LISTESI ---
        public IActionResult IslemListesi()
        {
            var islemler = _context.Islemler.ToList();
            return View(islemler);
        }

        // --- ISLEM EKLE ---
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
                return RedirectToAction("Index");
            }
            return View(islem);
        }

        // --- ISLEM DUZENLE ---
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
                return RedirectToAction("Index");
            }
            return View(islem);
        }

        // --- ISLEM SIL ---
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
            return RedirectToAction("Index");
        }
    }
}
