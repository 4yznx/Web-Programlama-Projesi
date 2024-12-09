using System.ComponentModel.DataAnnotations;

namespace BarberShop.Models
{
    public class Islem
    {
        public int IslemID { get; set; }

        [Required(ErrorMessage = "İşlem adı zorunludur.")]
        [StringLength(50, ErrorMessage = "İşlem adı 50 karakterden fazla olamaz.")]
        [Display(Name = "İşlem Adı")]
        public string Adi { get; set; }

        [Required(ErrorMessage = "İşlem süresi zorunludur.")]
        [Range(1, 300, ErrorMessage = "İşlem süresi 1 ile 300 dakika arasında olmalıdır.")]
        [Display(Name = "Süre (dakika)")]
        public int Sure { get; set; }

        [Required(ErrorMessage = "Ücret zorunludur.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Ücret")]
        public decimal Ucret { get; set; }
        public ICollection<Calisan> Calisanlar { get; set; } = new List<Calisan>();

    }
}
