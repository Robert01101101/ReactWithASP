import { useEffect, useState } from 'react';
import { Todo } from './types/Todo';
import { Scan } from './types/Scan';
import { todoService } from './services/todoService';
import { scanService } from './services/scanService';
import { TodoList } from './components/TodoList';
import { TodoForm } from './components/TodoForm';
import { ScanList } from './components/ScanList';
import { ScanForm } from './components/ScanForm';
import './App.css';

function App() {
    // Todo state
    const [todos, setTodos] = useState<Todo[]>([]);
    const [newTodoTitle, setNewTodoTitle] = useState('');

    // Scan state
    const [scans, setScans] = useState<Scan[]>([]);
    const [newScan, setNewScan] = useState({
        title: '',
        subject: '',
        description: ''
    });
    const [selectedFiles, setSelectedFiles] = useState<FileList | null>(null);

    useEffect(() => {
        fetchTodos();
        fetchScans();
    }, []);

    // Todo handlers
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

    // Scan handlers
    const fetchScans = async () => {
        try {
            const data = await scanService.fetchScans();
            setScans(data);
        } catch (error) {
            console.error('Error fetching scans:', error);
        }
    };

    const handleAddScan = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!newScan.title.trim()) return;

        try {
            const formData = new FormData();
            formData.append('title', newScan.title);
            formData.append('subject', newScan.subject);
            formData.append('description', newScan.description);
            
            // Add all selected files to the FormData
            if (selectedFiles) {
                for (let i = 0; i < selectedFiles.length; i++) {
                    formData.append('files', selectedFiles[i]);
                }
            }

            await scanService.addScan(formData);
            setNewScan({ title: '', subject: '', description: '' });
            setSelectedFiles(null);
            fetchScans();
        } catch (error) {
            console.error('Error adding scan:', error);
        }
    };

    const handleDeleteScan = async (id: string) => {
        try {
            await scanService.deleteScan(id);
            fetchScans();
        } catch (error) {
            console.error('Error deleting scan:', error);
        }
    };

    return (
        <div className="app-container">
            <div className="modules-wrapper">
                <div className="module form-module">
                    <h2>New Todo</h2>
                    <TodoForm
                        value={newTodoTitle}
                        onChange={setNewTodoTitle}
                        onSubmit={handleAddTodo}
                    />
                </div>
                <div className="module form-module">
                    <h2>New Scan</h2>
                    <ScanForm
                        title={newScan.title}
                        subject={newScan.subject}
                        description={newScan.description}
                        onTitleChange={(value) => setNewScan({ ...newScan, title: value })}
                        onSubjectChange={(value) => setNewScan({ ...newScan, subject: value })}
                        onDescriptionChange={(value) => setNewScan({ ...newScan, description: value })}
                        onFilesChange={(files) => setSelectedFiles(files)}
                        onSubmit={handleAddScan}
                    />
                </div>
                <div className="module list-module">
                    <h2>Todo List</h2>
                    <TodoList 
                        todos={todos}
                        onDelete={handleDeleteTodo}
                    />
                </div>
                <div className="module list-module">
                    <h2>Scan List</h2>
                    <ScanList 
                        scans={scans}
                        onDelete={handleDeleteScan}
                    />
                </div>
            </div>
        </div>
    );
}

export default App;