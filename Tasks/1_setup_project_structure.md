# Task 1: Setup Project Structure

## Description
Create the foundational project structure for the Microservice.Email backend application following Clean Architecture principles.

## Requirements
- Create main project folder structure in `src/` directory
- Implement the following folders:
  - `Controllers/` - REST API endpoints
  - `Core/` - Business logic, interfaces, validators, and core contracts
  - `Domain/` - Database entities and enums
  - `Infrastructure/` - External integrations (Persistence, Messaging, SMTP, File Storage)
  - `Modules/` - Dependency injection registration logic
  - `Templates/` - HTML templates for emails
  - `Extensions/` - Helper methods and reflection extensions

## Acceptance Criteria
- All folders are created in `src/` directory
- Project follows Clean Architecture principles
- Structure supports separation of concerns
