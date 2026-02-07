/// <reference types="vitest" />

import { describe, it, expect, vi } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import { TemplateTable } from '../../components/TemplateTable';
import { EmailTemplateResponse } from '../../types';

describe('TemplateTable', () => {
  const mockTemplates: EmailTemplateResponse[] = [
    { id: 1, name: 'Template 1', path: '/path1', size: 1024 },
    { id: 2, name: 'Template 2', path: '/path2', size: 2048 },
    { id: 3, name: 'Template 3', path: '/path3', size: 1048576 },
  ];

  const defaultProps = {
    templates: mockTemplates,
    onDelete: vi.fn(),
  };

  it('should render table with templates', () => {
    render(<TemplateTable {...defaultProps} />);
    
    expect(screen.getByTestId('template-table')).toBeInTheDocument();
    expect(screen.getByText('Template 1')).toBeInTheDocument();
    expect(screen.getByText('Template 2')).toBeInTheDocument();
    expect(screen.getByText('Template 3')).toBeInTheDocument();
  });

  it('should render column headers', () => {
    render(<TemplateTable {...defaultProps} />);
    
    expect(screen.getByText('ID')).toBeInTheDocument();
    expect(screen.getByText('Name')).toBeInTheDocument();
    expect(screen.getByText('Path')).toBeInTheDocument();
    expect(screen.getByText('Size')).toBeInTheDocument();
    expect(screen.getByText('Actions')).toBeInTheDocument();
  });

  it('should format file sizes correctly', () => {
    render(<TemplateTable {...defaultProps} />);
    
    expect(screen.getByText('1 KB')).toBeInTheDocument();
    expect(screen.getByText('2 KB')).toBeInTheDocument();
    expect(screen.getByText('1 MB')).toBeInTheDocument();
  });

  it('should call onDelete when delete button is clicked', () => {
    const onDelete = vi.fn();
    render(<TemplateTable templates={mockTemplates} onDelete={onDelete} />);
    
    fireEvent.click(screen.getByTestId('delete-button-1'));
    
    expect(onDelete).toHaveBeenCalledWith(mockTemplates[0]);
  });

  it('should render empty state when no templates', () => {
    render(<TemplateTable templates={[]} onDelete={vi.fn()} />);
    
    expect(screen.getByText('No templates found')).toBeInTheDocument();
    expect(screen.getByText('Create your first email template to get started.')).toBeInTheDocument();
  });

  it('should render loading skeleton when isLoading is true', () => {
    render(<TemplateTable templates={[]} onDelete={vi.fn()} isLoading={true} />);
    
    // Table should exist but with skeletons
    expect(screen.getByRole('table')).toBeInTheDocument();
    expect(screen.queryByText('No templates found')).not.toBeInTheDocument();
  });

  it('should show pagination controls', () => {
    render(<TemplateTable {...defaultProps} />);
    
    expect(screen.getByText('Rows per page:')).toBeInTheDocument();
  });
});
