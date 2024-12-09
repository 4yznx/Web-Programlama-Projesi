using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberShop.Models
{
    public class Randevu
    {
        public int RandevuID { get; set; }

        public int IslemId { get; set; }
        public int CalisanId { get; set; }

        [Required(ErrorMessage = "Kullanıcı emaili zorunludur.")]
        public string KullaniciEmail { get; set; }

        [Required(ErrorMessage = "Randevu saati zorunludur.")]
        public DateTime RandevuSaati { get; set; }

        public bool IsApproved { get; set; } = false; // Default is false (pending approval)

        [ForeignKey("CalisanId")]
        public Calisan Calisan { get; set; }

        [ForeignKey("IslemId")]
        public Islem Islem { get; set; }

        public string Status { get; set; }
    }
}
