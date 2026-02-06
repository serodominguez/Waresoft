namespace Domain.Entities
{
    public class UserEntity : BaseEntity
    {
        public string? UserName { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string? Names { get; set; }
        public string? LastNames { get; set; }
        public string? IdentificationNumber { get; set; }
        public int? PhoneNumber { get; set; }
        public int IdRole { get; set; }
        public int IdStore { get; set; }
        public virtual RoleEntity Role { get; set; } = null!;
        public virtual StoreEntity Store { get; set; } = null!;
        public virtual ICollection<GoodsIssueEntity> GoodsIssue { get; set; } = new List<GoodsIssueEntity>();
    }
}
