export { store } from './store';
export type { RootState, AppDispatch } from './store';
export { useAppDispatch, useAppSelector } from './hooks';

// Template slice exports
export {
  fetchTemplates,
  createTemplate,
  deleteTemplate,
  clearError,
  setSelectedTemplate,
  selectTemplates,
  selectTemplatesLoading,
  selectTemplatesError,
  selectSelectedTemplate,
} from './templateSlice';
export type { TemplateState } from './templateSlice';

// UI slice exports
export {
  setLoading,
  addNotification,
  removeNotification,
  clearNotifications,
  toggleSidebar,
  setSidebarOpen,
  selectIsLoading,
  selectNotifications,
  selectSidebarOpen,
} from './uiSlice';
export type { UiState, Notification, NotificationType } from './uiSlice';
