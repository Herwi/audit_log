namespace AuditLog.Application.Endpoints;

using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Extension methods for registering organizations endpoints
/// </summary>
public static class OrganizationsEndpoints
{
    /// <summary>
    /// Maps all organizations related endpoints to the route group
    /// </summary>
    /// <param name="group">The route group builder (typically /api/v1)</param>
    /// <returns>The route group builder for chaining</returns>
    public static RouteGroupBuilder MapOrganizationsEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/organizations", GetOrganizations)
            .WithName("GetOrganizations")
            .WithOpenApi();

        return group;
    }

    /// <summary>
    /// Gets distinct organization IDs from audit log using EF Core
    /// </summary>
    /// <param name="context">Database context</param>
    /// <returns>List of distinct organization IDs</returns>
    private static async Task<IResult> GetOrganizations(RekrutacjaDbContext context)
    {
        var organizationIds = await context.AuditLogs
            .Where(a => a.OrganizationId != null)
            .Select(a => a.OrganizationId!.Value)
            .Distinct()
            .OrderBy(id => id)
            .ToListAsync();

        return Results.Ok(organizationIds);
    }
}
