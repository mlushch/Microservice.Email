# API Contracts: Microservice.Email

This document describes the API contracts for the Microservice.Email service, including REST and RabbitMQ message formats.

## 1. REST API

The REST API is exposed at `/api/email`.

### Endpoints

#### Send Plain Email

- **URL:** `POST /api/email/send`
- **Request Body:** `AttachmentsWrapper<SendEmailRequest>`
- **Response:** `EmailResponse`

#### Send Templated Email

- **URL:** `POST /api/email/send/templated`
- **Request Body:** `AttachmentsWrapper<SendTemplatedEmailRequest>`
- **Response:** `EmailResponse`

#### Send Plain Email (Multipart/Form-Data)

- **URL:** `POST /api/email/send/formFiles`
- **Request Body:** `FormFilesWrapper<SendEmailRequest>` (via form data)
- **Response:** `EmailResponse`

#### Send Templated Email (Multipart/Form-Data)

- **URL:** `POST /api/email/send/templated/formFiles`
- **Request Body:** `FormFilesWrapper<SendTemplatedEmailRequest>` (via form data)
- **Response:** `EmailResponse`

#### Get all templates

- **URL:** `GET /api/email-templates/all`
- **Response:** `EmailTemplateResponse[]`

#### Create template

- **URL:** `POST /api/email-templates`
- **Request Body:** `CreateEmailTemplateRequest`
- **Response:** `NoContent on success`

#### Delete template

- **URL:** `DELETE /api/email-templates/{templateId}`
- **Response:** `NoContent on success`

### Models

#### SendEmailRequest

```json
{
  "sender": {
    "name": "string (optional)",
    "email": "string"
  },
  "recipients": ["string"],
  "subject": "string",
  "body": "string"
}
```

#### SendTemplatedEmailRequest

```json
{
  "sender": {
    "name": "string (optional)",
    "email": "string"
  },
  "recipients": ["string"],
  "templateName": "string",
  "templateProperties": "dictionary (JSON properties for mapping to email template)"
}
```

#### AttachmentsWrapper<T>

```json
{
  "email": { "...": "..." },
  "attachments": [
    {
      "fileName": "string"
    }
  ]
}
```

#### EmailResponse

```json
{
  "id": "int",
  "sender": {
    "name": "string",
    "email": "string"
  },
  "recipients": [
    {
      "id": "int",
      "email": "string"
    }
  ],
  "sentDate": "datetime",
  "emailStatus": {
    "id": "int",
    "name": "string"
  }
}
```

#### EmailTemplateResponse

```json
{
  "id": "int",
  "name": "string",
  "path": "string",
  "size": "int"
}
```

#### CreateEmailTemplateRequest

```json
{
  "name": "string",
  "path": "string",
  "file": "IFormFile"
}
```

---

## 2. RabbitMQ Messaging

The service consumes messages from RabbitMQ for asynchronous email processing.

### Message Format

Messages are wrapped in a `BusMessage<T>` container.

#### BusMessage<T>

```json
{
  "payload": { "...": "..." },
  "correlationId": "string (Guid)"
}
```

### Supported Payloads

#### Send Email Message

Payload type: `AttachmentsWrapper<SendEmailRequest>`

#### Send Templated Email Message

Payload type: `AttachmentsWrapper<SendTemplatedEmailRequest>`
