# Task 5: Create Core Services and Interfaces

## Description
Implement core business logic services and their interfaces for email sending and template management.

## Requirements
- Create interfaces in `Core/`:
  - `IEmailService` with methods: SendAsync, SendTemplatedAsync
  - `IFileStorageService` with methods: UploadAsync, DownloadAsync, RemoveAsync
  - `ITemplateService` with methods: GetAllAsync, CreateAsync, DeleteAsync
  - `ISmtpService` with methods: SendAsync (for SMTP operations)
  - `IValidator<T>` for validation contracts
- Create implementations following SOLID principles
- Use async/await patterns with proper `Task` and `Task<T>` return types
- Use constructor injection for dependencies
- Use sealed classes by default
- Add proper error handling with specific exceptions
- Create validators for business logic validation

## Acceptance Criteria
- All interfaces are defined with clear contracts
- Service implementations follow SOLID principles
- Async methods use proper Task patterns
- Validators are implemented for key operations
- Services use constructor injection
