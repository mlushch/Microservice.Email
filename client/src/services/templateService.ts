import apiClient from './api';
import { EmailTemplateResponse, CreateEmailTemplateRequest } from '../types';

const TEMPLATES_BASE_URL = '/api/email-templates';

export const templateService = {
  /**
   * Fetch all email templates
   */
  getAll: async (): Promise<EmailTemplateResponse[]> => {
    const response = await apiClient.get<EmailTemplateResponse[]>(`${TEMPLATES_BASE_URL}/all`);
    return response.data;
  },

  /**
   * Create a new email template
   */
  create: async (request: CreateEmailTemplateRequest): Promise<void> => {
    const formData = new FormData();
    formData.append('name', request.name);
    formData.append('path', request.path);
    formData.append('file', request.file);

    await apiClient.post(TEMPLATES_BASE_URL, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
  },

  /**
   * Delete an email template by ID
   */
  delete: async (templateId: number): Promise<void> => {
    await apiClient.delete(`${TEMPLATES_BASE_URL}/${templateId}`);
  },
};

export default templateService;
