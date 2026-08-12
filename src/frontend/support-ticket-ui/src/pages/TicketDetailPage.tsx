import { useEffect, useState } from 'react';
import type { FormEvent } from 'react';
import { Link, useParams } from 'react-router-dom';
import { formatApiError } from '../api/client';
import {
  addComment,
  changeTicketStatus,
  getTicket,
  getUsers,
  updateTicket,
} from '../api/tickets';
import { UserSelect } from '../components/UserSelect';
import { EmptyState, ErrorAlert, LoadingSpinner } from '../components/Feedback';
import type { Priority, TicketDetail, TicketStatus, User } from '../types';

export function TicketDetailPage() {
  const { id } = useParams();
  const ticketId = Number(id);

  const [ticket, setTicket] = useState<TicketDetail | null>(null);
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [priority, setPriority] = useState<Priority>('Medium');
  const [assignedToId, setAssignedToId] = useState<number | ''>('');
  const [nextStatus, setNextStatus] = useState<TicketStatus | ''>('');
  const [commentMessage, setCommentMessage] = useState('');
  const [commentAuthorId, setCommentAuthorId] = useState<number | ''>('');
  const [saving, setSaving] = useState(false);

  async function load() {
    setLoading(true);
    setError('');
    try {
      const [ticketData, userData] = await Promise.all([getTicket(ticketId), getUsers()]);
      setTicket(ticketData);
      setUsers(userData);
      setTitle(ticketData.title);
      setDescription(ticketData.description);
      setPriority(ticketData.priority);
      setAssignedToId(ticketData.assignedTo.id);
      setNextStatus(ticketData.validNextStatuses[0] ?? '');
      if (!commentAuthorId && userData.length > 0) {
        setCommentAuthorId(userData[0].id);
      }
    } catch (err) {
      setError(formatApiError(err));
      setTicket(null);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    if (!Number.isFinite(ticketId)) {
      setError('Invalid ticket id.');
      setLoading(false);
      return;
    }
    void load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [ticketId]);

  async function handleUpdate(event: FormEvent) {
    event.preventDefault();
    if (!assignedToId) {
      setError('Assignee is required.');
      return;
    }

    setSaving(true);
    setError('');
    setSuccess('');
    try {
      const updated = await updateTicket(ticketId, {
        title: title.trim(),
        description: description.trim(),
        priority,
        assignedToId: Number(assignedToId),
      });
      setTicket(updated);
      setSuccess('Ticket updated successfully.');
    } catch (err) {
      setError(formatApiError(err));
    } finally {
      setSaving(false);
    }
  }

  async function handleStatusChange(event: FormEvent) {
    event.preventDefault();
    if (!nextStatus) {
      setError('Select a valid next status.');
      return;
    }

    setSaving(true);
    setError('');
    setSuccess('');
    try {
      const updated = await changeTicketStatus(ticketId, nextStatus);
      setTicket(updated);
      setTitle(updated.title);
      setDescription(updated.description);
      setPriority(updated.priority);
      setAssignedToId(updated.assignedTo.id);
      setNextStatus(updated.validNextStatuses[0] ?? '');
      setSuccess(`Status changed to ${updated.status}.`);
    } catch (err) {
      setError(formatApiError(err));
    } finally {
      setSaving(false);
    }
  }

  async function handleAddComment(event: FormEvent) {
    event.preventDefault();
    if (!commentMessage.trim() || !commentAuthorId) {
      setError('Comment message and author are required.');
      return;
    }

    setSaving(true);
    setError('');
    setSuccess('');
    try {
      await addComment(ticketId, commentMessage.trim(), Number(commentAuthorId));
      setCommentMessage('');
      await load();
      setSuccess('Comment added.');
    } catch (err) {
      setError(formatApiError(err));
    } finally {
      setSaving(false);
    }
  }

  if (loading) return <LoadingSpinner label="Loading ticket..." />;
  if (!ticket) {
    return (
      <section className="page">
        <ErrorAlert message={error || 'Ticket not found.'} />
        <Link className="button" to="/">
          Back to list
        </Link>
      </section>
    );
  }

  return (
    <section className="page">
      <div className="page-header">
        <div>
          <h1>{ticket.title}</h1>
          <p>
            Status: <span className={`badge status-${ticket.status.toLowerCase()}`}>{ticket.status}</span> · Created by{' '}
            {ticket.createdBy.name}
          </p>
        </div>
        <Link className="button" to="/">
          Back to list
        </Link>
      </div>

      <ErrorAlert message={error} />
      {success && <div className="alert alert-success">{success}</div>}

      <div className="detail-grid">
        <form className="form panel" onSubmit={handleUpdate}>
          <h2>Update ticket</h2>
          <label className="field">
            <span>Title</span>
            <input value={title} onChange={(e) => setTitle(e.target.value)} maxLength={200} required />
          </label>
          <label className="field">
            <span>Description</span>
            <textarea value={description} onChange={(e) => setDescription(e.target.value)} rows={5} required />
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
            id="assignee"
            label="Assigned to"
            users={users}
            value={assignedToId}
            onChange={setAssignedToId}
            required
          />
          <button className="button primary" type="submit" disabled={saving}>
            Save changes
          </button>
        </form>

        <form className="form panel" onSubmit={handleStatusChange}>
          <h2>Change status</h2>
          <p className="muted">Backend enforces allowed transitions.</p>
          {ticket.validNextStatuses.length === 0 ? (
            <EmptyState message="This ticket is in a terminal state. No further transitions are allowed." />
          ) : (
            <>
              <label className="field">
                <span>Next status</span>
                <select
                  value={nextStatus}
                  onChange={(e) => setNextStatus(e.target.value as TicketStatus)}
                  required
                >
                  {ticket.validNextStatuses.map((status) => (
                    <option key={status} value={status}>
                      {status}
                    </option>
                  ))}
                </select>
              </label>
              <button className="button primary" type="submit" disabled={saving}>
                Apply transition
              </button>
            </>
          )}
        </form>
      </div>

      <div className="panel">
        <h2>Comments</h2>
        {ticket.comments.length === 0 ? (
          <EmptyState message="No comments yet." />
        ) : (
          <ul className="comment-list">
            {ticket.comments.map((comment) => (
              <li key={comment.id}>
                <div className="comment-meta">
                  <strong>{comment.createdBy.name}</strong>
                  <span>{new Date(comment.createdAt).toLocaleString()}</span>
                </div>
                <p>{comment.message}</p>
              </li>
            ))}
          </ul>
        )}

        <form className="form" onSubmit={handleAddComment}>
          <label className="field">
            <span>New comment</span>
            <textarea
              value={commentMessage}
              onChange={(e) => setCommentMessage(e.target.value)}
              rows={3}
              maxLength={2000}
              required
            />
          </label>
          <UserSelect
            id="commentAuthor"
            label="Comment author"
            users={users}
            value={commentAuthorId}
            onChange={setCommentAuthorId}
            required
          />
          <button className="button primary" type="submit" disabled={saving}>
            Add comment
          </button>
        </form>
      </div>
    </section>
  );
}
