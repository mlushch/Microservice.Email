# Code Style Guide: Microservice.Email

This document outlines the coding standards and best practices for the Microservice.Email project to ensure consistency, readability, and maintainability across the codebase.

## 1. General Principles

- **Clean Architecture:** Maintain separation of concerns between Domain, Core (Application), Infrastructure, and API layers.
- **SOLID Principles:** Adhere to SOLID design principles for robust and extensible code.
- **Sealed Classes:** Use `sealed` by default for all classes unless inheritance is explicitly required.
- **Immutability:** Favor immutable data structures. Use `init` properties for DTOs and Contracts.
- **Async/Await:** Use asynchronous programming throughout the application for better scalability. Always use `Task` as return type for async methods.

## 2. Naming Conventions

### 2.1 General Naming
- **PascalCase:** Classes, Methods, Properties, Public Fields, Enums, Namespaces.
- **camelCase:** Method arguments, Local variables.
- **camelCase with `_` prefix:** Private fields (e.g., `private readonly IEmailService _emailService;` - *Note: The project also uses `this.emailService` pattern with plain camelCase fields in some places, but `_` is a common standard*). 
  *Update based on existing code:* Use **camelCase** for private fields and access them via `this.` in constructors (e.g., `this.emailService = emailService;`).

### 2.2 Specific Naming
- **Interfaces:** Prefix with `I` (e.g., `IEmailService`).
- **Classes:** Use descriptive nouns. Suffix with their role (e.g., `EmailService`, `EmailController`, `EmailResponse`, `SendEmailRequest`).
- **Entity Classes:** Suffix with `Entity` (e.g., `EmailEntity`).
- **Methods:** Use verbs or verb-noun pairs (e.g., `Send`, `Validate`, `Process`).
- **Boolean Properties/Variables:** Use prefixes like `Is`, `Has`, `Can` (e.g., `IsSent`).

## 3. Formatting & Syntax

- **Indentation:** Use 4 spaces (standard for C#).
- **Braces:** Use K&R style or Allman style? 
  *Based on code:* Use **Allman style** (braces on a new line) for classes, methods, and complex statements.
- **File-Scoped Namespaces:** Use file-scoped namespaces to reduce nesting.
- **Var Keyword:** Use `var` when the type is obvious from the right side of the assignment (e.g., `var result = new List<string>();`). Use explicit types when it's not (e.g., `int count = GetCount();`).
- **Expression-Bodied Members:** Use for simple one-line properties or methods.
- **Nullable Reference Types:** Enable and use nullable reference types (`?`) to prevent `NullReferenceException`.

## 4. Project-Specific Patterns

### 4.1 Dependency Injection
- Register services in the `Modules` layer.
- Use constructor injection for all dependencies.
- Use `Scrutor` or manual registration as defined in the `IModule` implementations.

### 4.2 Error Handling
- Use global exception handling (interceptors for gRPC, middleware for REST).
- Throw specific exceptions (e.g., `ValidationException`) when business rules are violated.
- Avoid using `try-catch` blocks for flow control.

### 4.3 Persistence (EF Core)
- Use `IEntityTypeConfiguration<T>` for entity configurations.
- Follow snake_case naming for database tables and columns (via `EFCore.NamingConventions`).
- Prefix table names if necessary (e.g., `TEmail`).

### 4.4 Documentation
- Use XML documentation comments (`///`) for public APIs, complex methods, and interfaces.
- Keep `CONTRACTS.md` and `TECHNICAL_DOCUMENTATION.md` updated with any API or architectural changes.
