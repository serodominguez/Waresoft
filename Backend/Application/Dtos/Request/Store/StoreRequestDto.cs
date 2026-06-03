namespace Application.Dtos.Request.Store
{
    public record StoreRequestDto
    {
        public string? StoreName { get; init; }
        public string? Manager { get; init; }
        public string? Address { get; init; }
        public string? PhoneNumber { get; init; }
        public string? City { get; init; }
        public string? Email { get; init; }
        public decimal ProfitMargin { get; init; }
        public string? Type { get; init; }
    }
}
