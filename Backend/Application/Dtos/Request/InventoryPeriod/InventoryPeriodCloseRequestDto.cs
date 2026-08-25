namespace Application.Dtos.Request.InventoryPeriod
{
    public record InventoryPeriodCloseRequestDto
    {
        public int IdPeriod { get; init; }
        public List<InventoryPeriodPhysicalCountDto> PhysicalCounts { get; init; } = new();
    }
}
