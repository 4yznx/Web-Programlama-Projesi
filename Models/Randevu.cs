using System.ComponentModel.DataAnnotations;

namespace BarberShop.Models
{
	public enum RandevuDurumu
	{
		Beklemede,
		Onaylandi,
		Reddedildi
	}
	public class Randevu
	{
		public int RandevuID { get; set; }

		[Required(ErrorMessage = "Randevu tarihi zorunludur.")]
		public DateTime AppointmentDate { get; set; }
		public RandevuDurumu Durum { get; set; } = RandevuDurumu.Beklemede;

		[Required]
		public int CalisanID { get; set; }
		public Calisan Calisan { get; set; }

		[Required]
		public int IslemID { get; set; }
		public Islem Islem { get; set; }

		[Required]
		public string KullaniciID { get; set; }
		public Kullanici Kullanici { get; set; }
	}
}