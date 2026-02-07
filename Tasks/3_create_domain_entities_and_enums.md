# Task 3: Create Domain Entities and Enums

## Description
Define domain entities and enums that represent core business concepts in the email microservice.

## Requirements
- Create `EmailStatusEnum` with values: Pending, Sent, Failed
- Create domain models:
  - `Sender` model with properties: Name (optional), Email
  - `Recipient` model with properties: Id, Email
  - `EmailResponse` model with properties: Id, Sender, Recipients[], SentDate, EmailStatus
  - `EmailTemplateResponse` model with properties: Id, Name, Path, Size
- Implement immutable properties using `init` keywords
- Follow PascalCase naming conventions
- Add XML documentation comments for public types

## Acceptance Criteria
- All enums and domain models are created
- Models use `init` properties for immutability
- Models follow naming conventions and documentation standards
