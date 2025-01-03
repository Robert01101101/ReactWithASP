import { Todo } from '../types/Todo';

interface TodoListProps {
    todos: Todo[];
    onDelete: (id: string) => void;
}

export function TodoList({ todos, onDelete }: TodoListProps) {
    return (
        <ul className="todo-list">
            {todos.map(todo => (
                <li key={todo.id} className={todo.isComplete ? 'completed' : ''}>
                    <div className="todo-content">
                        <span>{todo.title}</span>
                        <small>{new Date(todo.createdAt).toLocaleDateString()}</small>
                    </div>
                    <button 
                        onClick={() => onDelete(todo.id)}
                        className="delete-button"
                    >
                        Delete
                    </button>
                </li>
            ))}
        </ul>
    );
} 