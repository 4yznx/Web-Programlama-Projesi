using System.ComponentModel.DataAnnotations.Schema;

namespace BarberShop.Models
{
    public class CalisanIslem
    {
        
        public int CalisanID { get; set; }
      
        public Calisan Calisan { get; set; }
        public int IslemID { get; set; }
        public Islem Islem { get; set; }
    }
}