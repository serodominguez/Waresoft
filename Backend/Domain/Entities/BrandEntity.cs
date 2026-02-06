namespace Domain.Entities
{
    public class BrandEntity : BaseEntity
    {
        public string? BrandName { get; set; }
        public virtual ICollection<ProductEntity> Product { get; set; } = new List<ProductEntity>();
    }
}
