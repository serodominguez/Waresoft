namespace Domain.Entities
{
    public class CustomerEntity : BaseAuditEntity
    {
        public string? Names { get; set; }
        public string? LastNames { get; set; }
        public string? IdentificationNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public bool Status { get; set; }
    }
}
