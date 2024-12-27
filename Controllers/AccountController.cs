using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BarberShop.ViewModels;
using BarberShop.Models;

namespace BarberShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Kullanici> signInManager;
        private readonly UserManager<Kullanici> userManager;

        public AccountController(SignInManager<Kullanici> signInManager, UserManager<Kullanici> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Register model)
        {
            if (ModelState.IsValid)
            {
                Kullanici user = new Kullanici
                {
                    FullName = model.Name,
                    Email = model.Email,
                    UserName = model.Email,
                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Kullanici");

                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }

        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl; // Pass the return URL to the view
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        var role = (await userManager.GetRolesAsync(user)).FirstOrDefault();

                        HttpContext.Session.SetString("UserName", user.FullName);
                        HttpContext.Session.SetString("UserEmail", user.Email);
                        HttpContext.Session.SetString("UserRole", role);

                        if (role == "Admin")
                            return RedirectToAction("Index", "Admin");
                        if (role == "Calisan")
                            return RedirectToAction("Index", "Calisan");
                        if (role == "Kullanici")
                            return Redirect(returnUrl ?? Url.Action("Index", "Home"));

                        ModelState.AddModelError("", "Geçersiz rol.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email veya şifre yanlış!");
                }
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }


        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmail model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("", "bir hata oluştu!");
                }
                else
                {
                    return RedirectToAction("ChangePassword", "Account", new { username = user.UserName });
                }
            }

            return View(model);
        }

        public IActionResult ChangePassword(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("VerifyEmail", "Account");
            }

            return View(new ChangePassword { Email = username });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePassword model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    var result = await userManager.RemovePasswordAsync(user);
                    if (result.Succeeded)
                    {
                        result = await userManager.AddPasswordAsync(user, model.NewPassword);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("Login", "Account");
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to remove the password.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email not found!");
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}