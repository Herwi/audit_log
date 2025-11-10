namespace AuditLog.Infrastructure.Persistence.Entities;

public class DocumentHeader
{
    public Guid Id { get; set; }

    public string? Number { get; set; }

    public DateOnly? EffectiveDate { get; set; }

    public DateOnly? ExecutionDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateOnly? ConclusionDate { get; set; }

    public string? ContractorId { get; set; }

    public short DocumentType { get; set; }

    public string? Subject { get; set; }

    public decimal? ContractValue { get; set; }

    public Guid? ParentId { get; set; }

    public string? Reason { get; set; }

    public DateTime? DeletedDate { get; set; }

    public DateOnly? PaymentDueDate { get; set; }

    public string? ContractorName { get; set; }

    public Guid OrganizationId { get; set; }

    public Guid? EngagementId { get; set; }

    public bool? IsMultiyear { get; set; }

    public short Confidentiality { get; set; }

    public Guid? SubUnitId { get; set; }

    public short? ContractType { get; set; }

    public bool IsFunded { get; set; }

    public Guid? ContractorId1 { get; set; }

    public int? ContractFlags { get; set; }

    public decimal? ContractNetValue { get; set; }
}
