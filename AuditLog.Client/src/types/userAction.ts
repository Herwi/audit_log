import * as v from "valibot";

/**
 * Action type enum matching backend ActionType
 */
export enum ActionType {
  Unknown = 0,
  ContractCreated = 101,
  ContractDeleted = 102,
  ContractModified = 103,
  AnnexCreated = 201,
  AnnexDeleted = 202,
  AnnexModified = 203,
  AnnexChangeCreated = 301,
  AnnexChangeDeleted = 302,
  AnnexChangeModified = 303,
  FileCreated = 401,
  FileDeleted = 402,
  FileModified = 403,
  InvoiceCreated = 501,
  InvoiceDeleted = 502,
  InvoiceModified = 503,
  PaymentScheduleCreated = 601,
  PaymentScheduleDeleted = 602,
  PaymentScheduleModified = 603,
  ContractFundingCreated = 701,
  ContractFundingDeleted = 702,
  ContractFundingModified = 703,
}

/**
 * Maps ActionType enum values to human-readable text
 */
export function formatActionType(actionType: ActionType): string {
  const actionTypeMap: Record<ActionType, string> = {
    [ActionType.Unknown]: "Unknown",
    [ActionType.ContractCreated]: "Contract Created",
    [ActionType.ContractDeleted]: "Contract Deleted",
    [ActionType.ContractModified]: "Contract Modified",
    [ActionType.AnnexCreated]: "Annex Created",
    [ActionType.AnnexDeleted]: "Annex Deleted",
    [ActionType.AnnexModified]: "Annex Modified",
    [ActionType.AnnexChangeCreated]: "Annex Change Created",
    [ActionType.AnnexChangeDeleted]: "Annex Change Deleted",
    [ActionType.AnnexChangeModified]: "Annex Change Modified",
    [ActionType.FileCreated]: "File Created",
    [ActionType.FileDeleted]: "File Deleted",
    [ActionType.FileModified]: "File Modified",
    [ActionType.InvoiceCreated]: "Invoice Created",
    [ActionType.InvoiceDeleted]: "Invoice Deleted",
    [ActionType.InvoiceModified]: "Invoice Modified",
    [ActionType.PaymentScheduleCreated]: "Payment Schedule Created",
    [ActionType.PaymentScheduleDeleted]: "Payment Schedule Deleted",
    [ActionType.PaymentScheduleModified]: "Payment Schedule Modified",
    [ActionType.ContractFundingCreated]: "Contract Funding Created",
    [ActionType.ContractFundingDeleted]: "Contract Funding Deleted",
    [ActionType.ContractFundingModified]: "Contract Funding Modified",
  };

  return actionTypeMap[actionType] || "Unknown";
}

/**
 * Schema for pagination metadata
 */
export const paginationMetadataSchema = v.object({
  currentPage: v.number(),
  pageSize: v.number(),
  totalCount: v.number(),
  totalPages: v.number(),
  hasPreviousPage: v.boolean(),
  hasNextPage: v.boolean(),
});

export type PaginationMetadata = v.InferOutput<typeof paginationMetadataSchema>;

/**
 * Schema for user action
 */
export const userActionSchema = v.object({
  correlationId: v.pipe(v.string(), v.uuid()),
  userEmail: v.pipe(v.string(), v.email()),
  actionType: v.enum(ActionType),
  contractNumber: v.nullable(v.string()),
  startDate: v.string(), // ISO datetime string (backend sends with microseconds)
  endDate: v.string(), // ISO datetime string (backend sends with microseconds)
  duration: v.string(), // TimeSpan format from backend
  changedEntitiesCount: v.number(),
  organizationId: v.pipe(v.string(), v.uuid()),
});

export type UserAction = v.InferOutput<typeof userActionSchema>;

/**
 * Generic schema for paginated responses
 */
export const pagedResponseSchema = <T extends v.BaseSchema<unknown, unknown, v.BaseIssue<unknown>>>(
  dataSchema: T
) =>
  v.object({
    data: v.array(dataSchema),
    pagination: paginationMetadataSchema,
  });

export type PagedResponse<T> = {
  data: T[];
  pagination: PaginationMetadata;
};
