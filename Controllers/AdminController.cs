﻿using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> CalisanEkle(Calisan calisan, string EmailPrefix, List<int> SecilenIslemIds)
        {
            if (calisan.CalismaBaslangici >= calisan.CalismaBitisi)
            {
                ModelState.AddModelError("", "Başlangıç saati bitiş saatinden önce olmalıdır.");
            }

            if (ModelState.IsValid)
            {
                var email = $"{EmailPrefix.Trim().ToLower()}@barbershop.com";

                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Oluşturulan email adresi kullanıliyor.");
                    ViewBag.Islemler = _context.Islemler.ToList();
                    return View(calisan);
                }

                _context.Calisanlar.Add(calisan);
                await _context.SaveChangesAsync();

                var user = new Kullanici
                {
                    FullName = calisan.FullName,
                    Email = email,
                    UserName = email
                };

                var result = await _userManager.CreateAsync(user, "cal");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Calisan");

                    calisan.UserID = user.Id;

                    foreach (var islemId in SecilenIslemIds)
                    {
                        _context.CalisanIslemler.Add(new CalisanIslem
                        {
                            CalisanID = calisan.CalisanID,
                            IslemID = islemId
                        });
                    }

                    _context.Update(calisan);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("CalisanListesi");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
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

        public async Task<IActionResult> CalisanKazanclari()
        {
            var calisanKazanc = await _context.Randevular
                .Where(r => r.Durum == "Kabul Edildi")
                .GroupBy(r => new { r.CalisanID, Tarih = r.RandevuSaati.Date })
                .Select(g => new CalisanKazanclari
                {
                    Tarih = g.Key.Tarih,
                    CalisanID = g.Key.CalisanID,
                    ToplamKazanc = g.Sum(r => r.Islem.Ucret),
                    Calisan = g.Select(r => r.Calisan).FirstOrDefault()
                })
                .ToListAsync();

            return View(calisanKazanc);
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

        public async Task<IActionResult> ManageUsers()
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

        public async Task<IActionResult> EditUserRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = _roleManager.Roles.ToList();
            var userRoles = await _userManager.GetRolesAsync(user);

            ViewBag.User = user;
            ViewBag.Roles = roles;
            ViewBag.UserRoles = userRoles;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditUserRole(string userId, string selectedRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (string.IsNullOrEmpty(selectedRole))
            {
                TempData["msj"] = "Lütfen bir rol seçin.";
                return RedirectToAction("EditUserRole", new { id = userId });
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, selectedRole);

            TempData["msj"] = "Kullanıcının rolü başarıyla güncellendi.";
            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("ManageUsers");
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "Kullanıcı başarıyla silindi.";
            }
            else
            {
                TempData["Error"] = "Kullanıcı silinirken bir hata oluştu.";
            }

            return RedirectToAction("ManageUsers");
        }
        public async Task<IActionResult> DeleteUserConfirm(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("ManageUsers");
            }

            return View(user);
        }
    }
}