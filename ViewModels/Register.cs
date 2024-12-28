using System.ComponentModel.DataAnnotations;

namespace BarberShop.ViewModels
{
    public class Register
    {
        [Display (Name = "İsim")]
        [Required(ErrorMessage = "Ad gereklidir.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email gereklidir.")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Şifre")]
        [Required(ErrorMessage = "Şifre gereklidir.")]
        [StringLength(40, MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string Password { get; set; }

        [Display(Name = "Şifre Onayla")]
        [Required(ErrorMessage = "Şifre onayı gereklidir.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
