interface TodoFormProps {
    value: string;
    onChange: (value: string) => void;
    onSubmit: (e: React.FormEvent) => void;
}

export function TodoForm({ value, onChange, onSubmit }: TodoFormProps) {
    return (
        <form onSubmit={onSubmit} className="add-todo-form">
            <textarea
                value={value}
                onChange={(e) => onChange(e.target.value)}
                placeholder="Enter new todo..."
                className="form-input form-textarea"
            />
            <button type="submit">Add Todo</button>
        </form>
    );
} 