/// <reference types="vitest" />

import { describe, it, expect, vi } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import { DeleteConfirmation } from '../../components/DeleteConfirmation';

describe('DeleteConfirmation', () => {
  const defaultProps = {
    open: true,
    message: 'Are you sure you want to delete this?',
    onConfirm: vi.fn(),
    onCancel: vi.fn(),
  };

  it('should render dialog when open', () => {
    render(<DeleteConfirmation {...defaultProps} />);
    
    expect(screen.getByText('Confirm Delete')).toBeInTheDocument();
    expect(screen.getByText('Are you sure you want to delete this?')).toBeInTheDocument();
  });

  it('should render custom title', () => {
    render(<DeleteConfirmation {...defaultProps} title="Custom Title" />);
    
    expect(screen.getByText('Custom Title')).toBeInTheDocument();
  });

  it('should render item name when provided', () => {
    render(<DeleteConfirmation {...defaultProps} itemName="Test Item" />);
    
    expect(screen.getByText('Test Item')).toBeInTheDocument();
  });

  it('should call onConfirm when confirm button is clicked', () => {
    const onConfirm = vi.fn();
    render(<DeleteConfirmation {...defaultProps} onConfirm={onConfirm} />);
    
    fireEvent.click(screen.getByTestId('confirm-delete-button'));
    
    expect(onConfirm).toHaveBeenCalledTimes(1);
  });

  it('should call onCancel when cancel button is clicked', () => {
    const onCancel = vi.fn();
    render(<DeleteConfirmation {...defaultProps} onCancel={onCancel} />);
    
    fireEvent.click(screen.getByTestId('cancel-delete-button'));
    
    expect(onCancel).toHaveBeenCalledTimes(1);
  });

  it('should show loading state when isLoading is true', () => {
    render(<DeleteConfirmation {...defaultProps} isLoading={true} />);
    
    expect(screen.getByText('Deleting...')).toBeInTheDocument();
    expect(screen.getByTestId('confirm-delete-button')).toBeDisabled();
    expect(screen.getByTestId('cancel-delete-button')).toBeDisabled();
  });

  it('should not render when closed', () => {
    render(<DeleteConfirmation {...defaultProps} open={false} />);
    
    expect(screen.queryByText('Confirm Delete')).not.toBeInTheDocument();
  });
});
