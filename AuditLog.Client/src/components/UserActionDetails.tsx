import useSWR from "swr";
import { useValidatedFetcher } from "@/hooks/useValidatedFetcher";
import { useFormatters } from "@/hooks/useFormatters";
import { useMobileLayout } from "@/hooks/useMobileLayout";
import * as v from "valibot";
import {
  auditLogEntrySchema,
  formatOperationType,
  formatEntityType,
  type AuditLogEntry,
  OperationType,
} from "@/types/auditLogEntry";
import {
  type UserAction,
  formatActionType,
} from "@/types/userAction";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetDescription,
} from "@/components/ui/sheet";

interface UserActionDetailsProps {
  userAction: UserAction | null;
  organizationId: string | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

const auditLogArraySchema = v.array(auditLogEntrySchema);

const UserActionDetails = ({
  userAction,
  organizationId,
  open,
  onOpenChange,
}: UserActionDetailsProps) => {
  const { formatDateTime, formatDuration } = useFormatters();
  const isMobile = useMobileLayout();
  const fetcher = useValidatedFetcher(auditLogArraySchema);

  const { data, error, isLoading } = useSWR<AuditLogEntry[]>(
    open && userAction && organizationId
      ? `/api/v1/organizations/${organizationId}/user-actions/${userAction.correlationId}/audit-logs`
      : null,
    fetcher
  );

  return (
    <Sheet open={open} onOpenChange={onOpenChange}>
      <SheetContent
        side={isMobile ? "bottom" : "right"}
        className={isMobile ? "h-[90vh] overflow-y-auto gap-2" : "sm:max-w-2xl overflow-y-auto gap-2"}
      >
        <SheetHeader>
          <SheetTitle>Audit Log Details</SheetTitle>
          <SheetDescription>
            Detailed view of all changes made in this user action
          </SheetDescription>
        </SheetHeader>

        {userAction && (
          <div className="px-6">
            <h3 className="text-sm font-semibold mb-2">User action details</h3>
            <div className="rounded-lg border bg-muted/50 p-4 space-y-2 text-sm">
              <div className="flex justify-between">
                <span className="text-muted-foreground">User</span>
                <span className="font-medium">{userAction.userEmail}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-muted-foreground">Action Type</span>
                <span className="font-medium">{formatActionType(userAction.actionType)}</span>
              </div>
              {userAction.contractNumber && (
                <div className="flex justify-between">
                  <span className="text-muted-foreground">Contract Number</span>
                  <span className="font-medium">{userAction.contractNumber}</span>
                </div>
              )}
              <div className="flex justify-between">
                <span className="text-muted-foreground">Start Date & Time</span>
                <span className="font-medium">{formatDateTime(userAction.startDate)}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-muted-foreground">Duration</span>
                <span className="font-medium">{formatDuration(userAction.duration)}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-muted-foreground">Changed Entities</span>
                <span className="font-medium">{userAction.changedEntitiesCount}</span>
              </div>
            </div>
          </div>
        )}

        <div className="px-6 pb-6">
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
            <>
              <h3 className="text-sm font-semibold mb-2">Audit log entries</h3>
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
            </>
          )}
        </div>
      </SheetContent>
    </Sheet>
  );
};

export default UserActionDetails;
