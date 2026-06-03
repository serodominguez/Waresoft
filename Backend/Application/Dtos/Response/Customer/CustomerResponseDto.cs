namespace Application.Dtos.Response.Customer
{
    public record CustomerResponseDto
    {
        public int IdCustomer { get; init; }
        public string? Names { get; init; }
        public string? LastNames { get; init; }
        public string? IdentificationNumber { get; init; }
        public string? PhoneNumber { get; init; }
        public string? AuditCreateDate { get; init; }
        public string? StatusCustomer { get; init; }
    }
}
