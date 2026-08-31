namespace Application.Dtos.Response.InventoryPeriod
{
    public class InventoryPeriodResponseDto
    {
        public int IdPeriod { get; init; }
        public int IdStore { get; init; }
        public string? StoreName { get; init; }
        public string? PeriodName { get; init; }
        public string? StartDate { get; init; }
        public string? EndDate { get; init; }
        public string? StatusPeriod { get; init; }
        public int OpenedByUser { get; init; }
        public string? OpenedDate { get; init; }
        public int? ClosedByUser { get; init; }
        public string? ClosedDate { get; init; }
    }
}
