import { type JSX } from "react";
import { ChevronRight } from "lucide-react";
import { type UserAction, type PaginationMetadata, formatActionType } from "@/types/userAction";
import { useFormatters } from "@/hooks/useFormatters";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  Pagination,
  PaginationContent,
  PaginationItem,
  PaginationLink,
  PaginationNext,
  PaginationPrevious,
  PaginationEllipsis,
} from "@/components/ui/pagination";

interface UserActionsDesktopProps {
  userActions: UserAction[];
  pagination: PaginationMetadata;
  onActionClick: (action: UserAction) => void;
  currentPage: number;
  onPageChange: (page: number) => void;
}

const UserActionsDesktop = ({
  userActions,
  pagination,
  onActionClick,
  currentPage,
  onPageChange,
}: UserActionsDesktopProps) => {
  const { formatDateTime, formatDuration } = useFormatters();

  return (
    <>
      <div className="rounded-md border">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>User Email</TableHead>
              <TableHead>Action Type</TableHead>
              <TableHead>Contract Number</TableHead>
              <TableHead>Start Date & Time</TableHead>
              <TableHead>Duration</TableHead>
              <TableHead>Changed Entities</TableHead>
              <TableHead className="w-12"></TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {userActions.map((action, index) => (
              <TableRow
                key={action.correlationId}
                onClick={() => onActionClick(action)}
                className={`cursor-pointer hover:bg-accent ${index % 2 === 1 ? "bg-muted/50" : ""}`}
              >
                <TableCell className="font-medium">{action.userEmail}</TableCell>
                <TableCell>{formatActionType(action.actionType)}</TableCell>
                <TableCell>{action.contractNumber || "-"}</TableCell>
                <TableCell>{formatDateTime(action.startDate)}</TableCell>
                <TableCell>{formatDuration(action.duration)}</TableCell>
                <TableCell>{action.changedEntitiesCount}</TableCell>
                <TableCell>
                  <ChevronRight className="h-4 w-4 text-muted-foreground" />
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>

      <div className="flex items-center justify-end gap-2 flex-nowrap">
        <div className="text-sm text-muted-foreground whitespace-nowrap">
          Page {pagination.currentPage} of {pagination.totalPages} · {pagination.totalCount} actions
        </div>
        <Pagination className="justify-end">
          <PaginationContent className="justify-end">
            <PaginationItem>
              <PaginationPrevious
                onClick={() => onPageChange(Math.max(1, currentPage - 1))}
                className={!pagination.hasPreviousPage ? "pointer-events-none opacity-50" : "cursor-pointer"}
              />
            </PaginationItem>

            {(() => {
              const pages: JSX.Element[] = [];
              const totalPages = pagination.totalPages;
              const current = pagination.currentPage;

              if (totalPages <= 7) {
                for (let i = 1; i <= totalPages; i++) {
                  pages.push(
                    <PaginationItem key={i}>
                      <PaginationLink
                        onClick={i === current ? undefined : () => onPageChange(i)}
                        isActive={i === current}
                        className={i === current ? "pointer-events-none" : "cursor-pointer"}
                      >
                        {i}
                      </PaginationLink>
                    </PaginationItem>
                  );
                }
              } else {
                pages.push(
                  <PaginationItem key={1}>
                    <PaginationLink
                      onClick={current === 1 ? undefined : () => onPageChange(1)}
                      isActive={current === 1}
                      className={current === 1 ? "pointer-events-none" : "cursor-pointer"}
                    >
                      1
                    </PaginationLink>
                  </PaginationItem>
                );

                if (current > 3) {
                  pages.push(
                    <PaginationItem key="ellipsis-start">
                      <PaginationEllipsis />
                    </PaginationItem>
                  );
                }

                const startPage = Math.max(2, current - 1);
                const endPage = Math.min(totalPages - 1, current + 1);

                for (let i = startPage; i <= endPage; i++) {
                  pages.push(
                    <PaginationItem key={i}>
                      <PaginationLink
                        onClick={i === current ? undefined : () => onPageChange(i)}
                        isActive={i === current}
                        className={i === current ? "pointer-events-none" : "cursor-pointer"}
                      >
                        {i}
                      </PaginationLink>
                    </PaginationItem>
                  );
                }

                if (current < totalPages - 2) {
                  pages.push(
                    <PaginationItem key="ellipsis-end">
                      <PaginationEllipsis />
                    </PaginationItem>
                  );
                }

                pages.push(
                  <PaginationItem key={totalPages}>
                    <PaginationLink
                      onClick={current === totalPages ? undefined : () => onPageChange(totalPages)}
                      isActive={current === totalPages}
                      className={current === totalPages ? "pointer-events-none" : "cursor-pointer"}
                    >
                      {totalPages}
                    </PaginationLink>
                  </PaginationItem>
                );
              }

              return pages;
            })()}

            <PaginationItem>
              <PaginationNext
                onClick={() => onPageChange(Math.min(pagination.totalPages, currentPage + 1))}
                className={!pagination.hasNextPage ? "pointer-events-none opacity-50" : "cursor-pointer"}
              />
            </PaginationItem>
          </PaginationContent>
        </Pagination>
      </div>
    </>
  );
};

export default UserActionsDesktop;
