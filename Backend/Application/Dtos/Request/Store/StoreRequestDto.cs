namespace Application.Dtos.Request.Store
{
    public class StoreRequestDto
    {
        public string? StoreName { get; set; }
        public string? Manager { get; set; }
        public string? Address { get; set; }
        public int? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? Email { get; set; }
        public string? Type { get; set; }
    }
}
