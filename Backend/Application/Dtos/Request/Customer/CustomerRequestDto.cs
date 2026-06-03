namespace Application.Dtos.Request.Customer
{
    public record CustomerRequestDto
    {
        public string? Names { get; init; }
        public string? LastNames { get; init; }
        public string? IdentificationNumber { get; init; }
        public string? PhoneNumber { get; init; }

    }
}
