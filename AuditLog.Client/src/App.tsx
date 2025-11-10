import Navbar from "./components/navbar";
import UserActionsTable from "./components/UserActionsTable";
import { OrganizationsProvider } from "./contexts/OrganizationsContext";

const App = () => {
  return (
    <OrganizationsProvider>
      <Navbar />
      <main className="container mx-auto px-4 py-8">
        <UserActionsTable />
      </main>
    </OrganizationsProvider>
  );
}

export default App;
