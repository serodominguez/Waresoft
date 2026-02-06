namespace Application.Dtos.Response.Store
{
    public class StoreResponseDto
    {
        public int IdStore { get; set; }
        public string? StoreName { get; set; }
        public string? Manager { get; set; }
        public string? Address { get; set; }
        public int? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? Email { get; set; }
        public string? Type { get; set; }
        public string? AuditCreateDate { get; set; }
        public bool Status { get; set; }
        public string? StatusStore { get; set; }
    }
}
