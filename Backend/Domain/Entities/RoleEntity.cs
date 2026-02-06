namespace Domain.Entities
{
    public class RoleEntity : BaseEntity
    {
        public string? RoleName { get; set; }
        public virtual ICollection<PermissionEntity> Permission { get; set; } = new List<PermissionEntity>();
        public virtual ICollection<UserEntity> User { get; set; } = new List<UserEntity>();
    }
}
