using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using BarberShop.Models;

namespace BarberShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly BarberDbContext _context;
        private readonly UserManager<Kullanici> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(BarberDbContext context, UserManager<Kullanici> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            ViewData["Layout"] = "_LayoutAdmin.cshtml";
            return View();
        }

        public IActionResult CalisanListesi()
        {
            var calisanlar = _context.Calisanlar
                .Include(c => c.CalisanIslemler)
                .ThenInclude(ci => ci.Islem)
                .ToList();
            return View(calisanlar);
        }

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
                _context.Calisanlar.Add(calisan);
                _context.SaveChanges();

                foreach (var islemId in SecilenIslemIds)
                {
                    var calisanIslem = new CalisanIslem
                    {
                        CalisanID = calisan.CalisanID,
                        IslemID = islemId
                    };
                    _context.CalisanIslemler.Add(calisanIslem);
                }

                _context.SaveChanges();
                TempData["msj"] = $"{calisan.FullName} başarıyla eklendi.";
                return RedirectToAction("CalisanListesi");
            }

            ViewBag.Islemler = _context.Islemler.ToList();
            return View(calisan);
        }
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

                existingCalisan.FullName = calisan.FullName;
                existingCalisan.BaslangicSaati = calisan.BaslangicSaati;
                existingCalisan.BitisSaati = calisan.BitisSaati;

                existingCalisan.CalisanIslemler.Clear();
                foreach (var islemId in SelectedIslemIds)
                {
                    existingCalisan.CalisanIslemler.Add(new CalisanIslem
                    {
                        CalisanID = calisan.CalisanID,
                        IslemID = islemId
                    });
                }

                _context.SaveChanges();
                TempData["msj"] = $"{existingCalisan.FullName} başarıyla güncellendi.";
                return RedirectToAction("CalisanListesi");
            }

            ViewBag.Islemler = _context.Islemler.ToList();
            return View(calisan);
        }
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

        [HttpPost]
        public IActionResult CalisanSilConfirmed(int id)
        {
            var calisan = _context.Calisanlar
                .Include(c => c.CalisanIslemler)
                .FirstOrDefault(c => c.CalisanID == id);

            if (calisan != null)
            {
                _context.Calisanlar.Remove(calisan);
                _context.SaveChanges();
                TempData["msj"] = $"{calisan.FullName} başarıyla silindi.";
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

        [HttpPost]
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

        public async Task<IActionResult> KullaniciYonetimi()
        {
            var users = _userManager.Users.ToList();
            var model = new List<RolSecici>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new RolSecici
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return View(model);
        }
        public async Task<IActionResult> RolDegistir(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = _roleManager.Roles.ToList();
            var userRoles = await _userManager.GetRolesAsync(user);

            ViewBag.User = user;
            ViewBag.Roles = roles;
            ViewBag.UserRoles = userRoles;

            return View(user);
        }


        [HttpPost]
        public async Task<IActionResult> RolDegistir(string userId, string selectedRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("KullaniciYonetimi");
            }

            if (string.IsNullOrEmpty(selectedRole))
            {
                TempData["Error"] = "Lütfen bir rol seçin.";
                return RedirectToAction("RolDegistir", new { id = userId });
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var result = await _userManager.AddToRoleAsync(user, selectedRole);
            if (result.Succeeded)
            {
                TempData["Başarılı"] = "Kullanıcının rolü başarıyla güncellendi.";
            }
            else
            {
                TempData["Error"] = "Rol değişikliği sırasında bir hata oluştu.";
            }

            return RedirectToAction("KullaniciYonetimi");
        }

        [HttpPost]
        public async Task<IActionResult> KullaniciSilConfirm(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("KullaniciYonetimi");
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> KullaniciSil(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("KullaniciYonetimi");
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "Kullanıcı başarıyla silindi.";
            }
            else
            {
                TempData["Error"] = "Bir hata oluştu!";
            }

            return RedirectToAction("KullaniciYonetimi");
        }


    }
}