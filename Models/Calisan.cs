using System.ComponentModel.DataAnnotations;

namespace BarberShop.Models
{
    public class Calisan
    {
        public int CalisanID { get; set; }

        [Required(ErrorMessage = "Adı alanı zorunludur.")]
        [StringLength(50, ErrorMessage = "Adı 50 karakterden fazla olamaz.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Başlangıç saati zorunludur.")]
        [DataType(DataType.Time)]
        public TimeSpan BaslangicSaati { get; set; }

        [Required(ErrorMessage = "Bitiş saati zorunludur.")]
        [DataType(DataType.Time)]
        public TimeSpan BitisSaati { get; set; }
        public ICollection<CalisanIslem> CalisanIslemler { get; set; } = new List<CalisanIslem>();

		//public ICollection<Randevu> Randevular { get; set; } = new List<Randevu>();

	}
}
