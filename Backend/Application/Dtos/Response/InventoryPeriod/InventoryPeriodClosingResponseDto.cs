namespace Application.Dtos.Response.InventoryPeriod
{
    public record InventoryPeriodClosingResponseDto
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
        public int TotalProducts { get; init; }
        public int TotalSystemStock { get; init; }
        public int TotalPhysicalStock { get; init; }
        public int TotalDifference { get; init; }
        public List<InventoryPeriodClosingItemResponseDto> Items { get; init; } = [];
    }

    public record InventoryPeriodClosingItemResponseDto
    {
        public int IdProduct { get; init; }
        public string? ProductCode { get; init; }
        public string? ProductDescription { get; init; }
        public string? UnitMeasure { get; init; }
        public int SystemStock { get; init; }
        public int? PhysicalStock { get; init; }
        public int? Difference { get; init; }
        public int ClosingStock { get; init; }
    }
}
