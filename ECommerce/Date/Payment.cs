namespace E_Commerce.Date
{
    public class Payment
    {
        public int Id { get; set; }

        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }


    }
}