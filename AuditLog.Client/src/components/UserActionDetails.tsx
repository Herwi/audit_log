import useSWR from "swr";
import { useValidatedFetcher } from "@/hooks/useValidatedFetcher";
import { useFormatters } from "@/hooks/useFormatters";
import * as v from "valibot";
import {
  auditLogEntrySchema,
  formatOperationType,
  formatEntityType,
  type AuditLogEntry,
  OperationType,
} from "@/types/auditLogEntry";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetDescription,
} from "@/components/ui/sheet";

interface UserActionDetailsProps {
  correlationId: string | null;
  organizationId: string | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

const auditLogArraySchema = v.array(auditLogEntrySchema);

const UserActionDetails = ({
  correlationId,
  organizationId,
  open,
  onOpenChange,
}: UserActionDetailsProps) => {
  const { formatDateTime } = useFormatters();
  const fetcher = useValidatedFetcher(auditLogArraySchema);

  const { data, error, isLoading } = useSWR<AuditLogEntry[]>(
    open && correlationId && organizationId
      ? `/api/v1/organizations/${organizationId}/user-actions/${correlationId}/audit-logs`
      : null,
    fetcher
  );

  return (
    <Sheet open={open} onOpenChange={onOpenChange}>
      <SheetContent className="sm:max-w-2xl overflow-y-auto">
        <SheetHeader>
          <SheetTitle>Audit Log Details</SheetTitle>
          <SheetDescription>
            Detailed view of all changes made in this user action
          </SheetDescription>
        </SheetHeader>

        <div className="mt-6 px-6">
          {isLoading && (
            <div className="flex items-center justify-center p-8 text-muted-foreground">
              Loading audit log entries...
            </div>
          )}

          {error && (
            <div className="flex items-center justify-center p-8 text-destructive">
              Error loading audit log entries: {error.message}
            </div>
          )}

          {data && data.length === 0 && (
            <div className="flex items-center justify-center p-8 text-muted-foreground">
              No audit log entries found
            </div>
          )}

          {data && data.length > 0 && (
            <div className="space-y-4">
              {data.map((entry, index) => {
                const oldValues = entry.oldValues
                  ? JSON.parse(entry.oldValues)
                  : null;
                const newValues = entry.newValues
                  ? JSON.parse(entry.newValues)
                  : null;

                // Determine which object to loop through
                const valuesToLoop =
                  entry.operationType === OperationType.Deleted && oldValues
                    ? oldValues
                    : newValues;

                return (
                  <div
                    key={`${entry.entityId}-${index}`}
                    className="rounded-lg border p-4 space-y-3"
                  >
                    <div className="flex items-center gap-4 text-sm">
                      <span className="font-medium">
                        {formatDateTime(entry.createdDate)}
                      </span>
                      <span className="text-muted-foreground">•</span>
                      <span className="font-medium">
                        {formatOperationType(entry.operationType)}
                      </span>
                      <span className="text-muted-foreground">•</span>
                      <span className="text-muted-foreground">
                        {formatEntityType(entry.entityType)}
                      </span>
                    </div>

                    {valuesToLoop && (
                      <div className="space-y-2">
                        {Object.entries(valuesToLoop).map(([key, newValue]) => {
                          const oldValue = oldValues?.[key];
                          const hasChanged = oldValue !== newValue;

                          return (
                            <div
                              key={key}
                              className="flex justify-between items-start gap-4 text-sm"
                            >
                              <span className="text-muted-foreground font-medium">
                                {key}
                              </span>
                              <span className="text-right">
                                {entry.operationType === OperationType.Modified &&
                                hasChanged ? (
                                  <>
                                    <span className="text-muted-foreground">
                                      {String(oldValue)}
                                    </span>
                                    <span className="mx-2 text-muted-foreground">
                                      →
                                    </span>
                                    <span>{String(newValue)}</span>
                                  </>
                                ) : entry.operationType === OperationType.Added ? (
                                  <span>{String(newValue)}</span>
                                ) : (
                                  <span className="text-muted-foreground">
                                    {String(oldValue)}
                                  </span>
                                )}
                              </span>
                            </div>
                          );
                        })}
                      </div>
                    )}
                  </div>
                );
              })}
            </div>
          )}
        </div>
      </SheetContent>
    </Sheet>
  );
};

export default UserActionDetails;
