import { describe, it, expect, vi, beforeEach } from 'vitest';

// Create shared mock functions using vi.hoisted to ensure they exist before mock factory runs
const { mockGet, mockPost, mockDelete } = vi.hoisted(() => ({
  mockGet: vi.fn(),
  mockPost: vi.fn(),
  mockDelete: vi.fn(),
}));

// Mock the apiClient module before importing templateService
vi.mock('../../services/api', () => {
  const apiClientMock = {
    get: mockGet,
    post: mockPost,
    delete: mockDelete,
  };
  return {
    apiClient: apiClientMock,
    default: apiClientMock,
    parseApiError: vi.fn(),
  };
});

// Import after mock setup
import { templateService } from '../../services/templateService';

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
      
      mockGet.mockResolvedValue({ data: mockTemplates });

      const result = await templateService.getAll();

      expect(mockGet).toHaveBeenCalledWith('/api/email-templates/all');
      expect(result).toEqual(mockTemplates);
    });

    it('should throw error when fetch fails', async () => {
      const error = new Error('Failed to fetch');
      mockGet.mockRejectedValue(error);

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

      mockPost.mockResolvedValue({});

      await templateService.create(request);

      expect(mockPost).toHaveBeenCalledWith(
        '/api/email-templates',
        expect.any(FormData),
        { headers: { 'Content-Type': 'multipart/form-data' } }
      );
    });
  });

  describe('delete', () => {
    it('should delete a template by ID', async () => {
      mockDelete.mockResolvedValue({});

      await templateService.delete(1);

      expect(mockDelete).toHaveBeenCalledWith('/api/email-templates/1');
    });

    it('should throw error when delete fails', async () => {
      const error = new Error('Failed to delete');
      mockDelete.mockRejectedValue(error);

      await expect(templateService.delete(1)).rejects.toThrow('Failed to delete');
    });
  });
});
