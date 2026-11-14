using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
namespace REAL_STATE.Data;
public sealed class DatabaseHealthCheck(ApplicationDbContext db) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (!await db.Database.CanConnectAsync(cancellationToken)) return HealthCheckResult.Unhealthy();
        if ((await db.Database.GetPendingMigrationsAsync(cancellationToken)).Any()) return HealthCheckResult.Unhealthy();
        var requiredRoles = new[] { REAL_STATE.Utilities.RoleSD.User, REAL_STATE.Utilities.RoleSD.Admin, REAL_STATE.Utilities.RoleSD.Agent };
        return await db.Roles.CountAsync(r => r.Name != null && requiredRoles.Contains(r.Name), cancellationToken) == requiredRoles.Length
            ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
    }
}
