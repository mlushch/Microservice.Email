import { useEffect } from 'react';
import { Snackbar, Alert } from '@mui/material';
import { useAppDispatch, useAppSelector, selectNotifications, removeNotification } from '../../store';

export const NotificationAlert = () => {
  const dispatch = useAppDispatch();
  const notifications = useAppSelector(selectNotifications);

  // Auto-remove notifications after 5 seconds
  useEffect(() => {
    if (notifications.length > 0) {
      const timer = setTimeout(() => {
        dispatch(removeNotification(notifications[0].id));
      }, 5000);
      return () => clearTimeout(timer);
    }
  }, [notifications, dispatch]);

  const handleClose = (id: string) => {
    dispatch(removeNotification(id));
  };

  if (notifications.length === 0) return null;

  const currentNotification = notifications[0];

  return (
    <Snackbar
      open={true}
      anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
      onClose={() => handleClose(currentNotification.id)}
    >
      <Alert
        onClose={() => handleClose(currentNotification.id)}
        severity={currentNotification.type}
        sx={{ width: '100%' }}
        data-testid="notification-alert"
      >
        {currentNotification.message}
      </Alert>
    </Snackbar>
  );
};

export default NotificationAlert;
