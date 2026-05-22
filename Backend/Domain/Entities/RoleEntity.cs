namespace Domain.Entities
{
    public class RoleEntity : BaseAuditEntity
    {
        public string? RoleName { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<PermissionEntity> Permission { get; set; } = new List<PermissionEntity>();
        public virtual ICollection<UserEntity> User { get; set; } = new List<UserEntity>();
    }
}
