# Task 15: Create Unit Tests for Services

## Description
Implement comprehensive unit tests for all service layer classes.

## Requirements
- Setup testing framework: xUnit or NUnit
- Create test project structure matching source structure
- Test services:
  - `EmailService` - Test send, send templated, validation
  - `FileStorageService` - Test upload, download, remove operations
  - `TemplateService` - Test CRUD operations, caching
  - `SmtpService` - Test sending with various scenarios
- Create mocks for external dependencies (RabbitMQ, MinIO, SMTP)
- Test cases should cover:
  - Happy path scenarios
  - Error/exception scenarios
  - Validation failures
  - Edge cases (empty recipients, missing templates, etc.)
- Target minimum 80% code coverage
- Use appropriate assertion libraries
- Follow Arrange-Act-Assert (AAA) pattern

## Acceptance Criteria
- All services have unit tests
- Tests pass successfully
- Coverage is at least 80%
- Tests are maintainable and clear
- Mocking is used appropriately
