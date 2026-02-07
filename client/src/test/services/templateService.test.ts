import { describe, it, expect, vi, beforeEach } from 'vitest';

// Mock the apiClient module before importing templateService
vi.mock('../../services/api', () => ({
  apiClient: {
    get: vi.fn(),
    post: vi.fn(),
    delete: vi.fn(),
  },
  default: {
    get: vi.fn(),
    post: vi.fn(),
    delete: vi.fn(),
  },
  parseApiError: vi.fn(),
}));

// Import after mock setup
import { templateService } from '../../services/templateService';
import apiClient from '../../services/api';

describe('Template Service', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('getAll', () => {
    it('should fetch all templates', async () => {
      const mockTemplates = [
        { id: 1, name: 'Template 1', path: '/path1', size: 1024 },
        { id: 2, name: 'Template 2', path: '/path2', size: 2048 },
      ];
      
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockTemplates });

      const result = await templateService.getAll();

      expect(apiClient.get).toHaveBeenCalledWith('/api/email-templates/all');
      expect(result).toEqual(mockTemplates);
    });

    it('should throw error when fetch fails', async () => {
      const error = new Error('Failed to fetch');
      vi.mocked(apiClient.get).mockRejectedValue(error);

      await expect(templateService.getAll()).rejects.toThrow('Failed to fetch');
    });
  });

  describe('create', () => {
    it('should create a new template', async () => {
      const file = new File(['content'], 'template.html', { type: 'text/html' });
      const request = {
        name: 'Test Template',
        path: '/test/path',
        file,
      };

      vi.mocked(apiClient.post).mockResolvedValue({});

      await templateService.create(request);

      expect(apiClient.post).toHaveBeenCalledWith(
        '/api/email-templates',
        expect.any(FormData),
        { headers: { 'Content-Type': 'multipart/form-data' } }
      );
    });
  });

  describe('delete', () => {
    it('should delete a template by ID', async () => {
      vi.mocked(apiClient.delete).mockResolvedValue({});

      await templateService.delete(1);

      expect(apiClient.delete).toHaveBeenCalledWith('/api/email-templates/1');
    });

    it('should throw error when delete fails', async () => {
      const error = new Error('Failed to delete');
      vi.mocked(apiClient.delete).mockRejectedValue(error);

      await expect(templateService.delete(1)).rejects.toThrow('Failed to delete');
    });
  });
});
