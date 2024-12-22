using Microsoft.AspNetCore.Identity;
using BarberShop.Models;

namespace BarberShop.Data
{
    public static class KullaniciRolu
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Kullanici>>();

            string[] roleNames = { "Admin", "Calisan", "Kullanici" };
            IdentityResult roleResult;

            foreach (var role in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "b211210572@sakarya.edu.tr";
            var adminPassword = "sau";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new Kullanici
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    FullName = "Yazan Suleman"
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}