interface TodoFormProps {
    value: string;
    onChange: (value: string) => void;
    onSubmit: (e: React.FormEvent) => void;
}

export function TodoForm({ value, onChange, onSubmit }: TodoFormProps) {
    return (
        <form onSubmit={onSubmit} className="add-todo-form">
            <input
                type="text"
                value={value}
                onChange={(e) => onChange(e.target.value)}
                placeholder="Enter new todo..."
            />
            <button type="submit">Add Todo</button>
        </form>
    );
} 