import { configureStore } from '@reduxjs/toolkit';
import templateReducer from './templateSlice';
import uiReducer from './uiSlice';

export const store = configureStore({
  reducer: {
    templates: templateReducer,
    ui: uiReducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: {
        // Ignore these action types (for file uploads)
        ignoredActions: ['templates/create/pending'],
      },
    }),
  devTools: import.meta.env.DEV,
});

// Export types
export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
