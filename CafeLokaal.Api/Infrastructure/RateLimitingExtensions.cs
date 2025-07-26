using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace CafeLokaal.Api.Infrastructure;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddCafeLokaalRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Dashboard data endpoint rate limit: 30 requests per minute per cafe
            options.AddPolicy("dashboard", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.FindFirst("sub")?.Value ?? httpContext.Request.Headers.Host.ToString(),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 30,
                        QueueLimit = 0,
                        Window = TimeSpan.FromMinutes(1)
                    }));

            // POS data integration endpoint rate limit: 60 requests per minute per cafe
            options.AddPolicy("posdata", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.FindFirst("sub")?.Value ?? httpContext.Request.Headers.Host.ToString(),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 60,
                        QueueLimit = 0,
                        Window = TimeSpan.FromMinutes(1)
                    }));
        });

        return services;
    }
}
