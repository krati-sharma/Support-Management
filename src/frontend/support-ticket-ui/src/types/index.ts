export type Priority = 'Low' | 'Medium' | 'High';
export type TicketStatus = 'Open' | 'InProgress' | 'Resolved' | 'Closed' | 'Cancelled';

export interface UserSummary {
  id: number;
  name: string;
}

export interface UserDetailSummary extends UserSummary {
  email: string;
}

export interface User {
  id: number;
  name: string;
  email: string;
  role: string;
}

export interface TicketListItem {
  id: number;
  title: string;
  description: string;
  priority: Priority;
  status: TicketStatus;
  assignedTo: UserSummary;
  createdBy: UserSummary;
  createdAt: string;
  updatedAt: string;
  commentCount: number;
}

export interface Comment {
  id: number;
  message: string;
  createdBy: UserSummary;
  createdAt: string;
}

export interface TicketDetail {
  id: number;
  title: string;
  description: string;
  priority: Priority;
  status: TicketStatus;
  assignedTo: UserDetailSummary;
  createdBy: UserDetailSummary;
  createdAt: string;
  updatedAt: string;
  comments: Comment[];
  validNextStatuses: TicketStatus[];
}

export interface CreateTicketPayload {
  title: string;
  description: string;
  priority: Priority;
  createdById: number;
  assignedToId: number;
}

export interface UpdateTicketPayload {
  title: string;
  description: string;
  priority: Priority;
  assignedToId: number;
}

export interface ApiError {
  title?: string;
  detail?: string;
  status?: number;
  errors?: Record<string, string[]>;
}
