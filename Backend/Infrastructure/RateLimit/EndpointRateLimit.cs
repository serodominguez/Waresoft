using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Infrastructure.RateLimit
{
    public class EndpointRateLimit
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly List<EndpointLimitRule> _rules;
        private static readonly object _lockObj = new();

        public EndpointRateLimit(RequestDelegate next, IMemoryCache cache, IOptions<EndpointRateLimitOptions> options)
        {
            _next = next;
            _cache = cache;
            _rules = options.Value.Rules
                .OrderByDescending(r => r.PathPrefix.Length)
                .ToList();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";
            var method = context.Request.Method.ToUpper();
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var rule = _rules.FirstOrDefault(r =>
                path.StartsWith(r.PathPrefix.ToLower()) &&
                (r.HttpMethod == "*" || r.HttpMethod.ToUpper() == method));

            if (rule is not null)
            {
                var slot = (long)(DateTime.UtcNow.Ticks / TimeSpan.FromSeconds(rule.WindowSeconds).Ticks);
                var key = $"ep_rl:{rule.PathPrefix}:{method}:{ip}:{slot}";

                int count;
                lock (_lockObj)
                {
                    count = _cache.GetOrCreate(key, entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(rule.WindowSeconds * 2);
                        return 0;
                    });

                    count++;
                    _cache.Set(key, count, TimeSpan.FromSeconds(rule.WindowSeconds * 2));
                }

                context.Response.Headers["X-RateLimit-Limit"] = rule.MaxRequests.ToString();
                context.Response.Headers["X-RateLimit-Remaining"] = Math.Max(0, rule.MaxRequests - count).ToString();

                if (count > rule.MaxRequests)
                {
                    context.Response.StatusCode = rule.StatusCode;
                    context.Response.Headers["Retry-After"] = rule.WindowSeconds.ToString();
                    await context.Response.WriteAsJsonAsync(new
                    {
                        isSuccess = false,
                        message = "Demasiadas solicitudes. Intente nuevamente en unos segundos."
                    });
                    return;
                }
            }

            await _next(context);
        }
    }
}
