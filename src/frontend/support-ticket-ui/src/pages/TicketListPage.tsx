import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { formatApiError } from '../api/client';
import { getTickets } from '../api/tickets';
import type { TicketListItem, TicketStatus } from '../types';
import { EmptyState, ErrorAlert, LoadingSpinner } from '../components/Feedback';

const STATUS_OPTIONS: Array<TicketStatus | ''> = ['', 'Open', 'InProgress', 'Resolved', 'Closed', 'Cancelled'];

export function TicketListPage() {
  const [tickets, setTickets] = useState<TicketListItem[]>([]);
  const [search, setSearch] = useState('');
  const [status, setStatus] = useState<TicketStatus | ''>('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    let cancelled = false;

    async function load() {
      setLoading(true);
      setError('');
      try {
        const data = await getTickets(search, status || undefined);
        if (!cancelled) setTickets(data);
      } catch (err) {
        if (!cancelled) setError(formatApiError(err));
      } finally {
        if (!cancelled) setLoading(false);
      }
    }

    const timer = window.setTimeout(load, 250);
    return () => {
      cancelled = true;
      window.clearTimeout(timer);
    };
  }, [search, status]);

  return (
    <section className="page">
      <div className="page-header">
        <div>
          <h1>Tickets</h1>
          <p>Search, filter, and open support tickets.</p>
        </div>
        <Link className="button primary" to="/tickets/new">
          Create ticket
        </Link>
      </div>

      <div className="toolbar">
        <input
          type="search"
          placeholder="Search title or description"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          aria-label="Keyword search"
        />
        <select
          value={status}
          onChange={(e) => setStatus(e.target.value as TicketStatus | '')}
          aria-label="Status filter"
        >
          {STATUS_OPTIONS.map((option) => (
            <option key={option || 'all'} value={option}>
              {option || 'All statuses'}
            </option>
          ))}
        </select>
      </div>

      <ErrorAlert message={error} />

      {loading ? (
        <LoadingSpinner label="Loading tickets..." />
      ) : tickets.length === 0 ? (
        <EmptyState message="No tickets found. Try a different search or create a new ticket." />
      ) : (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Title</th>
                <th>Status</th>
                <th>Priority</th>
                <th>Assignee</th>
                <th>Updated</th>
                <th>Comments</th>
              </tr>
            </thead>
            <tbody>
              {tickets.map((ticket) => (
                <tr key={ticket.id}>
                  <td>
                    <Link to={`/tickets/${ticket.id}`}>{ticket.title}</Link>
                  </td>
                  <td>
                    <span className={`badge status-${ticket.status.toLowerCase()}`}>{ticket.status}</span>
                  </td>
                  <td>{ticket.priority}</td>
                  <td>{ticket.assignedTo.name}</td>
                  <td>{new Date(ticket.updatedAt).toLocaleString()}</td>
                  <td>{ticket.commentCount}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  );
}
