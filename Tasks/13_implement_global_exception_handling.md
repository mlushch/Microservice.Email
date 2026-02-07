# Task 13: Implement Global Exception Handling

## Description
Create middleware for centralized exception handling across the REST API.

## Requirements
- Create exception handling middleware
- Define custom exception types:
  - `ValidationException` - For business rule violations
  - `NotFoundException` - For missing resources
  - `StorageException` - For file storage errors
  - `SmtpException` - For email delivery errors
- Map exceptions to appropriate HTTP status codes:
  - ValidationException → 400 Bad Request
  - NotFoundException → 404 Not Found
  - Others → 500 Internal Server Error
- Include error details in response body:
  - Error message
  - Error code
  - Timestamp
  - Request ID (for correlation)
- Add logging for all exceptions
- Register middleware in `Program.cs`

## Acceptance Criteria
- Unhandled exceptions are caught and logged
- Proper HTTP status codes are returned
- Error responses follow consistent format
- Stack traces are hidden in production
- Request IDs enable tracing
