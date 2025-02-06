using System.Threading.RateLimiting;

namespace Presentation.Extensions
{
    internal static class RateLimiterExtensions
    {
        internal const string UserRatePolicy = "user_rate_policy";
        internal static IServiceCollection AddRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.AddPolicy(UserRatePolicy, httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.User.Identity?.Name?.ToString(),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromSeconds(10)
                        }));
            });
            return services;
        }
    }
}
