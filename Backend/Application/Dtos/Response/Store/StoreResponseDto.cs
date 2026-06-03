namespace Application.Dtos.Response.Store
{
    public record StoreResponseDto
    {
        public int IdStore { get; init; }
        public string? StoreName { get; init; }
        public string? Manager { get; init; }
        public string? Address { get; init; }
        public string? PhoneNumber { get; init; }
        public string? City { get; init; }
        public string? Email { get; init; }
        public decimal? ProfitMargin { get; init; }
        public string? Type { get; init; }
        public string? AuditCreateDate { get; init; }
        public string? StatusStore { get; init; }
    }
}
