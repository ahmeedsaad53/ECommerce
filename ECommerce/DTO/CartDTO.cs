using E_Commerce.Date;

namespace ECommerce.DTO
{
    public class CartDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }   // Identity uses string
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
