# Task 16: Create Integration Tests

## Description
Implement integration tests for API endpoints and external service interactions.

## Requirements
- Create test database (PostgreSQL test instance or in-memory alternative)
- Setup test fixtures for:
  - Database context
  - WebApplicationFactory for testing
  - Test data seeding
- Test API endpoints:
  - All email sending endpoints (plain, templated, form-data)
  - Template management endpoints (get all, create, delete)
- Test external integrations:
  - RabbitMQ message consumption
  - MinIO file storage operations
  - SMTP email delivery
- Test error scenarios:
  - Invalid requests
  - Missing resources
  - Service failures
- Use TestContainers for containerized dependencies (optional)
- Add database cleanup between tests

## Acceptance Criteria
- Integration tests cover major API flows
- Tests use real database connections
- External services are properly mocked or containerized
- Tests pass consistently
- Cleanup prevents test pollution
