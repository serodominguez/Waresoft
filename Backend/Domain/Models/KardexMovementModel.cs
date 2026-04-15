namespace Domain.Models
{
    public record KardexMovementModel
    {
        public int IdProduct { get; init; }
        public int Quantity { get; init; }
        public int IdMovement { get; init; }
        public string? Code { get; init; }
        public DateTime Date { get; init; }
        public string? MovementType { get; init; }
        public string? Type { get; init; }
        public int? State { get; init; }
        public int AccumulatedStock { get; init; }
    }
}
