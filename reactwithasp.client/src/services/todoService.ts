import { Todo } from '../types/Todo';

export const todoService = {
    async fetchTodos(): Promise<Todo[]> {
        const response = await fetch('/api/todo');
        if (!response.ok) {
            throw new Error('Failed to fetch todos');
        }
        return response.json();
    },

    async addTodo(title: string): Promise<Todo> {
        const response = await fetch('/api/todo', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                title,
                isComplete: false
            })
        });
        
        if (!response.ok) {
            throw new Error('Failed to add todo');
        }
        return response.json();
    },

    async deleteTodo(id: string): Promise<void> {
        const response = await fetch(`/api/todo/${id}`, {
            method: 'DELETE'
        });
        
        if (!response.ok) {
            throw new Error('Failed to delete todo');
        }
    }
}; 