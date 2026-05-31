namespace E_Commerce_Api.Date
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
        public List<CartItem> CartItems { get; set; } = new();
        public List<OrderItem> OrderItems { get; set; }
        public Category Category { get; set; }
    }
}