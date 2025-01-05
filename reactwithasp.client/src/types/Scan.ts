export interface Scan {
    id: string;
    title: string;
    subject: string;
    description: string;
    fileUrl: string | null; //nullable
    createdAt: string;
}
