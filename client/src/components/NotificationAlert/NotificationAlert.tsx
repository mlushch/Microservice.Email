import { useEffect } from 'react';
import { Snackbar, Alert } from '@mui/material';
import { useAppDispatch, useAppSelector, selectNotifications, removeNotification } from '../../store';

export const NotificationAlert = () => {
  const dispatch = useAppDispatch();
  const notifications = useAppSelector(selectNotifications);
  const currentNotificationId = notifications[0]?.id;

  // Auto-remove notifications after 5 seconds
  useEffect(() => {
    if (!currentNotificationId) {
      return;
    }

    const timer = setTimeout(() => {
      dispatch(removeNotification(currentNotificationId));
    }, 5000);

    return () => clearTimeout(timer);
  }, [currentNotificationId, dispatch]);
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
