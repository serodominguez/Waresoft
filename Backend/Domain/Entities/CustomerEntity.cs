namespace Domain.Entities
{
    public class CustomerEntity : BaseEntity
    {
        public string? Names { get; set; }
        public string? LastNames { get; set; }
        public string? IdentificationNumber { get; set; }
        public int? PhoneNumber { get; set; }
    }
}
