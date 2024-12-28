namespace BarberShop.Models
{
    public class HairstyleChangerRequest
    {
        public IFormFile Image { get; set; }

        public int HairType { get; set; }

    }
}