# Task 7: Setup RabbitMQ Messaging

## Description
Configure RabbitMQ message consumers for asynchronous email processing.

## Requirements
- Configure RabbitMQ connection in `Infrastructure/`
- Create message handler: `SendEmailMessageHandler` that processes `AttachmentsWrapper<SendEmailRequest>`
- Create message handler for templated emails: `SendTemplatedEmailMessageHandler` for `AttachmentsWrapper<SendTemplatedEmailRequest>`
- Implement message consumer that listens to queues
- Add correlation ID tracking for message tracing
- Use `BusMessage<T>` wrapper for all messages
- Integrate with email service for actual sending
- Use async/await patterns

## Acceptance Criteria
- RabbitMQ is configured and accessible
- Message handlers process both plain and templated emails
- Correlation IDs are tracked
- Messages are consumed and processed asynchronously
- Error handling is implemented for failed messages
