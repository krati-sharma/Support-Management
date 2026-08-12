interface Props {
  message: string;
}

export function ErrorAlert({ message }: Props) {
  if (!message) return null;
  return <div className="alert alert-error" role="alert">{message}</div>;
}

export function LoadingSpinner({ label = 'Loading...' }: { label?: string }) {
  return <div className="loading">{label}</div>;
}

export function EmptyState({ message }: { message: string }) {
  return <div className="empty-state">{message}</div>;
}
