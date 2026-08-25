namespace Application.Dtos.Response.InventoryPeriod
{
    public record InventoryPeriodOpeningResponseDto
    {
        public int IdPeriod { get; init; }
        public int IdProduct { get; init; }
        public string? ProductCode { get; init; }
        public string? ProductDescription { get; init; }
        public string? UnitMeasure { get; init; }
        public int OpeningStock { get; init; }
    }
}
