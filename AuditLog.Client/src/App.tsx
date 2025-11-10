import Navbar from "./components/navbar";
import { OrganizationsProvider } from "./contexts/OrganizationsContext";

const App = () => {
  return (
    <OrganizationsProvider>
      <Navbar />
    </OrganizationsProvider>
  );
}

export default App;
