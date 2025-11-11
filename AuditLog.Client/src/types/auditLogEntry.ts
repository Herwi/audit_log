import * as v from "valibot";

export enum OperationType {
  Added = 1,
  Deleted = 2,
  Modified = 3,
}

export enum EntityType {
  Unknown = 0,
  ContractHeaderEntity = 1,
  AnnexHeaderEntity = 2,
  AnnexChangeEntity = 3,
  FileEntity = 4,
  InvoiceEntity = 5,
  PaymentScheduleEntity = 6,
  ContractFundingEntity = 7,
}

export const auditLogEntrySchema = v.object({
  userEmail: v.string(),
  operationType: v.number(),
  entityType: v.number(),
  createdDate: v.string(),
  oldValues: v.nullable(v.string()),
  newValues: v.nullable(v.string()),
  correlationId: v.string(),
  entityId: v.string(),
});

export type AuditLogEntry = v.InferOutput<typeof auditLogEntrySchema>;

export function formatOperationType(type: number): string {
  switch (type) {
    case OperationType.Added:
      return "Added";
    case OperationType.Deleted:
      return "Deleted";
    case OperationType.Modified:
      return "Modified";
    default:
      return "Unknown";
  }
}

export function formatEntityType(type: number): string {
  switch (type) {
    case EntityType.ContractHeaderEntity:
      return "Contract Header";
    case EntityType.AnnexHeaderEntity:
      return "Annex Header";
    case EntityType.AnnexChangeEntity:
      return "Annex Change";
    case EntityType.FileEntity:
      return "File";
    case EntityType.InvoiceEntity:
      return "Invoice";
    case EntityType.PaymentScheduleEntity:
      return "Payment Schedule";
    case EntityType.ContractFundingEntity:
      return "Contract Funding";
    case EntityType.Unknown:
    default:
      return "Unknown";
  }
}
