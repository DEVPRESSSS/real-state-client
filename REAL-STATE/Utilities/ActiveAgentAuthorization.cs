using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using REAL_STATE.Data;

namespace REAL_STATE.Utilities;

public sealed class ActiveAgentRequirement : IAuthorizationRequirement { }

public sealed class ActiveAgentHandler(ApplicationDbContext db) : AuthorizationHandler<ActiveAgentRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ActiveAgentRequirement requirement)
    {
        var id = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id != null && context.User.IsInRole(RoleSD.Agent) &&
            await db.Agents.AsNoTracking().AnyAsync(a => a.UserId == id && a.IsActive))
            context.Succeed(requirement);
    }
}
