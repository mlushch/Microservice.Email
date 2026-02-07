import { useState } from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TablePagination,
  Paper,
  IconButton,
  Typography,
  Box,
  Skeleton,
} from '@mui/material';
import { Delete as DeleteIcon } from '@mui/icons-material';
import { EmailTemplateResponse } from '../../types';

interface TemplateTableProps {
  templates: EmailTemplateResponse[];
  onDelete: (template: EmailTemplateResponse) => void;
  isLoading?: boolean;
}

export const TemplateTable = ({ templates, onDelete, isLoading = false }: TemplateTableProps) => {
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);

  const handleChangePage = (_: unknown, newPage: number) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0);
  };

  const formatFileSize = (bytes: number): string => {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  };

  // Loading skeleton
  if (isLoading) {
    return (
      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>ID</TableCell>
              <TableCell>Name</TableCell>
              <TableCell>Path</TableCell>
              <TableCell>Size</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {[...Array(5)].map((_, index) => (
              <TableRow key={index}>
                <TableCell><Skeleton animation="wave" /></TableCell>
                <TableCell><Skeleton animation="wave" /></TableCell>
                <TableCell><Skeleton animation="wave" /></TableCell>
                <TableCell><Skeleton animation="wave" /></TableCell>
                <TableCell><Skeleton animation="wave" width={40} /></TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    );
  }

  // Empty state
  if (templates.length === 0) {
    return (
      <Paper sx={{ p: 4, textAlign: 'center' }}>
        <Typography variant="h6" color="text.secondary">
          No templates found
        </Typography>
        <Typography variant="body2" color="text.secondary">
          Create your first email template to get started.
        </Typography>
      </Paper>
    );
  }

  const paginatedTemplates = templates.slice(
    page * rowsPerPage,
    page * rowsPerPage + rowsPerPage
  );

  return (
    <Box>
      <TableContainer component={Paper}>
        <Table data-testid="template-table">
          <TableHead>
            <TableRow>
              <TableCell>ID</TableCell>
              <TableCell>Name</TableCell>
              <TableCell>Path</TableCell>
              <TableCell>Size</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {paginatedTemplates.map((template) => (
              <TableRow key={template.id} data-testid={`template-row-${template.id}`}>
                <TableCell>{template.id}</TableCell>
                <TableCell>{template.name}</TableCell>
                <TableCell>{template.path}</TableCell>
                <TableCell>{formatFileSize(template.size)}</TableCell>
                <TableCell align="right">
                  <IconButton
                    onClick={() => onDelete(template)}
                    color="error"
                    aria-label={`Delete ${template.name}`}
                    data-testid={`delete-button-${template.id}`}
                  >
                    <DeleteIcon />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
      <TablePagination
        rowsPerPageOptions={[5, 10, 25]}
        component="div"
        count={templates.length}
        rowsPerPage={rowsPerPage}
        page={page}
        onPageChange={handleChangePage}
        onRowsPerPageChange={handleChangeRowsPerPage}
      />
    </Box>
  );
};

export default TemplateTable;
