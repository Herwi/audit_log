import Navbar from "./components/navbar";
import UserActions from "./components/UserActions";
import { OrganizationsProvider } from "./contexts/OrganizationsContext";

const App = () => {
  return (
    <OrganizationsProvider>
      <Navbar />
      <main className="container mx-auto px-4 py-8">
        <UserActions />
      </main>
    </OrganizationsProvider>
  );
}

export default App;
