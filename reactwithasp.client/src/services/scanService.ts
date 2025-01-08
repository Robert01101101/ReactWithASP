import { Scan } from '../types/Scan';

export const scanService = {
    async fetchScans(): Promise<Scan[]> {
        const response = await fetch('/api/scan');
        if (!response.ok) {
            throw new Error('Failed to fetch scans');
        }
        return response.json();
    },

    async addScan(formData: FormData): Promise<Scan> {
        const response = await fetch('/api/scan', {
            method: 'POST',
            body: formData // FormData automatically sets the correct Content-Type
        });
        
        if (!response.ok) {
            throw new Error('Failed to add scan');
        }
        return response.json();
    },

    async deleteScan(id: string): Promise<void> {
        const response = await fetch(`/api/scan/${id}`, {
            method: 'DELETE'
        });
        
        if (!response.ok) {
            throw new Error('Failed to delete scan');
        }
    },

    async downloadScan(id: string): Promise<Blob> {
        const response = await fetch(`/api/scan/${id}/download`);
        if (!response.ok) {
            throw new Error('Failed to download scan');
        }
        return response.blob();
    }
}; 