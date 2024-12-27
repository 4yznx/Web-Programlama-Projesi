using System.ComponentModel.DataAnnotations;

namespace BarberShop.Models
{
    public class Randevu
    {
        public int RandevuID { get; set; }

        [Required(ErrorMessage = "Randevu tarihi zorunludur.")]
        public DateTime RandevuSaati { get; set; }
        public string Durum { get; set; } = "Beklemede";

        [Required(ErrorMessage = "Email zorunludur.")]
        public string Email { get; set; }
        public int CalisanID { get; set; }
        public Calisan Calisan { get; set; }
        public int IslemID { get; set; }
        public Islem Islem { get; set; }
        public string UserID { get; set; }
        public Kullanici Kullanici { get; set; }
    }
}