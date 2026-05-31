namespace E_Commerce_Api.Date
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }   // Identity uses string
        public ApplicationUser User { get; set; }
        public List<CartItem> CartItems { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}