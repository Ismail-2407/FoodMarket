namespace FoodMarket.Models
{
    public class Address
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string FullAddress { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }
}