using System.ComponentModel.DataAnnotations;

namespace BarberShop.ViewModels
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "E-posta adresi gereklidir.")]
        [EmailAddress]
        public string Email { get; set; }
    }
}