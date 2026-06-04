using E_Commerce.Date;

namespace ECommerce.DTO
{
    public class CreateOrderItemDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        public int OrderId { get; set; }

        public int Quantity { get; set; }
    }
}
