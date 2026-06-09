namespace Application.Dtos.Response.Product
{
    public record ProductStatsResponseDto
    {
        public int TotalActive { get; set; }
        public int NewThisMonth { get; set; }
    }
}
