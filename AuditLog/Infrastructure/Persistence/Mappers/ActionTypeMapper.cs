namespace AuditLog.Infrastructure.Persistence.Mappers;

using Entities;
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
    /// <param name="operationOperationType">The operation performed (Added, Deleted, Modified)</param>
    /// <param name="entityType">The type of entity affected</param>
    /// <returns>The corresponding ActionType, or Unknown if no mapping exists</returns>
    public static DomainActionType Map(OperationType operationOperationType, EntityType entityType)
    {
        return (operationType: operationOperationType, entityType) switch
        {
            // Contract Header actions
            (OperationType.Added, EntityType.ContractHeaderEntity) => DomainActionType.ContractCreated,
            (OperationType.Deleted, EntityType.ContractHeaderEntity) => DomainActionType.ContractDeleted,
            (OperationType.Modified, EntityType.ContractHeaderEntity) => DomainActionType.ContractModified,

            // Annex Header actions
            (OperationType.Added, EntityType.AnnexHeaderEntity) => DomainActionType.AnnexCreated,
            (OperationType.Deleted, EntityType.AnnexHeaderEntity) => DomainActionType.AnnexDeleted,
            (OperationType.Modified, EntityType.AnnexHeaderEntity) => DomainActionType.AnnexModified,

            // Annex Change actions
            (OperationType.Added, EntityType.AnnexChangeEntity) => DomainActionType.AnnexChangeCreated,
            (OperationType.Deleted, EntityType.AnnexChangeEntity) => DomainActionType.AnnexChangeDeleted,
            (OperationType.Modified, EntityType.AnnexChangeEntity) => DomainActionType.AnnexChangeModified,

            // File actions
            (OperationType.Added, EntityType.FileEntity) => DomainActionType.FileCreated,
            (OperationType.Deleted, EntityType.FileEntity) => DomainActionType.FileDeleted,
            (OperationType.Modified, EntityType.FileEntity) => DomainActionType.FileModified,

            // Invoice actions
            (OperationType.Added, EntityType.InvoiceEntity) => DomainActionType.InvoiceCreated,
            (OperationType.Deleted, EntityType.InvoiceEntity) => DomainActionType.InvoiceDeleted,
            (OperationType.Modified, EntityType.InvoiceEntity) => DomainActionType.InvoiceModified,

            // Payment Schedule actions
            (OperationType.Added, EntityType.PaymentScheduleEntity) => DomainActionType.PaymentScheduleCreated,
            (OperationType.Deleted, EntityType.PaymentScheduleEntity) => DomainActionType.PaymentScheduleDeleted,
            (OperationType.Modified, EntityType.PaymentScheduleEntity) => DomainActionType.PaymentScheduleModified,

            // Contract Funding actions
            (OperationType.Added, EntityType.ContractFundingEntity) => DomainActionType.ContractFundingCreated,
            (OperationType.Deleted, EntityType.ContractFundingEntity) => DomainActionType.ContractFundingDeleted,
            (OperationType.Modified, EntityType.ContractFundingEntity) => DomainActionType.ContractFundingModified,

            // Unknown entity type or operation type
            _ => DomainActionType.Unknown
        };
    }
}
