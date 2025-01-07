export interface Scan {
    id: string;
    title: string;
    subject: string;
    description: string;
    blobUrl: string | null;
    originalFileName: string | null;
    createdAt: string;
}
