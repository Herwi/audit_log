namespace AuditLog.Application.Endpoints;

using Dtos;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Dtos;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Extension methods for registering user actions endpoints
/// </summary>
public static class UserActionsEndpoints
{
    /// <summary>
    /// Maps all user actions related endpoints to the route group
    /// </summary>
    /// <param name="group">The route group builder (typically /api/v1)</param>
    /// <returns>The route group builder for chaining</returns>
    public static RouteGroupBuilder MapUserActionsEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/organizations/{organizationId:guid}/user-actions", GetUserActions)
            .WithName("GetUserActions")
            .WithOpenApi();

        group.MapGet("/organizations/{organizationId:guid}/user-actions/{userActionId:guid}/audit-logs", GetAuditLogs)
            .WithName("GetAuditLogs")
            .WithOpenApi();

        return group;
    }

    /// <summary>
    /// Gets user actions for a specific organization using optimized raw SQL
    /// </summary>
    /// <param name="organizationId">The organization identifier</param>
    /// <param name="context">Database context</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Number of items per page (default: 10)</param>
    /// <returns>Paginated list of user actions with metadata</returns>
    private static async Task<IResult> GetUserActions(
        Guid organizationId,
        RekrutacjaDbContext context,
        int page = 1,
        int pageSize = 10)
    {
        // Get total count for pagination metadata
        var totalCount = await GetTotalUserActionsCountAsync(organizationId, context);

        // Get paginated data
        var userActionDtos = await GetUserActionDtosAsync(organizationId, context, page, pageSize);
        var userActions = userActionDtos.Select(dto => dto.ToUserAction()).ToList();

        // Calculate pagination metadata
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var paginationMetadata = new PaginationMetadata
        {
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasPreviousPage = page > 1,
            HasNextPage = page < totalPages
        };

        var response = new PagedResponse<Domain.Entities.UserAction>
        {
            Data = userActions,
            Pagination = paginationMetadata
        };

        return Results.Ok(response);
    }

    /// <summary>
    /// Executes raw SQL query to fetch user action DTOs from the database
    /// </summary>
    /// <param name="organizationId">The organization identifier</param>
    /// <param name="context">Database context</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>List of user action DTOs</returns>
    private static async Task<List<UserActionDto>> GetUserActionDtosAsync(
        Guid organizationId,
        RekrutacjaDbContext context,
        int page,
        int pageSize)
    {
        const string sql = """
                           WITH grouped_actions AS (
                               SELECT
                                   correlation_id,
                                   organization_id,
                                   MIN(created_date) as start_date,
                                   MAX(created_date) as end_date,
                                   COUNT(*)::int as changed_entities_count,
                                   (array_agg(user_email ORDER BY created_date))[1] as user_email,
                                   (array_agg(type ORDER BY created_date))[1] as first_type,
                                   (array_agg(entity_type ORDER BY created_date))[1] as first_entity_type,
                                   (array_agg(entity_id ORDER BY created_date))[1] as first_entity_id
                               FROM audit_log
                               WHERE organization_id = {0}
                               GROUP BY correlation_id, organization_id
                           )
                           SELECT
                               ga.correlation_id AS CorrelationId,
                               ga.organization_id AS OrganizationId,
                               ga.user_email AS UserEmail,
                               ga.start_date AS StartDate,
                               ga.end_date AS EndDate,
                               ga.changed_entities_count AS ChangedEntitiesCount,
                               ga.first_type AS FirstType,
                               ga.first_entity_type AS FirstEntityType,
                               CASE
                                   WHEN ga.first_entity_type = 1 THEN dh.number
                                   ELSE NULL
                               END AS ContractNumber
                           FROM grouped_actions ga
                           LEFT JOIN document_header dh
                               ON ga.first_entity_type = 1
                               AND ga.first_entity_id = dh.id
                           ORDER BY ga.start_date DESC
                           OFFSET {1} ROWS
                           FETCH NEXT {2} ROWS ONLY
                           """;

        return await context.Database
            .SqlQueryRaw<UserActionDto>(sql,
                organizationId,
                (page - 1) * pageSize,
                pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// Gets the total count of distinct user actions for an organization using EF Core
    /// </summary>
    /// <param name="organizationId">The organization identifier</param>
    /// <param name="context">Database context</param>
    /// <returns>Total number of user actions</returns>
    private static async Task<int> GetTotalUserActionsCountAsync(
        Guid organizationId,
        RekrutacjaDbContext context)
    {
        return await context.AuditLogs
            .Where(a => a.OrganizationId == organizationId)
            .Select(a => a.CorrelationId)
            .Distinct()
            .CountAsync();
    }

    /// <summary>
    /// Gets all audit log entries for a specific user action (correlation ID)
    /// </summary>
    /// <param name="organizationId">The organization identifier</param>
    /// <param name="userActionId">The user action identifier (correlation ID)</param>
    /// <param name="context">Database context</param>
    /// <returns>List of audit log entries ordered chronologically</returns>
    private static async Task<IResult> GetAuditLogs(
        Guid organizationId,
        Guid userActionId,
        RekrutacjaDbContext context)
    {
        var auditLogEntities = await context.AuditLogs
            .Where(a => a.CorrelationId == userActionId && a.OrganizationId == organizationId)
            .OrderBy(a => a.CreatedDate)
            .ToListAsync();

        var auditLogEntries = auditLogEntities
            .Select(entity => new Domain.Entities.AuditLogEntry
            {
                UserEmail = entity.UserEmail ?? string.Empty,
                OperationType = (Infrastructure.Persistence.Entities.OperationType)entity.Type,
                EntityType = (Infrastructure.Persistence.Entities.EntityType)entity.EntityType,
                CreatedDate = entity.CreatedDate,
                OldValues = entity.OldValues,
                NewValues = entity.NewValues,
                CorrelationId = entity.CorrelationId ?? Guid.Empty,
                EntityId = entity.EntityId ?? Guid.Empty
            })
            .ToList();

        return Results.Ok(auditLogEntries);
    }
}
