# Task 8: Configure MinIO File Storage

## Description
Setup MinIO S3-compatible storage for email attachments and templates.

## Requirements
- Configure MinIO client in `Infrastructure/`
- Implement `IFileStorageService` with:
  - `UploadAsync(stream, fileName)` - Upload file to MinIO
  - `DownloadAsync(fileName)` - Download file from MinIO
  - `RemoveAsync(fileName)` - Delete file from MinIO
- Create bucket for attachments if not exists
- Create bucket for templates if not exists
- Handle connection errors and retries
- Add logging for storage operations
- Use async/await patterns

## Acceptance Criteria
- MinIO is configured and accessible
- File upload, download, and removal operations work
- Buckets are created automatically
- Error handling is implemented
- Service follows the defined interface contract
