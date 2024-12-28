using System.ComponentModel.DataAnnotations;

namespace BarberShop.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Email gereklidir.")]
        [EmailAddress]
        public string Email { get; set; }
        [Display (Name = "Şifre")]
        [Required(ErrorMessage = "Şifre gereklidir.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Beni Hatırla?")]
        public bool RememberMe { get; set; }
    }
}
