namespace Application.Dtos.Response.GoodsIssue
{
    public class GoodsIssueResponseDto
    {
        public int IdIssue { get; set; }
        public string? Code { get; set; }
        public string? Type { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Annotations { get; set; }
        public int IdUser { get; set; }
        public string? UserName { get; set; }
        public int IdStore { get; set; }
        public string? StoreName { get; set; }
        public int? AuditCreateUser { get; set; }
        public string? AuditCreateDate { get; set; }
        public bool Status { get; set; }
        public string? StatusIssue { get; set; }
    }
}
