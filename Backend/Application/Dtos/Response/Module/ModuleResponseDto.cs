namespace Application.Dtos.Response.Module
{
    public class ModuleResponseDto
    {
        public int IdModule { get; set; }
        public string? ModuleName { get; set; }
        public string? AuditCreateDate { get; set; }
        public string? StatusModule { get; set; }
    }
}
