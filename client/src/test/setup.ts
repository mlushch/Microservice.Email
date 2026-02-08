import '@testing-library/jest-dom';
import { vi } from 'vitest';

// Mock axios to prevent real network requests during tests
vi.mock('axios', () => {
  const mockAxiosInstance = {
    get: vi.fn(),
    post: vi.fn(),
    put: vi.fn(),
    delete: vi.fn(),
    patch: vi.fn(),
    interceptors: {
      request: { use: vi.fn() },
      response: { use: vi.fn() },
    },
  };
  return {
    default: {
      create: vi.fn(() => mockAxiosInstance),
      ...mockAxiosInstance,
    },
    AxiosError: class AxiosError extends Error {
      response?: unknown;
      request?: unknown;
      constructor(message?: string) {
        super(message);
        this.name = 'AxiosError';
      }
    },
  };
});

// Mock @mui/icons-material to avoid EMFILE errors on Windows
vi.mock('@mui/icons-material', () => ({
  Delete: () => null,
  Add: () => null,
  Refresh: () => null,
  CloudUpload: () => null,
  Email: () => null,
  List: () => null,
}));
