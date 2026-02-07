/// <reference types="vitest" />

import { describe, it, expect, vi } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { TemplateForm } from '../../components/TemplateForm';

describe('TemplateForm', () => {
  const defaultProps = {
    onSubmit: vi.fn(),
  };

  it('should render form fields', () => {
    render(<TemplateForm {...defaultProps} />);
    
    expect(screen.getByTestId('template-name-input')).toBeInTheDocument();
    expect(screen.getByTestId('template-path-input')).toBeInTheDocument();
    expect(screen.getByTestId('upload-file-button')).toBeInTheDocument();
    expect(screen.getByTestId('submit-template-button')).toBeInTheDocument();
  });

  it('should show validation errors when submitting empty form', async () => {
    render(<TemplateForm {...defaultProps} />);
    
    fireEvent.click(screen.getByTestId('submit-template-button'));
    
    expect(await screen.findByText('Template name is required')).toBeInTheDocument();
    expect(screen.getByText('Path is required')).toBeInTheDocument();
    expect(screen.getByText('Template file is required')).toBeInTheDocument();
  });

  it('should update input values', async () => {
    const user = userEvent.setup();
    render(<TemplateForm {...defaultProps} />);
    
    const nameInput = screen.getByTestId('template-name-input');
    const pathInput = screen.getByTestId('template-path-input');
    
    await user.type(nameInput, 'Test Template');
    await user.type(pathInput, '/test/path');
    
    expect(nameInput).toHaveValue('Test Template');
    expect(pathInput).toHaveValue('/test/path');
  });

  it('should show loading state when isLoading is true', () => {
    render(<TemplateForm {...defaultProps} isLoading={true} />);
    
    expect(screen.getByTestId('template-name-input')).toBeDisabled();
    expect(screen.getByTestId('template-path-input')).toBeDisabled();
    expect(screen.getByTestId('upload-file-button')).toBeDisabled();
    expect(screen.getByTestId('submit-template-button')).toBeDisabled();
  });

  it('should handle file selection', async () => {
    render(<TemplateForm {...defaultProps} />);
    
    const file = new File(['content'], 'template.html', { type: 'text/html' });
    const fileInput = screen.getByTestId('template-file-input');
    
    await userEvent.upload(fileInput, file);
    
    expect(screen.getByText('template.html')).toBeInTheDocument();
  });

  it('should call onSubmit with form data when valid', async () => {
    const onSubmit = vi.fn();
    const user = userEvent.setup();
    render(<TemplateForm onSubmit={onSubmit} />);
    
    const file = new File(['content'], 'template.html', { type: 'text/html' });
    
    await user.type(screen.getByTestId('template-name-input'), 'Test Template');
    await user.type(screen.getByTestId('template-path-input'), '/test/path');
    await userEvent.upload(screen.getByTestId('template-file-input'), file);
    
    fireEvent.click(screen.getByTestId('submit-template-button'));
    
    expect(onSubmit).toHaveBeenCalledWith({
      name: 'Test Template',
      path: '/test/path',
      file: expect.any(File),
    });
  });
});
