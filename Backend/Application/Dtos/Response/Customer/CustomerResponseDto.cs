namespace Application.Dtos.Response.Customer
{
    public class CustomerResponseDto
    {
        public int IdCustomer { get; set; }
        public string? Names { get; set; }
        public string? LastNames { get; set; }
        public string? IdentificationNumber { get; set; }
        public int? PhoneNumber { get; set; }
        public string? AuditCreateDate { get; set; }
        public bool Status { get; set; }
        public string? StatusCustomer { get; set; }
    }
}
