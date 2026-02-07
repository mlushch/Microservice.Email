import '@testing-library/jest-dom';
import { vi } from 'vitest';

// Mock @mui/icons-material to avoid EMFILE errors on Windows
vi.mock('@mui/icons-material', () => ({
  Delete: () => null,
  Add: () => null,
  Refresh: () => null,
  CloudUpload: () => null,
  Email: () => null,
  List: () => null,
}));
