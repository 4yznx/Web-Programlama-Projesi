using System.ComponentModel.DataAnnotations;

namespace BarberShop.Models
{
    public class Calisan
    {
        public int CalisanID { get; set; }

        public string FullName { get; set; }

        // Working hours
        public TimeSpan CalismaBaslangici { get; set; } // Start time (e.g., 09:00)
        public TimeSpan CalismaBitisi { get; set; } // End time (e.g., 17:00)

        // Relationships
        public ICollection<CalisanIslem> CalisanIslemler { get; set; } = new List<CalisanIslem>();
        public ICollection<Randevu> Randevular { get; set; } = new List<Randevu>();





        public List<DateTime> GetAvailableTimeSlots(Calisan calisan, DateTime date, List<Randevu> appointments, int islemDuration)
        {
            var availableSlots = new List<DateTime>();
            var startTime = date.Date.Add(calisan.CalismaBaslangici); // Start time
            var endTime = date.Date.Add(calisan.CalismaBitisi); // End time

            // Iterate through the day in intervals of the Islem duration
            while (startTime.AddMinutes(islemDuration) <= endTime)
            {
                // Check if the slot overlaps with any existing appointments
                bool isAvailable = !appointments.Any(a =>
                    startTime < a.RandevuSaati.AddMinutes(a.Islem.Sure) && // Start overlaps
                    startTime.AddMinutes(islemDuration) > a.RandevuSaati); // End overlaps

                if (isAvailable)
                {
                    availableSlots.Add(startTime);
                }

                startTime = startTime.AddMinutes(islemDuration); // Move to the next slot
            }

            return availableSlots;
        }

    }
}
