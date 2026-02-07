import { useEffect, useState } from 'react';
import { Box, Typography, Button, Breadcrumbs, Link } from '@mui/material';
import { Add as AddIcon, Refresh as RefreshIcon } from '@mui/icons-material';
import { Link as RouterLink } from 'react-router-dom';
import { TemplateTable, DeleteConfirmation } from '../../components';
import {
  useAppDispatch,
  useAppSelector,
  fetchTemplates,
  deleteTemplate,
  selectTemplates,
  selectTemplatesLoading,
  selectTemplatesError,
  addNotification,
} from '../../store';
import { EmailTemplateResponse } from '../../types';

export const TemplateListPage = () => {
  const dispatch = useAppDispatch();
  const templates = useAppSelector(selectTemplates);
  const isLoading = useAppSelector(selectTemplatesLoading);
  const error = useAppSelector(selectTemplatesError);

  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [templateToDelete, setTemplateToDelete] = useState<EmailTemplateResponse | null>(null);

  useEffect(() => {
    dispatch(fetchTemplates());
  }, [dispatch]);

  useEffect(() => {
    if (error) {
      dispatch(addNotification({ message: error, type: 'error' }));
    }
  }, [error, dispatch]);

  const handleRefresh = () => {
    dispatch(fetchTemplates());
  };

  const handleDeleteClick = (template: EmailTemplateResponse) => {
    setTemplateToDelete(template);
    setDeleteDialogOpen(true);
  };

  const handleDeleteConfirm = async () => {
    if (templateToDelete) {
      const result = await dispatch(deleteTemplate(templateToDelete.id));
      if (deleteTemplate.fulfilled.match(result)) {
        dispatch(addNotification({ message: 'Template deleted successfully', type: 'success' }));
      }
      setDeleteDialogOpen(false);
      setTemplateToDelete(null);
    }
  };

  const handleDeleteCancel = () => {
    setDeleteDialogOpen(false);
    setTemplateToDelete(null);
  };

  return (
    <Box>
      <Breadcrumbs sx={{ mb: 2 }}>
        <Link component={RouterLink} to="/" color="inherit">
          Home
        </Link>
        <Typography color="text.primary">Templates</Typography>
      </Breadcrumbs>

      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4" component="h1">
          Email Templates
        </Typography>
        <Box>
          <Button
            startIcon={<RefreshIcon />}
            onClick={handleRefresh}
            sx={{ mr: 1 }}
            disabled={isLoading}
          >
            Refresh
          </Button>
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            component={RouterLink}
            to="/create"
          >
            Create Template
          </Button>
        </Box>
      </Box>

      <TemplateTable
        templates={templates}
        onDelete={handleDeleteClick}
        isLoading={isLoading}
      />

      <DeleteConfirmation
        open={deleteDialogOpen}
        message="Are you sure you want to delete this template?"
        itemName={templateToDelete?.name}
        onConfirm={handleDeleteConfirm}
        onCancel={handleDeleteCancel}
        isLoading={isLoading}
      />
    </Box>
  );
};

export default TemplateListPage;
