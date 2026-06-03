namespace Application.Dtos.Response.Module
{
    public record ModuleResponseDto
    {
        public int IdModule { get; init; }
        public string? ModuleName { get; init; }
        public string? AuditCreateDate { get; init; }
        public string? StatusModule { get; init; }
    }
}
