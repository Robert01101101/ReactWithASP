import { useEffect, useState } from 'react';
import './App.css';

interface Todo {
    id: string;
    title: string;
    isComplete: boolean;
    createdAt: string;
}

function App() {
    const [todos, setTodos] = useState<Todo[]>([]);
    const [newTodoTitle, setNewTodoTitle] = useState('');

    useEffect(() => {
        fetchTodos();
    }, []);

    const fetchTodos = async () => {
        console.log('Fetching todos...');
        try {
            const response = await fetch('/api/todo');
            console.log('Fetch response status:', response.status);
            if (response.ok) {
                const data = await response.json();
                console.log('Fetched todos:', data);
                setTodos(data);
            } else {
                console.error('Failed to fetch todos:', response.statusText);
            }
        } catch (error) {
            console.error('Error fetching todos:', error);
        }
    };

    const addTodo = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!newTodoTitle.trim()) return;
        
        console.log('Attempting to add todo:', { title: newTodoTitle });
        
        try {
            const response = await fetch('/api/todo', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    title: newTodoTitle,
                    isComplete: false
                })
            });
            
            console.log('Response status:', response.status);
            
            if (response.ok) {
                const responseData = await response.json();
                console.log('Response data:', responseData);
                setNewTodoTitle('');
                fetchTodos();
            } else {
                console.error('Failed to add todo:', response.statusText);
                const errorText = await response.text();
                console.error('Error details:', errorText);
            }
        } catch (error) {
            console.error('Error adding todo:', error);
        }
    };

    return (
        <div className="container">
            <h1>Todo List</h1>
            
            <form onSubmit={addTodo} className="add-todo-form">
                <input
                    type="text"
                    value={newTodoTitle}
                    onChange={(e) => setNewTodoTitle(e.target.value)}
                    placeholder="Enter new todo..."
                />
                <button type="submit">Add Todo</button>
            </form>

            <ul className="todo-list">
                {todos.map(todo => (
                    <li key={todo.id} className={todo.isComplete ? 'completed' : ''}>
                        <span>{todo.title}</span>
                        <small>{new Date(todo.createdAt).toLocaleDateString()}</small>
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default App;