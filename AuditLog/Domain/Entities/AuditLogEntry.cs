using AuditLog.Infrastructure.Persistence.Entities;

namespace AuditLog.Domain.Entities;

public class AuditLogEntry
{
    public required string UserEmail { get; init; }
    public required OperationType OperationType { get; init; }
    public required EntityType EntityType { get; init; }
    public required DateTime CreatedDate { get; init; }
    public required string? OldValues { get; init; }
    public required string? NewValues { get; init; }
    public required Guid CorrelationId { get; init; }
    public required Guid EntityId { get; init; }
}
