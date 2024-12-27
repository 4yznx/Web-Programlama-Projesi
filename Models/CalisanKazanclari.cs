namespace BarberShop.Models
{
    public class CalisanKazanclari
    {
        public int ID { get; set; }
        public DateTime Tarih { get; set; }
        public decimal ToplamKazanc { get; set; }
        public int CalisanID { get; set; }
        public Calisan Calisan { get; set; }
    }
}
