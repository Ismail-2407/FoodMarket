namespace FoodMarket.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public List<Product> Products { get; set; } = new();
    }
}