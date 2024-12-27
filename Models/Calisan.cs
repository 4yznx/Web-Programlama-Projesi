using System.ComponentModel.DataAnnotations;

namespace BarberShop.Models
{
    public class Calisan
    {
        public int CalisanID { get; set; }
        public string? UserID { get; set; }
        public string FullName { get; set; }
        public TimeSpan CalismaBaslangici { get; set; }
        public TimeSpan CalismaBitisi { get; set; }
        public ICollection<CalisanIslem> CalisanIslemler { get; set; } = new List<CalisanIslem>();
        public ICollection<Randevu> Randevular { get; set; } = new List<Randevu>();

        public List<DateTime> GetAvailableTimeSlots(Calisan calisan, DateTime date, List<Randevu> appointments, int islemDuration)
        {
            var availableSlots = new List<DateTime>();
            var startTime = date.Date.Add(calisan.CalismaBaslangici);
            var endTime = date.Date.Add(calisan.CalismaBitisi);

            while (startTime.AddMinutes(islemDuration) <= endTime)
            {
                bool isAvailable = !appointments.Any(a =>
                    startTime < a.RandevuSaati.AddMinutes(a.Islem.Sure) &&
                    startTime.AddMinutes(islemDuration) > a.RandevuSaati);

                if (isAvailable)
                {
                    availableSlots.Add(startTime);
                }

                startTime = startTime.AddMinutes(islemDuration);
            }

            return availableSlots;
        }
    }
}