using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BugTrackerBackend;

public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
    {
        Claim? scopeClaim = context.User.Claims.FirstOrDefault(c => c.Type == "scope" && c.Issuer == requirement.Issuer);
        if (scopeClaim is null) return Task.CompletedTask;

        string[] scopes = scopeClaim.Value.Split(" ");
        if (scopes.Any(s => s == requirement.Scope))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
