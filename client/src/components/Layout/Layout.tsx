import { ReactNode } from 'react';
import { Link, useLocation } from 'react-router-dom';
import {
  AppBar,
  Box,
  Toolbar,
  Typography,
  Drawer,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Container,
  CssBaseline,
} from '@mui/material';
import {
  Email as EmailIcon,
  Add as AddIcon,
  List as ListIcon,
} from '@mui/icons-material';
import { NotificationAlert } from '../NotificationAlert';

const DRAWER_WIDTH = 240;

interface LayoutProps {
  children: ReactNode;
}

interface NavItem {
  text: string;
  path: string;
  icon: ReactNode;
}

const navItems: NavItem[] = [
  { text: 'Templates', path: '/', icon: <ListIcon /> },
  { text: 'Create Template', path: '/create', icon: <AddIcon /> },
];

export const Layout = ({ children }: LayoutProps) => {
  const location = useLocation();

  return (
    <Box sx={{ display: 'flex' }}>
      <CssBaseline />
      <AppBar
        position="fixed"
        sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}
      >
        <Toolbar>
          <EmailIcon sx={{ mr: 2 }} />
          <Typography variant="h6" noWrap component="div">
            Email Template Manager
          </Typography>
        </Toolbar>
      </AppBar>

      <Drawer
        variant="permanent"
        sx={{
          width: DRAWER_WIDTH,
          flexShrink: 0,
          '& .MuiDrawer-paper': {
            width: DRAWER_WIDTH,
            boxSizing: 'border-box',
          },
        }}
      >
        <Toolbar />
        <Box sx={{ overflow: 'auto' }}>
          <List>
            {navItems.map((item) => (
              <ListItem key={item.text} disablePadding>
                <ListItemButton
                  component={Link}
                  to={item.path}
                  selected={location.pathname === item.path}
                >
                  <ListItemIcon>{item.icon}</ListItemIcon>
                  <ListItemText primary={item.text} />
                </ListItemButton>
              </ListItem>
            ))}
          </List>
        </Box>
      </Drawer>

      <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
        <Toolbar />
        <Container maxWidth="lg">
          {children}
        </Container>
      </Box>

      <NotificationAlert />
    </Box>
  );
};

export default Layout;
