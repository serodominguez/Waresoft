namespace Domain.Entities
{
    public class GoodsIssueEntity : BaseEntity
    {
        public string? Code { get; set; }
        public string? Type { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Annotations { get; set; }
        public int? IdUser { get; set; }
        public int IdStore { get; set; }
        public int Status { get; set; }

        public virtual UserEntity? User { get; set; }
        public virtual StoreEntity Store { get; set; } = null!;
        public virtual ICollection<GoodsIssueDetailsEntity> GoodsIssueDetails { get; set; } = null!;
    }
}
