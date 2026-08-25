namespace Application.Dtos.Response.InventoryPeriod
{
    public record InventoryPeriodClosingResponseDto
    {
        public int IdPeriod { get; init; }
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
