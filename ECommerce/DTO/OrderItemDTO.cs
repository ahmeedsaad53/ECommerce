using E_Commerce.Date;

namespace ECommerce.DTO
{
    public class OrderItemDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
