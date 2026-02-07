# Task 2: Setup Database Layer with Entity Framework Core

## Description
Configure PostgreSQL database connection and create entity models for the email microservice.

## Requirements
- Configure Entity Framework Core with PostgreSQL
- Create database entities:
  - `EmailEntity` with properties: Id, Body, Subject, SenderName, SenderEmail, SentDate, EmailStatus
  - `RecipientEntity` with properties: Id, Email, EmailId (FK)
  - `EmailTemplateEntity` with properties: Id, Path, Name (Unique), Size
- Setup entity configurations using `IEntityTypeConfiguration<T>`
- Use snake_case naming convention for database tables and columns via `EFCore.NamingConventions`
- Add migration for initial schema

## Acceptance Criteria
- PostgreSQL connection is configured
- All entities are created with proper relationships
- Entity configurations follow project conventions
- Database migrations are created and can be applied
