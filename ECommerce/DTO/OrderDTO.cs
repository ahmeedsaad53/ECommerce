using E_Commerce.Date;

namespace ECommerce.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Pending";
    }
}
