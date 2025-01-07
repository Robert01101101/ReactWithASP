interface ScanFormProps {
    title: string;
    subject: string;
    description: string;
    onTitleChange: (value: string) => void;
    onSubjectChange: (value: string) => void;
    onDescriptionChange: (value: string) => void;
    onFilesChange: (files: FileList | null) => void;
    onSubmit: (e: React.FormEvent) => void;
}

export function ScanForm({ 
    title,
    subject,
    description,
    onTitleChange,
    onSubjectChange,
    onDescriptionChange,
    onFilesChange,
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
                    required
                />
                <input
                    type="text"
                    value={subject}
                    onChange={(e) => onSubjectChange(e.target.value)}
                    placeholder="Enter subject..."
                    className="form-input"
                    required
                />
                <textarea
                    value={description}
                    onChange={(e) => onDescriptionChange(e.target.value)}
                    placeholder="Enter description..."
                    className="form-input form-textarea"
                />
                <div className="file-upload-section">
                    <input
                        type="file"
                        onChange={(e) => onFilesChange(e.target.files)}
                        multiple
                        accept=".obj,.mtl,.png,.jpg,.jpeg"
                        className="form-file-input"
                        required
                    />
                    <small className="file-help-text">
                        Upload OBJ file (required), MTL file and texture files (optional)
                    </small>
                </div>
            </div>
            <button type="submit" className="submit-button">Upload Model</button>
        </form>
    );
} 