namespace Domain.Entities
{
    public class ModuleEntity : BaseAuditEntity
    {
        public string? ModuleName { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<PermissionEntity> Permission { get; set; } = new List<PermissionEntity>();
    }
}
