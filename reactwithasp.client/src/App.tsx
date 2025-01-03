import { useEffect, useState } from 'react';
import { Todo } from './types/Todo';
import { todoService } from './services/todoService';
import { TodoList } from './components/TodoList';
import { TodoForm } from './components/TodoForm';
import './App.css';

function App() {
    const [todos, setTodos] = useState<Todo[]>([]);
    const [newTodoTitle, setNewTodoTitle] = useState('');

    useEffect(() => {
        //call here, because calling in the body would cause a loop
        //fetchTodos sets state at the end, which causes a re-render, which would call fetchTodos again if it was in the body
        fetchTodos();
    }, []);

    const fetchTodos = async () => {
        try {
            const data = await todoService.fetchTodos();
            setTodos(data);
        } catch (error) {
            console.error('Error fetching todos:', error);
        }
    };

    const handleAddTodo = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!newTodoTitle.trim()) return;
        
        try {
            await todoService.addTodo(newTodoTitle);
            setNewTodoTitle('');
            fetchTodos();
        } catch (error) {
            console.error('Error adding todo:', error);
        }
    };

    const handleDeleteTodo = async (id: string) => {
        try {
            await todoService.deleteTodo(id);
            fetchTodos();
        } catch (error) {
            console.error('Error deleting todo:', error);
        }
    };

    return (
        <div className="container">
            <h1>Todo List</h1>
            <TodoForm
                value={newTodoTitle}
                onChange={setNewTodoTitle}
                onSubmit={handleAddTodo}
            />
            <TodoList 
                todos={todos}
                onDelete={handleDeleteTodo}
            />
        </div>
    );
}

export default App;