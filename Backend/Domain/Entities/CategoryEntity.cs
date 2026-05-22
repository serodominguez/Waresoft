namespace Domain.Entities
{
    public class CategoryEntity : BaseAuditEntity
    {
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }
        public virtual ICollection<ProductEntity> Product { get; set; } = new List<ProductEntity>();
    }
}
