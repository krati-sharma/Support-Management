import { useEffect, useState } from 'react';
import type { FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { formatApiError } from '../api/client';
import { createTicket, getUsers } from '../api/tickets';
import { UserSelect } from '../components/UserSelect';
import { ErrorAlert, LoadingSpinner } from '../components/Feedback';
import type { Priority, User } from '../types';

export function CreateTicketPage() {
  const navigate = useNavigate();
  const [users, setUsers] = useState<User[]>([]);
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [priority, setPriority] = useState<Priority>('Medium');
  const [createdById, setCreatedById] = useState<number | ''>('');
  const [assignedToId, setAssignedToId] = useState<number | ''>('');
  const [loadingUsers, setLoadingUsers] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');
  const [fieldErrors, setFieldErrors] = useState<string[]>([]);

  useEffect(() => {
    getUsers()
      .then(setUsers)
      .catch((err) => setError(formatApiError(err)))
      .finally(() => setLoadingUsers(false));
  }, []);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    const localErrors: string[] = [];
    if (!title.trim()) localErrors.push('Title is required.');
    if (!description.trim()) localErrors.push('Description is required.');
    if (!createdById) localErrors.push('Created by is required.');
    if (!assignedToId) localErrors.push('Assignee is required.');
    setFieldErrors(localErrors);
    if (localErrors.length > 0) return;

    setSubmitting(true);
    setError('');
    try {
      const ticket = await createTicket({
        title: title.trim(),
        description: description.trim(),
        priority,
        createdById: Number(createdById),
        assignedToId: Number(assignedToId),
      });
      navigate(`/tickets/${ticket.id}`);
    } catch (err) {
      setError(formatApiError(err));
    } finally {
      setSubmitting(false);
    }
  }

  if (loadingUsers) return <LoadingSpinner label="Loading users..." />;

  return (
    <section className="page">
      <div className="page-header">
        <div>
          <h1>Create ticket</h1>
          <p>Open a new support ticket.</p>
        </div>
        <Link className="button" to="/">
          Back to list
        </Link>
      </div>

      <ErrorAlert message={error} />
      {fieldErrors.length > 0 && (
        <div className="alert alert-error">
          {fieldErrors.map((msg) => (
            <div key={msg}>{msg}</div>
          ))}
        </div>
      )}

      <form className="form" onSubmit={handleSubmit}>
        <label className="field">
          <span>Title</span>
          <input value={title} onChange={(e) => setTitle(e.target.value)} maxLength={200} required />
        </label>

        <label className="field">
          <span>Description</span>
          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            rows={6}
            maxLength={4000}
            required
          />
        </label>

        <label className="field">
          <span>Priority</span>
          <select value={priority} onChange={(e) => setPriority(e.target.value as Priority)}>
            <option value="Low">Low</option>
            <option value="Medium">Medium</option>
            <option value="High">High</option>
          </select>
        </label>

        <UserSelect
          id="createdBy"
          label="Created by"
          users={users}
          value={createdById}
          onChange={setCreatedById}
          required
        />

        <UserSelect
          id="assignedTo"
          label="Assigned to"
          users={users}
          value={assignedToId}
          onChange={setAssignedToId}
          required
        />

        <button className="button primary" type="submit" disabled={submitting}>
          {submitting ? 'Creating...' : 'Create ticket'}
        </button>
      </form>
    </section>
  );
}
