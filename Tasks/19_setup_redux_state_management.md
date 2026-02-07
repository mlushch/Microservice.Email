# Task 19: Setup Redux State Management

## Description
Configure Redux for application state management including templates and UI state.

## Requirements
- Setup Redux store configuration
- Create slices/reducers:
  - `templateSlice` - Manage email templates
  - `uiSlice` - Manage UI state (loading, notifications)
  - `authSlice` - Manage user authentication (if needed)
- Define actions:
  - Fetch all templates
  - Create template
  - Delete template
  - Update UI state (loading, success/error messages)
- Use Redux Thunk for async operations
- Create typed hooks: `useAppDispatch`, `useAppSelector`
- Setup middleware for async operations
- Add logging middleware for debugging (optional)
- Create selectors for derived state

## Acceptance Criteria
- Redux store is properly configured
- State updates work correctly
- Async actions dispatch properly
- Selectors return correct state slices
- Middleware is configured appropriately
