namespace Domain.Entities
{
    public class ModuleEntity : BaseEntity
    {
        public string? ModuleName { get; set; }

        public virtual ICollection<PermissionEntity> Permission { get; set; } = new List<PermissionEntity>();
    }
}
