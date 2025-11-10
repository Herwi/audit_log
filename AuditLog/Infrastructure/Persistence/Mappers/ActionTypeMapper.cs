namespace AuditLog.Infrastructure.Persistence.Mappers;

using AuditLog.Infrastructure.Persistence.Entities;
using DomainActionType = AuditLog.Domain.Entities.ActionType;

/// <summary>
/// Maps operation type and entity type to a combined ActionType enum value
/// </summary>
public static class ActionTypeMapper
{
    /// <summary>
    /// Maps a combination of Type (Added/Deleted/Modified) and EntityType
    /// to a specific ActionType enum value
    /// </summary>
    /// <param name="operationType">The operation performed (Added, Deleted, Modified)</param>
    /// <param name="entityType">The type of entity affected</param>
    /// <returns>The corresponding ActionType, or Unknown if no mapping exists</returns>
    public static DomainActionType Map(Type operationType, EntityType entityType)
    {
        return (operationType, entityType) switch
        {
            // Contract Header actions
            (Type.Added, EntityType.ContractHeaderEntity) => DomainActionType.ContractCreated,
            (Type.Deleted, EntityType.ContractHeaderEntity) => DomainActionType.ContractDeleted,
            (Type.Modified, EntityType.ContractHeaderEntity) => DomainActionType.ContractModified,

            // Annex Header actions
            (Type.Added, EntityType.AnnexHeaderEntity) => DomainActionType.AnnexCreated,
            (Type.Deleted, EntityType.AnnexHeaderEntity) => DomainActionType.AnnexDeleted,
            (Type.Modified, EntityType.AnnexHeaderEntity) => DomainActionType.AnnexModified,

            // Annex Change actions
            (Type.Added, EntityType.AnnexChangeEntity) => DomainActionType.AnnexChangeCreated,
            (Type.Deleted, EntityType.AnnexChangeEntity) => DomainActionType.AnnexChangeDeleted,
            (Type.Modified, EntityType.AnnexChangeEntity) => DomainActionType.AnnexChangeModified,

            // File actions
            (Type.Added, EntityType.FileEntity) => DomainActionType.FileCreated,
            (Type.Deleted, EntityType.FileEntity) => DomainActionType.FileDeleted,
            (Type.Modified, EntityType.FileEntity) => DomainActionType.FileModified,

            // Invoice actions
            (Type.Added, EntityType.InvoiceEntity) => DomainActionType.InvoiceCreated,
            (Type.Deleted, EntityType.InvoiceEntity) => DomainActionType.InvoiceDeleted,
            (Type.Modified, EntityType.InvoiceEntity) => DomainActionType.InvoiceModified,

            // Payment Schedule actions
            (Type.Added, EntityType.PaymentScheduleEntity) => DomainActionType.PaymentScheduleCreated,
            (Type.Deleted, EntityType.PaymentScheduleEntity) => DomainActionType.PaymentScheduleDeleted,
            (Type.Modified, EntityType.PaymentScheduleEntity) => DomainActionType.PaymentScheduleModified,

            // Contract Funding actions
            (Type.Added, EntityType.ContractFundingEntity) => DomainActionType.ContractFundingCreated,
            (Type.Deleted, EntityType.ContractFundingEntity) => DomainActionType.ContractFundingDeleted,
            (Type.Modified, EntityType.ContractFundingEntity) => DomainActionType.ContractFundingModified,

            // Unknown entity type or operation type
            _ => DomainActionType.Unknown
        };
    }
}
