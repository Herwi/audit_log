namespace AuditLog.Domain.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid? UserId { get; set; }
    public string? UserEmail { get; set; }
    public Type Type { get; set; }
    public EntityType EntityType { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? AffectedColumns { get; set; }
    public string? PrimaryKey { get; set; }
    public Guid? EntityId { get; set; }
    public Guid? ParentId { get; set; }
    public Guid? CorrelationId { get; set; }
    public Guid? SubUnitId { get; set; }
}
