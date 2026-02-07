import { describe, it, expect, vi, beforeEach } from 'vitest';
import { AxiosError } from 'axios';
import { parseApiError } from '../../services/api';

describe('API Service', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('parseApiError', () => {
    it('should parse error with response data', () => {
      const error = {
        response: {
          status: 400,
          data: {
            message: 'Bad Request',
            details: 'Invalid input',
          },
        },
        request: {},
        message: 'Request failed',
      } as AxiosError;

      const result = parseApiError(error);

      expect(result).toEqual({
        message: 'Bad Request',
        status: 400,
        details: 'Invalid input',
      });
    });

    it('should parse error with title instead of message', () => {
      const error = {
        response: {
          status: 404,
          data: {
            title: 'Not Found',
          },
        },
        request: {},
        message: 'Request failed',
      } as AxiosError;

      const result = parseApiError(error);

      expect(result).toEqual({
        message: 'Not Found',
        status: 404,
        details: '{"title":"Not Found"}',
      });
    });

    it('should handle error with no response', () => {
      const error = {
        request: {},
        message: 'Network Error',
      } as AxiosError;

      const result = parseApiError(error);

      expect(result).toEqual({
        message: 'No response from server. Please check your connection.',
        status: 0,
      });
    });

    it('should handle error with no request', () => {
      const error = {
        message: 'Request setup error',
      } as AxiosError;

      const result = parseApiError(error);

      expect(result).toEqual({
        message: 'Request setup error',
        status: 0,
      });
    });
  });
});
