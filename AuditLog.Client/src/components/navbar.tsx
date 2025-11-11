import { useState } from "react";
import { Check, ChevronsUpDown } from "lucide-react";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from "@/components/ui/command";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { useOrganizations } from "@/contexts/OrganizationsContext";

const Navbar = () => {
    const { organizations, activeOrganization, setActiveOrganization, isLoading, error } = useOrganizations();
    const [open, setOpen] = useState(false);

    return (<div className="border-b">
        <div className="flex flex-wrap items-center gap-4 px-4 py-3 min-h-16">
          <div className="font-semibold">User Actions</div>
          <div className="w-full sm:w-auto sm:ml-auto flex items-center">
            <Popover open={open} onOpenChange={setOpen}>
              <PopoverTrigger asChild>
                <Button
                  variant="outline"
                  role="combobox"
                  aria-expanded={open}
                  className="w-full sm:w-[350px] justify-between"
                >
                  {activeOrganization || (isLoading ? "Loading..." : "Select organization...")}
                  <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                </Button>
              </PopoverTrigger>
              <PopoverContent className="w-[var(--radix-popover-trigger-width)] sm:w-[350px] p-0">
                <Command>
                  <CommandInput placeholder="Search organization..." />
                  <CommandList>
                    <CommandEmpty>
                      {error ? "Error loading organizations" : "No organization found."}
                    </CommandEmpty>
                    <CommandGroup>
                      {organizations?.map((orgId) => (
                        <CommandItem
                          key={orgId}
                          value={orgId}
                          onSelect={(currentValue) => {
                            setActiveOrganization(currentValue === activeOrganization ? "" : currentValue);
                            setOpen(false);
                          }}
                        >
                          <Check
                            className={cn(
                              "mr-2 h-4 w-4",
                              activeOrganization === orgId ? "opacity-100" : "opacity-0"
                            )}
                          />
                          {orgId}
                        </CommandItem>
                      ))}
                    </CommandGroup>
                  </CommandList>
                </Command>
              </PopoverContent>
            </Popover>
          </div>
        </div>
      </div>)
}

export default Navbar;