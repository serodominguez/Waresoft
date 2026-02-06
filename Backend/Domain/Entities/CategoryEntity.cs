namespace Domain.Entities
{
    public class CategoryEntity : BaseEntity
    {
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<ProductEntity> Product { get; set; } = new List<ProductEntity>();
    }
}
