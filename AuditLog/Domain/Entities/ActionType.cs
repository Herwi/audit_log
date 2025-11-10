namespace AuditLog.Domain.Entities;

/// <summary>
/// Represents a user action type, combining operation type (Added/Deleted/Modified)
/// with entity type (Contract/Annex/etc.)
/// </summary>
public enum ActionType
{
    Unknown = 0,

    // Contract Header actions (EntityType.ContractHeaderEntity)
    ContractCreated = 101,
    ContractDeleted = 102,
    ContractModified = 103,

    // Annex Header actions (EntityType.AnnexHeaderEntity)
    AnnexCreated = 201,
    AnnexDeleted = 202,
    AnnexModified = 203,

    // Annex Change actions (EntityType.AnnexChangeEntity)
    AnnexChangeCreated = 301,
    AnnexChangeDeleted = 302,
    AnnexChangeModified = 303,

    // File actions (EntityType.FileEntity)
    FileCreated = 401,
    FileDeleted = 402,
    FileModified = 403,

    // Invoice actions (EntityType.InvoiceEntity)
    InvoiceCreated = 501,
    InvoiceDeleted = 502,
    InvoiceModified = 503,

    // Payment Schedule actions (EntityType.PaymentScheduleEntity)
    PaymentScheduleCreated = 601,
    PaymentScheduleDeleted = 602,
    PaymentScheduleModified = 603,

    // Contract Funding actions (EntityType.ContractFundingEntity)
    ContractFundingCreated = 701,
    ContractFundingDeleted = 702,
    ContractFundingModified = 703
}
