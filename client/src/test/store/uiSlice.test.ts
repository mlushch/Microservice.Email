import { describe, it, expect } from 'vitest';
import uiReducer, {
  UiState,
  setLoading,
  addNotification,
  removeNotification,
  clearNotifications,
  toggleSidebar,
  setSidebarOpen,
} from '../../store/uiSlice';

describe('UI Slice', () => {
  const initialState: UiState = {
    isLoading: false,
    notifications: [],
    sidebarOpen: true,
  };

  describe('reducers', () => {
    it('should return initial state', () => {
      const result = uiReducer(undefined, { type: 'unknown' });
      expect(result).toEqual(initialState);
    });

    it('should handle setLoading', () => {
      const result = uiReducer(initialState, setLoading(true));
      expect(result.isLoading).toBe(true);
    });

    it('should handle addNotification', () => {
      const notification = { message: 'Test message', type: 'success' as const };
      
      const result = uiReducer(initialState, addNotification(notification));
      
      expect(result.notifications).toHaveLength(1);
      expect(result.notifications[0].message).toBe('Test message');
      expect(result.notifications[0].type).toBe('success');
      expect(result.notifications[0].id).toBeDefined();
    });

    it('should handle removeNotification', () => {
      const stateWithNotifications: UiState = {
        ...initialState,
        notifications: [
          { id: 'notif-1', message: 'Message 1', type: 'info' },
          { id: 'notif-2', message: 'Message 2', type: 'error' },
        ],
      };
      
      const result = uiReducer(stateWithNotifications, removeNotification('notif-1'));
      
      expect(result.notifications).toHaveLength(1);
      expect(result.notifications[0].id).toBe('notif-2');
    });

    it('should handle clearNotifications', () => {
      const stateWithNotifications: UiState = {
        ...initialState,
        notifications: [
          { id: 'notif-1', message: 'Message 1', type: 'info' },
          { id: 'notif-2', message: 'Message 2', type: 'error' },
        ],
      };
      
      const result = uiReducer(stateWithNotifications, clearNotifications());
      
      expect(result.notifications).toHaveLength(0);
    });

    it('should handle toggleSidebar', () => {
      const result = uiReducer(initialState, toggleSidebar());
      expect(result.sidebarOpen).toBe(false);
      
      const result2 = uiReducer(result, toggleSidebar());
      expect(result2.sidebarOpen).toBe(true);
    });

    it('should handle setSidebarOpen', () => {
      const result = uiReducer(initialState, setSidebarOpen(false));
      expect(result.sidebarOpen).toBe(false);
    });
  });
});
