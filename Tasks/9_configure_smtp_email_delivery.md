# Task 9: Configure SMTP Email Delivery with FluentEmail

## Description
Setup SMTP configuration using FluentEmail for actual email delivery.

## Requirements
- Configure FluentEmail in `appsettings.json` with SMTP settings
- Create `ISmtpService` implementation using FluentEmail
- Configure SMTP server details (host, port, username, password)
- Implement TLS/SSL support
- Create method to send emails with:
  - Sender information
  - Recipients
  - Subject and body
  - Attachments support
- Add retry logic for failed sends
- Add logging for send operations
- Handle SMTP exceptions gracefully

## Acceptance Criteria
- SMTP is configured via appsettings.json
- FluentEmail is integrated
- Emails can be sent successfully
- Attachments are properly handled
- Retry logic works for transient failures
- Proper error logging is in place
