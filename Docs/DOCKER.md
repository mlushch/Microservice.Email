# Docker Configuration

## Overview

This project is fully containerized using Docker. The setup includes:

- **Backend**: .NET 9 API service
- **Frontend**: React application served via Nginx
- **PostgreSQL**: Database
- **RabbitMQ**: Message broker
- **MinIO**: S3-compatible object storage
- **MailHog**: Email testing tool
- **Prometheus**: Metrics collection

## Quick Start

### Prerequisites

- Docker Desktop installed
- Docker Compose v2+

### Running the Application

1. Clone the repository
2. Copy environment file (optional):
   ```bash
   cp .env.example .env
   ```

3. Start all services:
   ```bash
   docker compose up -d
   ```

4. Access the application:
   - Frontend: http://localhost:3000
   - Backend API: http://localhost:5000
   - Swagger UI: http://localhost:5000/swagger
   - RabbitMQ Management: http://localhost:15672 (guest/guest)
   - MinIO Console: http://localhost:9001 (minioadmin/minioadmin)
   - MailHog: http://localhost:8025
   - Prometheus: http://localhost:9090

### Stopping Services

```bash
docker compose down
```

To remove volumes (data):
```bash
docker compose down -v
```

## Port Mappings

| Service    | Container Port | Host Port | Description               |
|------------|----------------|-----------|---------------------------|
| Backend    | 5000           | 5000      | API endpoints             |
| Frontend   | 80             | 3000      | Web application           |
| PostgreSQL | 5432           | 5432      | Database                  |
| RabbitMQ   | 5672           | 5672      | AMQP protocol             |
| RabbitMQ   | 15672          | 15672     | Management UI             |
| MinIO      | 9000           | 9000      | S3 API                    |
| MinIO      | 9001           | 9001      | Console UI                |
| MailHog    | 1025           | 1025      | SMTP server               |
| MailHog    | 8025           | 8025      | Web UI                    |
| Prometheus | 9090           | 9090      | Metrics UI                |

## Network Configuration

All services are connected via the `microservice-network` bridge network. Services can communicate using their service names as hostnames.

## Building Individual Images

### Backend
```bash
docker build -t microservice-email-backend ./src
```

### Frontend
```bash
docker build -t microservice-email-frontend ./client
```

## Development Mode

For development, use the override file:
```bash
docker compose -f docker-compose.yml -f docker-compose.override.yml up -d
```

## Health Checks

All critical services have health checks configured:
- Backend: `/health` endpoint
- PostgreSQL: `pg_isready`
- RabbitMQ: `rabbitmq-diagnostics ping`
- MinIO: `mc ready local`

## Volumes

Persistent data is stored in Docker volumes:
- `postgres-data`: Database files
- `rabbitmq-data`: Message broker data
- `minio-data`: Object storage files
- `prometheus-data`: Metrics history

## Environment Variables

### Backend Service

| Variable | Description | Default |
|----------|-------------|---------|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string | See docker-compose.yml |
| `Minio__Endpoint` | MinIO server endpoint | minio:9000 |
| `Minio__AccessKey` | MinIO access key | minioadmin |
| `Minio__SecretKey` | MinIO secret key | minioadmin |
| `RabbitMq__HostName` | RabbitMQ hostname | rabbitmq |
| `Smtp__Host` | SMTP server hostname | mailhog |

## Troubleshooting

### Containers not starting
Check logs:
```bash
docker compose logs -f [service-name]
```

### Database connection issues
Ensure PostgreSQL is healthy:
```bash
docker compose ps postgres
```

### Permission denied errors
On Linux, you may need to adjust volume permissions or run with sudo.

### Port conflicts
If ports are already in use, modify the port mappings in `docker-compose.yml`.
