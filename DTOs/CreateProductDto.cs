namespace FoodMarket.DTOs;

public class CreateProductDto
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; }
    public string Category { get; set; } = "";
    public string? NameEn { get; set; }
    public string? NameAz { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DescriptionAz { get; set; }
    public IFormFile? Image { get; set; }
}