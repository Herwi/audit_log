import { useState, useEffect } from "react";
import useSWR from "swr";
import { useOrganizations } from "@/contexts/OrganizationsContext";
import { useValidatedFetcher } from "@/hooks/useValidatedFetcher";
import { useMobileLayout } from "@/hooks/useMobileLayout";
import {
  pagedResponseSchema,
  userActionSchema,
  type UserAction,
  type PagedResponse,
} from "@/types/userAction";
import UserActionDetails from "@/components/UserActionDetails";
import UserActionsMobile from "./UserActionsMobile";
import UserActionsDesktop from "./UserActionsDesktop";

const UserActions = () => {
  const { activeOrganization } = useOrganizations();
  const [currentPage, setCurrentPage] = useState(1);
  const [selectedUserAction, setSelectedUserAction] = useState<UserAction | null>(null);
  const [sheetOpen, setSheetOpen] = useState(false);
  const isMobile = useMobileLayout();

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

  const handleActionClick = (action: UserAction) => {
    setSelectedUserAction(action);
    setSheetOpen(true);
  };

  if (!activeOrganization) {
    return (
      <div className="flex items-center justify-center p-8 text-muted-foreground">
        Please select an organization to view user actions
      </div>
    );
  }

  if (isLoading && !data) {
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
      {isMobile ? (
        <UserActionsMobile
          userActions={userActions}
          pagination={pagination}
          onActionClick={handleActionClick}
          currentPage={currentPage}
          onPageChange={setCurrentPage}
        />
      ) : (
        <UserActionsDesktop
          userActions={userActions}
          pagination={pagination}
          onActionClick={handleActionClick}
          currentPage={currentPage}
          onPageChange={setCurrentPage}
        />
      )}

      <UserActionDetails
        userAction={selectedUserAction}
        organizationId={activeOrganization}
        open={sheetOpen}
        onOpenChange={setSheetOpen}
      />
    </div>
  );
};

export default UserActions;
