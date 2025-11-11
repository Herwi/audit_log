import { useState, useEffect } from "react";
import useSWR from "swr";
import { ChevronRight } from "lucide-react";
import { useOrganizations } from "@/contexts/OrganizationsContext";
import { useValidatedFetcher } from "@/hooks/useValidatedFetcher";
import { useFormatters } from "@/hooks/useFormatters";
import {
  pagedResponseSchema,
  userActionSchema,
  type UserAction,
  type PagedResponse,
  formatActionType,
} from "@/types/userAction";
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
import UserActionDetails from "@/components/UserActionDetails";

const UserActionsTable = () => {
  const { activeOrganization } = useOrganizations();
  const [currentPage, setCurrentPage] = useState(1);
  const [selectedUserAction, setSelectedUserAction] = useState<UserAction | null>(null);
  const [sheetOpen, setSheetOpen] = useState(false);
  const { formatDateTime, formatDuration } = useFormatters();

  useEffect(() => {
    setCurrentPage(1);
  }, [activeOrganization]);

  const fetcher = useValidatedFetcher(pagedResponseSchema(userActionSchema));

  const { data, error, isLoading } = useSWR<PagedResponse<UserAction>>(
    activeOrganization
      ? `/api/v1/organizations/${activeOrganization}/user-actions?page=${currentPage}&pageSize=10`
      : null,
    fetcher,
    {
      keepPreviousData: true,
    }
  );

  if (!activeOrganization) {
    return (
      <div className="flex items-center justify-center p-8 text-muted-foreground">
        Please select an organization to view user actions
      </div>
    );
  }

  if (isLoading) {
    return (
      <div className="flex items-center justify-center p-8 text-muted-foreground">
        Loading user actions...
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex items-center justify-center p-8 text-destructive">
        Error loading user actions: {error.message}
      </div>
    );
  }

  if (!data || data.data.length === 0) {
    return (
      <div className="flex items-center justify-center p-8 text-muted-foreground">
        No user actions found for this organization
      </div>
    );
  }

  const { data: userActions, pagination } = data;

  return (
    <div className="space-y-4">
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
                onClick={() => {
                  setSelectedUserAction(action);
                  setSheetOpen(true);
                }}
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
                onClick={() => setCurrentPage((prev) => Math.max(1, prev - 1))}
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
                        onClick={i === current ? undefined : () => setCurrentPage(i)}
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
                      onClick={current === 1 ? undefined : () => setCurrentPage(1)}
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
                        onClick={i === current ? undefined : () => setCurrentPage(i)}
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
                      onClick={current === totalPages ? undefined : () => setCurrentPage(totalPages)}
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
                onClick={() => setCurrentPage((prev) => Math.min(pagination.totalPages, prev + 1))}
                className={!pagination.hasNextPage ? "pointer-events-none opacity-50" : "cursor-pointer"}
              />
            </PaginationItem>
          </PaginationContent>
        </Pagination>
      </div>

      <UserActionDetails
        userAction={selectedUserAction}
        organizationId={activeOrganization}
        open={sheetOpen}
        onOpenChange={setSheetOpen}
      />
    </div>
  );
};

export default UserActionsTable;
