interface ScanFormProps {
    title: string;
    subject: string;
    description: string;
    onTitleChange: (value: string) => void;
    onSubjectChange: (value: string) => void;
    onDescriptionChange: (value: string) => void;
    onFileChange: (file: File | null) => void;
    onSubmit: (e: React.FormEvent) => void;
}

export function ScanForm({ 
    title, 
    subject,
    description,
    onTitleChange,
    onSubjectChange,
    onDescriptionChange,
    onFileChange,
    onSubmit 
}: ScanFormProps) {
    return (
        <form onSubmit={onSubmit} className="add-scan-form">
            <div className="form-group">
                <input
                    type="text"
                    value={title}
                    onChange={(e) => onTitleChange(e.target.value)}
                    placeholder="Enter title..."
                    className="form-input"
                />
                <input
                    type="text"
                    value={subject}
                    onChange={(e) => onSubjectChange(e.target.value)}
                    placeholder="Enter subject..."
                    className="form-input"
                />
                <textarea
                    value={description}
                    onChange={(e) => onDescriptionChange(e.target.value)}
                    placeholder="Enter description..."
                    className="form-textarea"
                />
                <input
                    type="file"
                    onChange={(e) => onFileChange(e.target.files ? e.target.files[0] : null)}
                    className="form-file-input"
                />
            </div>
            <button type="submit" className="submit-button">Add Scan</button>
        </form>
    );
} 