# Database Layer Setup

## Overview
This directory contains the Entity Framework Core configuration for the Email microservice's PostgreSQL database.

## Structure
- **`EmailDbContext.cs`** - The DbContext for database operations
- **`EntityConfigurations/`** - Entity type configurations using `IEntityTypeConfiguration<T>`
- **`Migrations/`** - Database migrations

## Entities
- **`EmailEntity`** - Main email message entity
- **`RecipientEntity`** - Email recipients (one-to-many relationship with EmailEntity)
- **`EmailTemplateEntity`** - Email templates for templated email sends

## Configuration
- Database: PostgreSQL
- ORM: Entity Framework Core
- Naming Convention: snake_case for tables and columns (via EFCore.NamingConventions)

## Applying Migrations
To apply the initial migration to your PostgreSQL database:

```bash
dotnet ef database update
```

## Creating New Migrations
When you modify entities, create a new migration:

```bash
dotnet ef migrations add <MigrationName>
```

Then apply it:

```bash
dotnet ef database update
```
