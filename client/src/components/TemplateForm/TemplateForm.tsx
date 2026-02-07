import { useState, useRef, FormEvent } from 'react';
import {
  Box,
  TextField,
  Button,
  Typography,
  Paper,
  CircularProgress,
} from '@mui/material';
import { CloudUpload as CloudUploadIcon } from '@mui/icons-material';

interface TemplateFormData {
  name: string;
  path: string;
  file: File | null;
}

interface TemplateFormProps {
  onSubmit: (data: { name: string; path: string; file: File }) => void;
  isLoading?: boolean;
}

export const TemplateForm = ({ onSubmit, isLoading = false }: TemplateFormProps) => {
  const [formData, setFormData] = useState<TemplateFormData>({
    name: '',
    path: '',
    file: null,
  });
  const [errors, setErrors] = useState<Partial<Record<keyof TemplateFormData, string>>>({});
  const fileInputRef = useRef<HTMLInputElement>(null);

  const validate = (): boolean => {
    const newErrors: Partial<Record<keyof TemplateFormData, string>> = {};

    if (!formData.name.trim()) {
      newErrors.name = 'Template name is required';
    }

    if (!formData.path.trim()) {
      newErrors.path = 'Path is required';
    }

    if (!formData.file) {
      newErrors.file = 'Template file is required';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();

    if (!validate()) return;

    if (formData.file) {
      onSubmit({
        name: formData.name.trim(),
        path: formData.path.trim(),
        file: formData.file,
      });
    }
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0] || null;
    setFormData((prev) => ({ ...prev, file }));
    if (file) {
      setErrors((prev) => ({ ...prev, file: undefined }));
    }
  };

  const handleFileButtonClick = () => {
    fileInputRef.current?.click();
  };

  return (
    <Paper elevation={2} sx={{ p: 3 }}>
      <Typography variant="h6" gutterBottom>
        Create New Template
      </Typography>
      <Box component="form" onSubmit={handleSubmit} noValidate>
        <TextField
          fullWidth
          label="Template Name"
          value={formData.name}
          onChange={(e) => setFormData((prev) => ({ ...prev, name: e.target.value }))}
          error={!!errors.name}
          helperText={errors.name}
          margin="normal"
          disabled={isLoading}
          required
          inputProps={{ 'data-testid': 'template-name-input' }}
        />

        <TextField
          fullWidth
          label="Path"
          value={formData.path}
          onChange={(e) => setFormData((prev) => ({ ...prev, path: e.target.value }))}
          error={!!errors.path}
          helperText={errors.path || 'Storage path for the template file'}
          margin="normal"
          disabled={isLoading}
          required
          inputProps={{ 'data-testid': 'template-path-input' }}
        />

        <Box sx={{ mt: 2, mb: 1 }}>
          <input
            type="file"
            ref={fileInputRef}
            onChange={handleFileChange}
            style={{ display: 'none' }}
            accept=".html,.htm,.txt"
            data-testid="template-file-input"
          />
          <Button
            variant="outlined"
            startIcon={<CloudUploadIcon />}
            onClick={handleFileButtonClick}
            disabled={isLoading}
            data-testid="upload-file-button"
          >
            {formData.file ? formData.file.name : 'Choose Template File'}
          </Button>
          {errors.file && (
            <Typography color="error" variant="caption" display="block" sx={{ mt: 0.5 }}>
              {errors.file}
            </Typography>
          )}
        </Box>

        <Box sx={{ mt: 3 }}>
          <Button
            type="submit"
            variant="contained"
            disabled={isLoading}
            data-testid="submit-template-button"
          >
            {isLoading ? <CircularProgress size={24} /> : 'Create Template'}
          </Button>
        </Box>
      </Box>
    </Paper>
  );
};

export default TemplateForm;
