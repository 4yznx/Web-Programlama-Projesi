using System.ComponentModel.DataAnnotations;

namespace BarberShop.ViewModels
{
    public class ChangePassword
    {
        [Required(ErrorMessage = "E-posta gereklidir.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre gereklidir.")]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "{0} en az {2} ve en fazla {1} karakter uzunluğunda olmalıdır.")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        [Compare("ConfirmNewPassword", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string NewPassword { get; set; }


        [Required(ErrorMessage = "Şifre onayı gereklidir.")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifreyi Onayla")]
        public string ConfirmNewPassword { get; set; }
    }
}
