import { Scan } from '../types/Scan';

interface ScanListProps {
    scans: Scan[];
    onDelete: (id: string) => void;
}

export function ScanList({ scans, onDelete }: ScanListProps) {
    return (
        <div className="scan-list">
            {scans.map(scan => (
                <div key={scan.id} className="scan-item">
                    <div className="column">
                        <h3>{scan.title}</h3>
                        <small>Subject: {scan.subject}</small>
                    </div>
                    <p>{scan.description}</p>
                    {scan.blobUrl && (
                        <a href={scan.blobUrl} target="_blank" rel="noopener noreferrer">
                            Download Model Files
                        </a>
                    )}
                    <button onClick={() => onDelete(scan.id)} className="delete-button">Delete</button>
                </div>
            ))}
        </div>
    );
} 