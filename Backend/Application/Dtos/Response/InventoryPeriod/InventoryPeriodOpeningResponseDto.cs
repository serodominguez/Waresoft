namespace Application.Dtos.Response.InventoryPeriod
{
    public record InventoryPeriodOpeningResponseDto
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
        public int TotalProducts { get; init; }
        public int TotalOpeningStock { get; init; }
        public List<InventoryPeriodOpeningItemResponseDto> Items { get; init; } = [];
    }

    public record InventoryPeriodOpeningItemResponseDto
    {
        public int IdProduct { get; init; }
        public string? ProductCode { get; init; }
        public string? ProductDescription { get; init; }
        public string? UnitMeasure { get; init; }
        public int OpeningStock { get; init; }
    }
}
