using Microsoft.AspNetCore.Identity;

namespace BarberShop.Models
{
    public class Kullanici : IdentityUser
    {
        public string FullName { get; set; }

    }
}