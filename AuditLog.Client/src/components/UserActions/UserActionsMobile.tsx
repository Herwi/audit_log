import { ChevronRight, Calendar, Clock } from "lucide-react";
import { type UserAction, type PaginationMetadata, formatActionType } from "@/types/userAction";
import { useFormatters } from "@/hooks/useFormatters";
import {
  PaginationPrevious,
  PaginationNext,
} from "@/components/ui/pagination";

interface UserActionsMobileProps {
  userActions: UserAction[];
  pagination: PaginationMetadata;
  onActionClick: (action: UserAction) => void;
  currentPage: number;
  onPageChange: (page: number) => void;
}

const UserActionsMobile = ({
  userActions,
  pagination,
  onActionClick,
  currentPage,
  onPageChange,
}: UserActionsMobileProps) => {
  const { formatDateTime, formatDuration } = useFormatters();

  return (
    <>
      <div className="space-y-4 pb-24">
        {userActions.map((action) => (
          <div
            key={action.correlationId}
            onClick={() => onActionClick(action)}
            className="border rounded-lg shadow-sm cursor-pointer active:bg-accent flex overflow-hidden"
          >
            <div className="flex-1 p-4">
              <div className="mb-3">
                <div className="font-semibold text-base mb-1">
                  {action.userEmail}
                </div>
                <div className="text-sm">
                  <span className="font-medium">{formatActionType(action.actionType)}</span>
                  {action.contractNumber && <span className="text-muted-foreground font-normal"> (#{action.contractNumber})</span>}
                </div>
              </div>

              <div className="grid grid-cols-[2fr_auto_auto] gap-3 text-sm">
                <div className="flex items-center gap-1.5">
                  <Calendar className="h-4 w-4 text-muted-foreground flex-shrink-0" />
                  <span className="text-foreground">
                    {formatDateTime(action.startDate)}
                  </span>
                </div>
                <div className="flex items-center gap-1.5">
                  <Clock className="h-4 w-4 text-muted-foreground flex-shrink-0" />
                  <span className="text-foreground">
                    {formatDuration(action.duration)}
                  </span>
                </div>
                <div className="flex items-center gap-1.5">
                  <span className="font-medium text-foreground">
                    {action.changedEntitiesCount} <span className="text-muted-foreground font-normal">entities</span>
                  </span>
                </div>
              </div>
            </div>

            <div className="flex items-center justify-center px-4 bg-muted/30">
              <ChevronRight className="h-5 w-5 text-muted-foreground" />
            </div>
          </div>
        ))}
      </div>

      <div className="fixed bottom-0 left-0 right-0 bg-background border-t p-4 flex items-center justify-between gap-4">
        <div className="text-sm text-muted-foreground">
          Page {pagination.currentPage} of {pagination.totalPages}
        </div>
        <div className="flex gap-2">
          <PaginationPrevious
            onClick={() => onPageChange(Math.max(1, currentPage - 1))}
            className={`${!pagination.hasPreviousPage ? "pointer-events-none opacity-50" : "cursor-pointer"} h-11 px-4`}
          />
          <PaginationNext
            onClick={() => onPageChange(Math.min(pagination.totalPages, currentPage + 1))}
            className={`${!pagination.hasNextPage ? "pointer-events-none opacity-50" : "cursor-pointer"} h-11 px-4`}
          />
        </div>
      </div>
    </>
  );
};

export default UserActionsMobile;
