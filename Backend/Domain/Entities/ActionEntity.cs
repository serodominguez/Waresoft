namespace Domain.Entities
{
    public class ActionEntity : BaseAuditEntity
    {
        public string? ActionName { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<PermissionEntity> Permissions { get; set; } = new List<PermissionEntity>();
    }
}
