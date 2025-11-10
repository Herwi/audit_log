namespace AuditLog.Infrastructure.Persistence.Dtos;

using AuditLog.Domain.Entities;
using Entities;
using Mappers;

/// <summary>
/// DTO for mapping raw SQL query results to domain entities
/// </summary>
public class UserActionDto
{
    public Guid CorrelationId { get; set; }
    public Guid OrganizationId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int ChangedEntitiesCount { get; set; }
    public int FirstType { get; set; }
    public int FirstEntityType { get; set; }
    public string? ContractNumber { get; set; }

    /// <summary>
    /// Converts the DTO to a domain UserAction entity
    /// </summary>
    /// <returns>UserAction domain entity</returns>
    public UserAction ToUserAction()
    {
        return new UserAction
        {
            CorrelationId = CorrelationId,
            OrganizationId = OrganizationId,
            UserEmail = UserEmail,
            StartDate = StartDate,
            EndDate = EndDate,
            Duration = EndDate - StartDate,
            ChangedEntitiesCount = ChangedEntitiesCount,
            ActionType = ActionTypeMapper.Map(
                (OperationType)FirstType,
                (EntityType)FirstEntityType
            ),
            ContractNumber = ContractNumber
        };
    }
}
