import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { EmailTemplateResponse, CreateEmailTemplateRequest, ApiError } from '../types';
import { templateService } from '../services';

// State interface
export interface TemplateState {
  templates: EmailTemplateResponse[];
  loading: boolean;
  error: string | null;
  selectedTemplate: EmailTemplateResponse | null;
}

// Initial state
const initialState: TemplateState = {
  templates: [],
  loading: false,
  error: null,
  selectedTemplate: null,
};

// Async thunks
export const fetchTemplates = createAsyncThunk<
  EmailTemplateResponse[],
  void,
  { rejectValue: string }
>('templates/fetchAll', async (_, { rejectWithValue }) => {
  try {
    return await templateService.getAll();
  } catch (error) {
    const apiError = error as ApiError;
    return rejectWithValue(apiError.message || 'Failed to fetch templates');
  }
});

export const createTemplate = createAsyncThunk<
  void,
  CreateEmailTemplateRequest,
  { rejectValue: string }
>('templates/create', async (request, { rejectWithValue, dispatch }) => {
  try {
    await templateService.create(request);
    // Refresh templates list after creation
    dispatch(fetchTemplates());
  } catch (error) {
    const apiError = error as ApiError;
    return rejectWithValue(apiError.message || 'Failed to create template');
  }
});

export const deleteTemplate = createAsyncThunk<
  number,
  number,
  { rejectValue: string }
>('templates/delete', async (templateId, { rejectWithValue }) => {
  try {
    await templateService.delete(templateId);
    return templateId;
  } catch (error) {
    const apiError = error as ApiError;
    return rejectWithValue(apiError.message || 'Failed to delete template');
  }
});

// Slice
const templateSlice = createSlice({
  name: 'templates',
  initialState,
  reducers: {
    clearError: (state) => {
      state.error = null;
    },
    setSelectedTemplate: (state, action: PayloadAction<EmailTemplateResponse | null>) => {
      state.selectedTemplate = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder
      // Fetch templates
      .addCase(fetchTemplates.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchTemplates.fulfilled, (state, action) => {
        state.loading = false;
        state.templates = action.payload;
      })
      .addCase(fetchTemplates.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || 'Failed to fetch templates';
      })
      // Create template
      .addCase(createTemplate.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(createTemplate.fulfilled, (state) => {
        state.loading = false;
      })
      .addCase(createTemplate.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || 'Failed to create template';
      })
      // Delete template
      .addCase(deleteTemplate.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(deleteTemplate.fulfilled, (state, action) => {
        state.loading = false;
        state.templates = state.templates.filter((t) => t.id !== action.payload);
      })
      .addCase(deleteTemplate.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || 'Failed to delete template';
      });
  },
});

// Export actions
export const { clearError, setSelectedTemplate } = templateSlice.actions;

// Selectors
export const selectTemplates = (state: { templates: TemplateState }) => state.templates.templates;
export const selectTemplatesLoading = (state: { templates: TemplateState }) => state.templates.loading;
export const selectTemplatesError = (state: { templates: TemplateState }) => state.templates.error;
export const selectSelectedTemplate = (state: { templates: TemplateState }) => state.templates.selectedTemplate;

// Export reducer
export default templateSlice.reducer;
