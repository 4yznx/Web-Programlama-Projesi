using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberShop.Models
{
    public class Randevu
    {
        public int RandevuID { get; set; }

        [Required(ErrorMessage = "Randevu tarihi zorunludur.")]
        public DateTime RandevuSaati { get; set; }
        public string Durum { get; set; } = "Beklemede";

        // Foreign Key for Calisan
        public int CalisanID { get; set; }
        public Calisan Calisan { get; set; }

        // Foreign Key for Islem
        public int IslemID { get; set; }
        public Islem Islem { get; set; }

        // Foreign Key for User
        public string UserID { get; set; }
        public Kullanici kullanici { get; set; }

        [Required(ErrorMessage = "Email zorunludur.")]
        public string Email { get; set; }
    }

}