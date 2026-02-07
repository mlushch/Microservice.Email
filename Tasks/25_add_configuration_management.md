# Task 25: Add Configuration Management

## Description
Setup configuration management for different environments (Development, Staging, Production).

## Requirements
- Create configuration files:
  - `appsettings.json` - Base configuration
  - `appsettings.Development.json` - Development overrides
  - `appsettings.Staging.json` - Staging overrides
  - `appsettings.Production.json` - Production overrides
- Configure settings for:
  - Database connection string
  - RabbitMQ connection
  - MinIO endpoints and credentials
  - SMTP configuration
  - Logging level
  - Prometheus settings
  - Frontend API URL
- Use .NET configuration providers:
  - JSON files
  - Environment variables
  - User secrets for development
- Create strongly-typed configuration classes using Options pattern
- Document all configuration options
- Add validation for required settings on startup

## Acceptance Criteria
- Configuration can be overridden per environment
- Environment variables work correctly
- User secrets are used for sensitive data in development
- Configuration is strongly typed and validated
- Application fails fast with missing required config
