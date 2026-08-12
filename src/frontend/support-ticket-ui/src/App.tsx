import { NavLink, Route, Routes } from 'react-router-dom';
import { TicketListPage } from './pages/TicketListPage';
import { CreateTicketPage } from './pages/CreateTicketPage';
import { TicketDetailPage } from './pages/TicketDetailPage';
import './App.css';

function App() {
  return (
    <div className="app-shell">
      <header className="app-header">
        <div>
          <p className="brand">Support Ticket Management</p>
          <p className="tagline">Internal support workflow</p>
        </div>
        <nav>
          <NavLink to="/" end>
            Tickets
          </NavLink>
          <NavLink to="/tickets/new">Create</NavLink>
        </nav>
      </header>
      <main>
        <Routes>
          <Route path="/" element={<TicketListPage />} />
          <Route path="/tickets/new" element={<CreateTicketPage />} />
          <Route path="/tickets/:id" element={<TicketDetailPage />} />
        </Routes>
      </main>
    </div>
  );
}

export default App;
