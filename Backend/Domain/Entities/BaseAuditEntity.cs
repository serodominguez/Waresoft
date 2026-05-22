namespace Domain.Entities
{
    public class BaseAuditEntity : BaseEntity
    {
        public int? AuditUpdateUser { get; set; }
        public DateTime? AuditUpdateDate { get; set; }
    }
}
