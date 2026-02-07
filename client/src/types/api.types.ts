// API Types matching backend contracts

export interface EmailSender {
  name?: string;
  email: string;
}

export interface EmailRecipient {
  id: number;
  email: string;
}

export interface EmailStatus {
  id: number;
  name: string;
}

export interface SendEmailRequest {
  sender: EmailSender;
  recipients: string[];
  subject: string;
  body: string;
}

export interface SendTemplatedEmailRequest {
  sender: EmailSender;
  recipients: string[];
  templateName: string;
  templateProperties: Record<string, unknown>;
}

export interface AttachmentInfo {
  fileName: string;
}

export interface AttachmentsWrapper<T> {
  email: T;
  attachments: AttachmentInfo[];
}

export interface EmailResponse {
  id: number;
  sender: EmailSender;
  recipients: EmailRecipient[];
  sentDate: string;
  emailStatus: EmailStatus;
}

export interface EmailTemplateResponse {
  id: number;
  name: string;
  path: string;
  size: number;
}

export interface CreateEmailTemplateRequest {
  name: string;
  path: string;
  file: File;
}

// API Error Response
export interface ApiError {
  message: string;
  status: number;
  details?: string;
}
