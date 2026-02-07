# Technical Documentation: Microservice.Email

## Overview

Microservice.Email is a robust email delivery service built on .NET 9. It provides multiple ways to send emails, including REST APIs and RabbitMQ message processing. It supports templated emails using Scriban and file attachments stored in MinIO. Also it have template managment system which allows to maintain email templates with help od React TS frontend application.

## Technology Stack (Backend)

- **Framework:** .NET 9.0 (ASP.NET Core)
- **Database:** PostgreSQL with Entity Framework Core
- **Messaging:** RabbitMQ
- **File Storage:** MinIO (S3 compatible)
- **Email Delivery:** FluentEmail (SMTP)
- **Templating:** Scriban
- **Monitoring:** Prometheus & Grafana (via prometheus-net)
- **API Documentation:** Swagger/OpenAPI

## Technology Stack (Frontend)

- **Library:** React v19.2
- **State managment:** Redux v5.0.1
- **Api Requests:** Axios v1.13.4

## Project Structure (Backend)

The project follows a modular architecture:

- `Controllers/`: REST API endpoints.
- `Core/`: Business logic, interfaces, validators, and core contracts.
- `Domain/`: Database entities and enums.
- `Infrastructure/`: External integrations (Persistence, Messaging, SMTP, File Storage).
- `Modules/`: Dependency injection registration logic.
- `Templates/`: HTML templates for emails.
- `Extensions/`: Helper methods and reflection extensions.

## Project Structure (Frontend)

The project follows a modular architecture:

- `components/`: Generic react components.
- `services/`: Api, utility services.
- `store/`: Store functions, reducers etc.
- `types/`: Api contracts, utility types.
- `pages/`: Application pages.

## Database Schema

The persistence layer uses PostgreSQL. The main entities are:

### `EmailEntity`

| Column      | Type           | Description                          |
| ----------- | -------------- | ------------------------------------ |
| Id          | int (PK)       | Unique identifier                    |
| Body        | string         | Content of the email                 |
| Subject     | string         | Subject line                         |
| SenderName  | string         | Name of the sender                   |
| SenderEmail | string         | Email address of the sender          |
| SentDate    | DateTimeOffset | When the email was sent              |
| EmailStatus | Enum           | Status (Pending, Sent, Failed, etc.) |

### `RecipientEntity`

| Column  | Type     | Description                |
| ------- | -------- | -------------------------- |
| Id      | int (PK) | Unique identifier          |
| Email   | string   | Recipient email address    |
| EmailId | int (FK) | Reference to `EmailEntity` |

### `EmailTemplateEntity`

| Column  | Type              | Description                 |
| ------- | ----------------- | --------------------------- |
| Id      | int (PK)          | Unique identifier           |
| Path    | string            | Path to template in storage |
| Name    | string (Unique)   | Tamplate name               |
| Size    | int               | Template size in KB         |

## API

### REST API

Base Route: `/api/email`

| Method | Endpoint                    | Description                                       |
| ------ | --------------------------- | ------------------------------------------------- |
| POST   | `/send`                     | Send a standard email with attachments            |
| POST   | `/send/templated`           | Send an email using a template                    |
| POST   | `/send/formFiles`           | Send an email with attachments from form-data     |
| POST   | `/send/templated/formFiles` | Send a templated email with form-data attachments |

Base Route: `/api/email-templates`

| Method | Endpoint                    | Description                                       |
| ------ | --------------------------- | ------------------------------------------------- |
| GET    | `/all`                      | Get list of all template                          |
| POST   | ``                          | Create new email template                         |
| DELETE | ``                          | Delete existing email template                    |

## API Contracts

Described in separate [file](./CONTRACTS.md)

## Project structure

- `client/`: Folder for react frontend application.
- `src/`: Folder for .Net backend application.

## Infrastructure Details

### Messaging

The service listens to RabbitMQ queues for asynchronous email sending tasks.

- Handler: `SendEmailMessageHandler` processes `AttachmentsWrapper<SendEmailRequest>`.

### File Storage

MinIO is used to store attachments temporarily or as a permanent record if configured.

- Service: `IFileStorageService` handles Upload, Download, and Removal.

### SMTP Configuration

Configured via `appsettings.json`, using FluentEmail for SMTP integration.
