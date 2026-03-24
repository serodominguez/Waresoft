namespace Infrastructure.RateLimit
{
    public class EndpointRateLimitOptions
    {
        public List<EndpointLimitRule> Rules { get; set; } = new();
    }

    public class EndpointLimitRule
    {
        public string PathPrefix { get; set; } = "";
        public string HttpMethod { get; set; } = "*"; // GET, POST o * para todos
        public int MaxRequests { get; set; }
        public int WindowSeconds { get; set; }
        public int StatusCode { get; set; } = 429;
    }
}
