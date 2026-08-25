namespace Application.Dtos.Request.InventoryPeriod
{
    public record InventoryPeriodOpenRequestDto
    {
        public string? PeriodName { get; init; }
        public DateTime? StartDate { get; init; }
        public DateTime? EndDate { get; init; }
    }
}
