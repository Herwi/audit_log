namespace AuditLog.Domain.Entities;

/// <summary>
/// Represents a user action aggregated from multiple audit log entries
/// sharing the same correlation_id
/// </summary>
public class UserAction
{
    /// <summary>
    /// Correlation ID that groups all audit log entries for this action
    /// </summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Email of the user who performed the action
    /// </summary>
    public string UserEmail { get; set; } = string.Empty;

    /// <summary>
    /// Type of action performed (e.g., ContractCreated, AnnexModified)
    /// Determined from the first chronological audit log entry
    /// </summary>
    public ActionType ActionType { get; set; }

    /// <summary>
    /// Contract number from document_header table
    /// Only populated if the action involves ContractHeaderEntity
    /// </summary>
    public string? ContractNumber { get; set; }

    /// <summary>
    /// Start date/time of the action (earliest audit log entry)
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// End date/time of the action (latest audit log entry)
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Duration of the action (difference between EndDate and StartDate)
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Number of entities changed in this action
    /// (count of audit log entries with this correlation_id)
    /// </summary>
    public int ChangedEntitiesCount { get; set; }

    /// <summary>
    /// Organization ID for filtering actions by organization context
    /// </summary>
    public Guid OrganizationId { get; set; }
}
