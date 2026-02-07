# Task 24: Add Logging Configuration

## Description
Implement comprehensive logging throughout the application using Serilog.

## Requirements
- Install Serilog NuGet packages
- Configure Serilog in `Program.cs`:
  - Console output for development
  - File output for production
  - Structured logging with properties
- Configure log levels:
  - Debug for development
  - Information for production
  - Error and Critical for issues
- Setup sinks:
  - Console sink
  - File sink (rolling files)
  - Seq sink (optional)
- Add context logging:
  - Request IDs for tracing
  - User information
  - Correlation IDs from RabbitMQ
- Create logging in key areas:
  - Service operations
  - Database queries
  - External API calls
  - Exception handling
- Configure `appsettings.json` and `appsettings.Production.json`

## Acceptance Criteria
- Logging works in all environments
- Structured logging captures relevant context
- Log files are created and rotated
- Log levels can be configured
- Request tracing is possible with correlation IDs
