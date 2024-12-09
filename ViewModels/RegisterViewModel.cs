using System.ComponentModel.DataAnnotations;

namespace BarberShop.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad gereklidir.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "E-posta adresi gereklidir.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre gereklidir.")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "{0} en az {2} ve en fazla {1} karakter uzunluğunda olmalıdır.")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifre onayı gereklidir.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifreyi Onayla")]
        public string ConfirmPassword { get; set; }
    }
}
