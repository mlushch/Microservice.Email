import { describe, it, expect } from 'vitest';
import templateReducer, {
  TemplateState,
  clearError,
  setSelectedTemplate,
  fetchTemplates,
  deleteTemplate,
} from '../../store/templateSlice';
import { EmailTemplateResponse } from '../../types';

describe('Template Slice', () => {
  const initialState: TemplateState = {
    templates: [],
    loading: false,
    error: null,
    selectedTemplate: null,
  };

  const mockTemplates: EmailTemplateResponse[] = [
    { id: 1, name: 'Template 1', path: '/path1', size: 1024 },
    { id: 2, name: 'Template 2', path: '/path2', size: 2048 },
  ];

  describe('reducers', () => {
    it('should return initial state', () => {
      const result = templateReducer(undefined, { type: 'unknown' });
      expect(result).toEqual(initialState);
    });

    it('should handle clearError', () => {
      const stateWithError: TemplateState = {
        ...initialState,
        error: 'Some error',
      };
      
      const result = templateReducer(stateWithError, clearError());
      
      expect(result.error).toBeNull();
    });

    it('should handle setSelectedTemplate', () => {
      const template = mockTemplates[0];
      
      const result = templateReducer(initialState, setSelectedTemplate(template));
      
      expect(result.selectedTemplate).toEqual(template);
    });

    it('should handle setSelectedTemplate with null', () => {
      const stateWithSelection: TemplateState = {
        ...initialState,
        selectedTemplate: mockTemplates[0],
      };
      
      const result = templateReducer(stateWithSelection, setSelectedTemplate(null));
      
      expect(result.selectedTemplate).toBeNull();
    });
  });

  describe('fetchTemplates', () => {
    it('should set loading to true when pending', () => {
      const action = { type: fetchTemplates.pending.type };
      const result = templateReducer(initialState, action);
      
      expect(result.loading).toBe(true);
      expect(result.error).toBeNull();
    });

    it('should set templates and loading to false when fulfilled', () => {
      const action = { 
        type: fetchTemplates.fulfilled.type, 
        payload: mockTemplates 
      };
      const loadingState = { ...initialState, loading: true };
      
      const result = templateReducer(loadingState, action);
      
      expect(result.loading).toBe(false);
      expect(result.templates).toEqual(mockTemplates);
    });

    it('should set error and loading to false when rejected', () => {
      const action = { 
        type: fetchTemplates.rejected.type, 
        payload: 'Failed to fetch' 
      };
      const loadingState = { ...initialState, loading: true };
      
      const result = templateReducer(loadingState, action);
      
      expect(result.loading).toBe(false);
      expect(result.error).toBe('Failed to fetch');
    });
  });

  describe('deleteTemplate', () => {
    const stateWithTemplates: TemplateState = {
      ...initialState,
      templates: mockTemplates,
    };

    it('should set loading to true when pending', () => {
      const action = { type: deleteTemplate.pending.type };
      const result = templateReducer(stateWithTemplates, action);
      
      expect(result.loading).toBe(true);
      expect(result.error).toBeNull();
    });

    it('should remove template and set loading to false when fulfilled', () => {
      const action = { 
        type: deleteTemplate.fulfilled.type, 
        payload: 1 
      };
      const loadingState = { ...stateWithTemplates, loading: true };
      
      const result = templateReducer(loadingState, action);
      
      expect(result.loading).toBe(false);
      expect(result.templates).toHaveLength(1);
      expect(result.templates[0].id).toBe(2);
    });

    it('should set error and loading to false when rejected', () => {
      const action = { 
        type: deleteTemplate.rejected.type, 
        payload: 'Failed to delete' 
      };
      const loadingState = { ...stateWithTemplates, loading: true };
      
      const result = templateReducer(loadingState, action);
      
      expect(result.loading).toBe(false);
      expect(result.error).toBe('Failed to delete');
    });
  });
});
