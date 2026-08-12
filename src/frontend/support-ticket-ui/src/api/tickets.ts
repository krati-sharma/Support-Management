import { apiRequest } from './client';
import type {
  Comment,
  CreateTicketPayload,
  TicketDetail,
  TicketListItem,
  TicketStatus,
  UpdateTicketPayload,
  User,
} from '../types';

export function getUsers(): Promise<User[]> {
  return apiRequest<User[]>('/api/users');
}

export function getTickets(search?: string, status?: string): Promise<TicketListItem[]> {
  const params = new URLSearchParams();
  if (search?.trim()) params.set('search', search.trim());
  if (status) params.set('status', status);
  const query = params.toString();
  return apiRequest<TicketListItem[]>(`/api/tickets${query ? `?${query}` : ''}`);
}

export function getTicket(id: number): Promise<TicketDetail> {
  return apiRequest<TicketDetail>(`/api/tickets/${id}`);
}

export function createTicket(payload: CreateTicketPayload): Promise<TicketDetail> {
  return apiRequest<TicketDetail>('/api/tickets', {
    method: 'POST',
    body: JSON.stringify(payload),
  });
}

export function updateTicket(id: number, payload: UpdateTicketPayload): Promise<TicketDetail> {
  return apiRequest<TicketDetail>(`/api/tickets/${id}`, {
    method: 'PUT',
    body: JSON.stringify(payload),
  });
}

export function changeTicketStatus(id: number, status: TicketStatus): Promise<TicketDetail> {
  return apiRequest<TicketDetail>(`/api/tickets/${id}/status`, {
    method: 'POST',
    body: JSON.stringify({ status }),
  });
}

export function addComment(ticketId: number, message: string, createdById: number): Promise<Comment> {
  return apiRequest<Comment>(`/api/tickets/${ticketId}/comments`, {
    method: 'POST',
    body: JSON.stringify({ message, createdById }),
  });
}
