namespace Domain.Entities
{
    public class ActionEntity : BaseEntity
    {
        public string? ActionName { get; set; }

        public virtual ICollection<PermissionEntity> Permissions { get; set; } = new List<PermissionEntity>();
    }
}
