using System.ComponentModel.DataAnnotations;

namespace BarberShop.ViewModels
{
    public class VerifyEmail
    {
        [Required(ErrorMessage = "Email gereklidir.")]
        [EmailAddress]
        public string Email { get; set; }
    }
}