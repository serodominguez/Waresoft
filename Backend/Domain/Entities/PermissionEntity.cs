namespace Domain.Entities
{
    public class PermissionEntity : BaseEntity
    {
        public int IdRole { get; set; }
        public int IdModule { get; set; }
        public int IdAction { get; set; }

        public virtual RoleEntity Role { get; set; } = null!;
        public virtual ModuleEntity Module { get; set; } = null!;
        public virtual ActionEntity Action { get; set; } = null!;
    }
}
