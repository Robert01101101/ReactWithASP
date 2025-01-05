import { Scan } from '../types/Scan';

interface ScanListProps {
    scans: Scan[];
    onDelete: (id: string) => void;
}

export function ScanList({ scans, onDelete }: ScanListProps) {
    return (
        <ul className="scan-list">
            {scans.map(scan => (
                <li key={scan.id}>
                    <div className="scan-content">
                        <h3>{scan.title}</h3>
                        <p className="subject">{scan.subject}</p>
                        <p className="description">{scan.description}</p>
                        {scan.fileUrl && (
                            <a href={scan.fileUrl} target="_blank" rel="noopener noreferrer">
                                View File
                            </a>
                        )}
                        <small>{new Date(scan.createdAt).toLocaleDateString()}</small>
                    </div>
                    <button 
                        onClick={() => onDelete(scan.id)}
                        className="delete-button"
                    >
                        Delete
                    </button>
                </li>
            ))}
        </ul>
    );
} 