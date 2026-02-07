# Task 18: Create API Client Service

## Description
Implement Axios-based API client for communicating with the backend microservice.

## Requirements
- Create `services/api.ts` for Axios configuration
- Configure base URL from environment variables
- Implement API service for:
  - `GET /api/email-templates` - Fetch all templates
  - `POST /api/email-templates` - Create new template
  - `DELETE /api/email-templates/{templateId}` - Delete template
- Add request/response interceptors:
  - Authorization headers
  - Error handling
  - Request logging
- Create typed responses matching backend contracts
- Handle loading and error states
- Add timeout configuration
- Implement error response parsing

## Acceptance Criteria
- API service is properly configured
- All endpoints are implemented
- Requests use proper HTTP methods and headers
- Error handling works correctly
- Service can be easily imported and used
