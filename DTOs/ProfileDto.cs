namespace FoodMarket.API.DTOs
{
    public class ProfileDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? PhotoUrl { get; set; }
        public List<AddressDto> Addresses { get; set; } = new();
    }

    public class UpdateProfileDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? PhotoUrl { get; set; }
    }

    public class AddressDto
    {
        public int Id { get; set; }
        public string FullAddress { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }

    public class UpsertAddressDto
    {
        public string FullAddress { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }
}