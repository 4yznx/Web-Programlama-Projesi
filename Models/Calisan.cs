using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberShop.Models
{
    public class Calisan
    {
        public int CalisanId { get; set; }

        [Required(ErrorMessage = "Adı alanı zorunludur.")]
        [StringLength(50, ErrorMessage = "Adı 50 karakterden fazla olamaz.")]
        public string Adi { get; set; }

        [Required(ErrorMessage = "Soyadı alanı zorunludur.")]
        [StringLength(50, ErrorMessage = "Soyadı 50 karakterden fazla olamaz.")]
        public string Soyadi { get; set; }
        public ICollection<Islem> Islemler { get; set; } = new List<Islem>();
    }
}
