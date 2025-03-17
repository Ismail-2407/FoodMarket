namespace FoodMarket.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public List<Product> Products { get; set; } = new();
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Pending"; 
    }
}