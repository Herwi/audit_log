import { createContext, useContext, useEffect, useState } from "react";
import useSWR from "swr";

interface OrganizationsContextType {
  organizations: string[] | undefined;
  activeOrganization: string | null;
  setActiveOrganization: (orgId: string) => void;
  isLoading: boolean;
  error: Error | undefined;
}

const OrganizationsContext = createContext<OrganizationsContextType | undefined>(undefined);

const fetcher = (url: string) => fetch(url).then((res) => res.json());

export const OrganizationsProvider = ({ children }: { children: React.ReactNode }) => {
  const { data: organizations, error, isLoading } = useSWR<string[]>(
    "/api/v1/organizations",
    fetcher
  );

  const [activeOrganization, setActiveOrganization] = useState<string | null>(null);

  useEffect(() => {
    if (organizations && organizations.length > 0 && !activeOrganization) {
      setActiveOrganization(organizations[0]);
    }
  }, [organizations, activeOrganization]);

  return (
    <OrganizationsContext.Provider
      value={{
        organizations,
        activeOrganization,
        setActiveOrganization,
        isLoading,
        error,
      }}
    >
      {children}
    </OrganizationsContext.Provider>
  );
};

export const useOrganizations = () => {
  const context = useContext(OrganizationsContext);
  if (context === undefined) {
    throw new Error("useOrganizations must be used within an OrganizationsProvider");
  }
  return context;
};
