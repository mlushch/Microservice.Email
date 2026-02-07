# Task 11: Setup Dependency Injection Container

## Description
Configure the dependency injection container for all services following the project's module pattern.

## Requirements
- Create `IModule` interface for service registration
- Implement modules in `Modules/` folder:
  - `PersistenceModule` - Register EF Core and database services
  - `InfrastructureModule` - Register external services (MinIO, SMTP, RabbitMQ)
  - `CoreModule` - Register business logic services
  - `TemplateModule` - Register template services
- Register all services with appropriate lifetimes:
  - Singleton: Configuration, Logging
  - Scoped: Database context
  - Transient: Request handlers, validators
- Use Scrutor for assembly scanning if appropriate
- Register Swagger/OpenAPI services
- Configure logging

## Acceptance Criteria
- All modules are created
- Services are registered with correct lifetimes
- DI container resolves dependencies correctly
- Module pattern is followed consistently
