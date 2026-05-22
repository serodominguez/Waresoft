namespace Application.Dtos.Response.Customer
{
    public class CustomerResponseDto
    {
        public int IdCustomer { get; set; }
        public string? Names { get; set; }
        public string? LastNames { get; set; }
        public string? IdentificationNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AuditCreateDate { get; set; }
        public string? StatusCustomer { get; set; }
    }
}
