namespace Application.Dtos.Response.InventoryPeriod
{
    public record InventoryPeriodDetailResponseDto
    {
        public int IdPeriod { get; init; }
        public int IdStore { get; init; }
        public string? StoreName { get; init; }
        public string? PeriodName { get; init; }
        public string? StartDate { get; init; }
        public string? EndDate { get; init; }
        public string? Status { get; init; }
        public int OpenedByUser { get; init; }
        public string? OpenedDate { get; init; }
        public int? ClosedByUser { get; init; }
        public string? ClosedDate { get; init; }
        public int TotalProducts { get; init; }
        public int TotalSystemStock { get; init; }
        public int TotalPhysicalStock { get; init; }
        public int TotalDifference { get; init; }
    }
}
