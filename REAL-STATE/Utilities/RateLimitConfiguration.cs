using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace REAL_STATE.Utilities;

public static class RateLimitConfiguration
{
    public const string PublicWrites = "public-writes";
    public static IServiceCollection AddApplicationRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy(PublicWrites, context => RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions { PermitLimit = 10, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                if (!HttpMethods.IsPost(context.Request.Method) ||
                    (!context.Request.Path.StartsWithSegments("/Identity/Account")
                        || context.Request.Path.StartsWithSegments("/Identity/Account/Logout")))
                    return RateLimitPartition.GetNoLimiter("unrestricted");
                return RateLimitPartition.GetFixedWindowLimiter(
                    context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions { PermitLimit = 10, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 });
            });
            options.OnRejected = async (context, token) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    context.HttpContext.Response.Headers.RetryAfter = Math.Ceiling(retryAfter.TotalSeconds).ToString(System.Globalization.CultureInfo.InvariantCulture);
                await context.HttpContext.Response.WriteAsync("Too many requests. Please try again shortly.", token);
            };
        });
        return services;
    }
}
