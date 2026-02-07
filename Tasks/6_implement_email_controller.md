# Task 6: Implement Email REST API Controller

## Description
Create REST API controller with endpoints for sending emails and managing email templates.

## Requirements
- Create `EmailController` class at `Controllers/EmailController.cs`
- Implement endpoints:
  - `POST /api/email/send` - Send plain email with attachments
  - `POST /api/email/send/templated` - Send templated email
  - `POST /api/email/send/form-files` - Send plain email with form-data attachments
  - `POST /api/email/send/templated/form-files` - Send templated email with form-data attachments
  - `GET /api/email-templates` - Get all templates
  - `POST /api/email-templates` - Create new template
  - `DELETE /api/email-templates/{templateId}` - Delete template
- Use dependency injection for services
- Implement proper request validation
- Return appropriate HTTP status codes
- Add Swagger/OpenAPI documentation
- Use async/await patterns

## Acceptance Criteria
- All endpoints are implemented
- Controller uses dependency injection
- Proper HTTP status codes are returned
- Swagger documentation is available
- Endpoints follow RESTful conventions
