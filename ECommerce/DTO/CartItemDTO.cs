using E_Commerce.Date;

namespace ECommerce.DTO
{
    public class CartItemDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CartId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
         public DateTime Date { get; set; } = DateTime.Now;
    }
}
