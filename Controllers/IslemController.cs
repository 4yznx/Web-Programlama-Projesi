using Microsoft.AspNetCore.Mvc;
using BarberShop.Data;
using BarberShop.Models;
using Microsoft.AspNetCore.Authorization;

namespace BarberShop.Controllers
{
    public class IslemController : Controller
    {
        private readonly BarberDbContext _context;

        public IslemController(BarberDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
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
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
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
            return RedirectToAction("Index");
        }
    }
}
