using System.ComponentModel.DataAnnotations;

namespace BarberShop.ViewModels
{
    public class VerifyEmail
    {
        [Required(ErrorMessage = "E-posta gereklidir.")]
        [EmailAddress]
        public string Email { get; set; }
    }
}