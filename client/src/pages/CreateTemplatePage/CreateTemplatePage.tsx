import { useNavigate } from 'react-router-dom';
import { Box, Typography, Breadcrumbs, Link } from '@mui/material';
import { Link as RouterLink } from 'react-router-dom';
import { TemplateForm } from '../../components';
import {
  useAppDispatch,
  useAppSelector,
  createTemplate,
  selectTemplatesLoading,
  addNotification,
} from '../../store';

export const CreateTemplatePage = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const isLoading = useAppSelector(selectTemplatesLoading);

  const handleSubmit = async (data: { name: string; path: string; file: File }) => {
    const result = await dispatch(createTemplate(data));
    if (createTemplate.fulfilled.match(result)) {
      dispatch(addNotification({ message: 'Template created successfully', type: 'success' }));
      navigate('/');
    } else if (createTemplate.rejected.match(result)) {
      dispatch(addNotification({ 
        message: result.payload || 'Failed to create template', 
        type: 'error' 
      }));
    }
  };

  return (
    <Box>
      <Breadcrumbs sx={{ mb: 2 }}>
        <Link component={RouterLink} to="/" color="inherit">
          Home
        </Link>
        <Link component={RouterLink} to="/" color="inherit">
          Templates
        </Link>
        <Typography color="text.primary">Create</Typography>
      </Breadcrumbs>

      <Typography variant="h4" component="h1" sx={{ mb: 3 }}>
        Create Email Template
      </Typography>

      <TemplateForm onSubmit={handleSubmit} isLoading={isLoading} />
    </Box>
  );
};

export default CreateTemplatePage;
