# Task 4: Create API Request/Response Contracts

## Description
Define API contracts for all REST API endpoints and messaging operations.

## Requirements
- Create request contracts:
  - `SendEmailRequest` with: sender (Sender object), recipients (string[]), subject, body
  - `SendTemplatedEmailRequest` with: sender (Sender object), recipients (string[]), templateName, templateProperties (Dictionary)
  - `CreateEmailTemplateRequest` with: name, path, file (IFormFile)
- Create wrapper contracts:
  - `AttachmentsWrapper<T>` with: email (generic T), attachments (array of objects with fileName)
  - `FormFilesWrapper<T>` with: email (generic T), files (IFormFile[])
  - `BusMessage<T>` for RabbitMQ messages with: payload (generic T), correlationId (Guid)
- Use `init` properties for immutability
- Add XML documentation comments
- Follow naming convention with `Request` and `Response` suffixes

## Acceptance Criteria
- All request/response contracts are created
- Contracts use proper typing and generics
- Contracts follow immutability patterns with `init`
- Documentation is complete
