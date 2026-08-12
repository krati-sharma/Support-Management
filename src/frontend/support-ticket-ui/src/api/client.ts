import type { ApiError } from '../types';

export class ApiRequestError extends Error {
  status: number;
  details?: ApiError;

  constructor(message: string, status: number, details?: ApiError) {
    super(message);
    this.name = 'ApiRequestError';
    this.status = status;
    this.details = details;
  }
}

export function formatApiError(error: unknown): string {
  if (error instanceof ApiRequestError) {
    if (error.details?.errors) {
      return Object.entries(error.details.errors)
        .map(([field, messages]) => `${field}: ${messages.join(', ')}`)
        .join(' | ');
    }
    return error.details?.detail || error.details?.title || error.message;
  }

  if (error instanceof Error) {
    return error.message;
  }

  return 'An unexpected error occurred.';
}

async function parseError(response: Response): Promise<ApiRequestError> {
  let details: ApiError | undefined;
  try {
    details = (await response.json()) as ApiError;
  } catch {
    details = undefined;
  }

  const message = details?.detail || details?.title || `Request failed with status ${response.status}`;
  return new ApiRequestError(message, response.status, details);
}

export async function apiRequest<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(path, {
    headers: {
      'Content-Type': 'application/json',
      ...(init?.headers ?? {}),
    },
    ...init,
  });

  if (!response.ok) {
    throw await parseError(response);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}
