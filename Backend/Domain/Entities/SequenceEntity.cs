namespace Domain.Entities
{
    public class SequenceEntity
    {
        public string Name { get; set; } = null!;
        public int CurrentValue { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
