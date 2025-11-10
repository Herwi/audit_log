import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { useOrganizations } from "@/contexts/OrganizationsContext";

const Navbar = () => {
    const { organizations, activeOrganization, setActiveOrganization, isLoading, error } = useOrganizations();

    return (<div className="border-b">
        <div className="flex h-16 items-center px-4">
          <div className="font-semibold">User Actions</div>
          <div className="ml-auto flex items-center space-x-4">
            <Select value={activeOrganization ?? undefined} onValueChange={setActiveOrganization}>
              <SelectTrigger className="w-[200px]">
                <SelectValue placeholder={isLoading ? "Loading..." : "Select organization"} />
              </SelectTrigger>
              <SelectContent>
                {isLoading && (
                  <SelectItem value="loading" disabled>Loading organizations...</SelectItem>
                )}
                {error && (
                  <SelectItem value="error" disabled>Error loading organizations</SelectItem>
                )}
                {organizations?.map((orgId) => (
                  <SelectItem key={orgId} value={orgId}>
                    {orgId}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        </div>
      </div>)
}

export default Navbar;