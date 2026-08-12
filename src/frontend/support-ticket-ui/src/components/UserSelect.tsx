import type { User } from '../types';

interface Props {
  id: string;
  label: string;
  users: User[];
  value: number | '';
  onChange: (value: number | '') => void;
  required?: boolean;
}

export function UserSelect({ id, label, users, value, onChange, required }: Props) {
  return (
    <label className="field" htmlFor={id}>
      <span>{label}</span>
      <select
        id={id}
        value={value}
        required={required}
        onChange={(e) => onChange(e.target.value ? Number(e.target.value) : '')}
      >
        <option value="">Select a user</option>
        {users.map((user) => (
          <option key={user.id} value={user.id}>
            {user.name} ({user.role})
          </option>
        ))}
      </select>
    </label>
  );
}
