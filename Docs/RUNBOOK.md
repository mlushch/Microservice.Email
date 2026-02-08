# Runbook - Microservice.Email

This document provides instructions for running, building, and testing the Microservice.Email project in local and Docker environments.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Project Dependencies](#project-dependencies)
- [Local Development Setup](#local-development-setup)
- [Backend Commands](#backend-commands)
- [Frontend Commands](#frontend-commands)
- [Docker Environment](#docker-environment)
- [Service URLs and Ports](#service-urls-and-ports)
- [Environment Variables](#environment-variables)

---

## Prerequisites

### Required Software

| Software | Version | Purpose |
|----------|---------|---------|
| .NET SDK | 9.0+ | Backend development |
| Node.js | 20+ LTS | Frontend development |
| npm | 10+ | Package management |
| Docker Desktop | Latest | Containerization |
| Docker Compose | v2+ | Multi-container orchestration |
| Git | Latest | Version control |

## Project Dependencies

### Backend Dependencies (.NET 9)

| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.EntityFrameworkCore | 9.0.12 | ORM |
| Npgsql.EntityFrameworkCore.PostgreSQL | 9.0.4 | PostgreSQL provider |
| RabbitMQ.Client | 7.1.2 | Message broker client |
| Minio | 6.0.4 | S3-compatible storage client |
| FluentEmail.Core | 3.0.2 | Email sending |
| FluentEmail.Smtp | 3.0.2 | SMTP transport |
| Scriban | 5.12.0 | Template engine |
| Serilog.AspNetCore | 10.0.0 | Logging |
| Swashbuckle.AspNetCore | 7.3.1 | Swagger/OpenAPI |
| prometheus-net.AspNetCore | 8.2.1 | Metrics |
| Scrutor | 6.0.1 | DI scanning |

### Frontend Dependencies (React + TypeScript)

| Package | Version | Purpose |
|---------|---------|---------|
| react | 18.3.1 | UI framework |
| react-dom | 18.3.1 | DOM rendering |
| react-router-dom | 7.13.0 | Routing |
| @reduxjs/toolkit | 2.11.2 | State management |
| react-redux | 9.2.0 | React-Redux bindings |
| axios | 1.13.4 | HTTP client |
| @mui/material | 7.3.7 | UI components |
| @emotion/react | 11.14.0 | CSS-in-JS |

---

## Local Development Setup

### 1. Clone Repository

```bash
git clone https://github.com/<your-org>/Microservice.Email.git
cd Microservice.Email
```

### 2. Start Infrastructure Services

Start required services (PostgreSQL, RabbitMQ, MinIO, MailHog) using Docker:

```bash
docker compose up -d postgres rabbitmq minio mailhog
```

### 3. Setup Backend

```bash
cd src/Microservice.Email

# Restore dependencies
dotnet restore

# Apply database migrations (if using EF migrations)
dotnet ef database update

# Run the backend
dotnet run
```

### 4. Setup Frontend

```bash
cd client

# Install dependencies
npm install

# Run development server
npm run dev
```

---

## Backend Commands

All commands should be run from the `src/Microservice.Email` directory.

### Build Commands

```bash
# Restore dependencies
dotnet restore

# Build project
dotnet build

# Build in Release mode
dotnet build -c Release

# Clean build artifacts
dotnet clean
```

### Run Commands

```bash
# Run in Development mode
dotnet run

# Run with specific environment
dotnet run --environment Development
dotnet run --environment Production

# Run with watch mode (auto-restart on changes)
dotnet watch run
```

### Test Commands

Run from the `src` directory or `Microservice.Email.Tests` directory:

```bash
# Run all tests
dotnet test

# Run tests with verbose output
dotnet test -v n

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "FullyQualifiedName~EmailServiceTests"

# Run tests in specific project
dotnet test Microservice.Email.Tests/Microservice.Email.Tests.csproj
```

### Entity Framework Commands

```bash
# Add migration
dotnet ef migrations add <MigrationName>

# Apply migrations
dotnet ef database update

# Remove last migration
dotnet ef migrations remove

# Generate SQL script
dotnet ef migrations script
```

### Publish Commands

```bash
# Publish for production
dotnet publish -c Release -o ./publish

# Publish self-contained
dotnet publish -c Release -r win-x64 --self-contained
dotnet publish -c Release -r linux-x64 --self-contained
```

---

## Frontend Commands

All commands should be run from the `client` directory.

### Install Dependencies

```bash
# Install all dependencies
npm install

# Install specific package
npm install <package-name>

# Install dev dependency
npm install -D <package-name>

# Clean install (removes node_modules first)
rm -rf node_modules && npm install
```

### Development

```bash
# Start development server (hot reload)
npm run dev

# Preview production build
npm run preview
```

### Build

```bash
# Build for production
npm run build

# Build with TypeScript checking
npx tsc -b && npm run build
```

### Testing

```bash
# Run tests in watch mode
npm test

# Run tests once
npm run test:run

# Run tests with coverage
npm run test:coverage
```

### Linting

```bash
# Run ESLint
npm run lint

# Fix auto-fixable issues
npm run lint -- --fix
```

---

## Docker Environment

### Quick Start (All Services)

```bash
# Start all services
docker compose up -d

# Start all services with build
docker compose up -d --build

# Start with logs visible
docker compose up

# Stop all services
docker compose down

# Stop and remove volumes
docker compose down -v
```

### Start Specific Services

```bash
# Start only infrastructure (database, message broker, storage)
docker compose up -d postgres rabbitmq minio mailhog

# Start backend only (requires infrastructure)
docker compose up -d backend

# Start frontend only (requires backend)
docker compose up -d frontend

# Start monitoring stack
docker compose up -d prometheus
```

### Development with Docker Override

```bash
# Start with development configuration
docker compose -f docker-compose.yml -f docker-compose.override.yml up -d

# This enables:
# - Development environment settings
# - Swagger UI container on port 8080
# - Hot-reload configuration mounting
```

### Container Management

```bash
# View running containers
docker compose ps

# View all containers (including stopped)
docker compose ps -a

# View container logs
docker compose logs backend
docker compose logs frontend
docker compose logs -f backend  # Follow logs

# Restart specific service
docker compose restart backend

# Rebuild specific service
docker compose up -d --build backend

# Execute command in container
docker compose exec backend bash
docker compose exec postgres psql -U postgres -d microservice_email
```

### Volume Management

```bash
# List volumes
docker volume ls

# Remove specific volume
docker volume rm microserviceemail_postgres-data

# Prune unused volumes
docker volume prune
```

### Image Management

```bash
# List images
docker images | grep microservice

# Remove images
docker rmi microserviceemail-backend
docker rmi microserviceemail-frontend

# Rebuild without cache
docker compose build --no-cache
```

---

## Environment Variables

### Backend Configuration

Create `appsettings.Development.json` or use environment variables:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=microservice_email;Username=postgres;Password=postgres"
  },
  "Minio": {
    "Endpoint": "localhost:9000",
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin"
  },
  "RabbitMq": {
    "HostName": "localhost",
    "Username": "guest",
    "Password": "guest"
  },
  "Smtp": {
    "Host": "localhost",
    "Port": 1025
  }
}
```

### Docker Environment Variables

Create a `.env` file in the project root:

```env
# PostgreSQL
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres

# MinIO
MINIO_ROOT_USER=minioadmin
MINIO_ROOT_PASSWORD=minioadmin
MINIO_ACCESS_KEY=minioadmin
MINIO_SECRET_KEY=minioadmin

# RabbitMQ
RABBITMQ_DEFAULT_USER=guest
RABBITMQ_DEFAULT_PASS=guest
```

---

## Quick Reference

### Start Development Environment

```bash
# 1. Start infrastructure
docker compose up -d postgres rabbitmq minio mailhog

# 2. Start backend (new terminal)
cd src/Microservice.Email
dotnet watch run

# 3. Start frontend (new terminal)
cd client
npm run dev
```

### Start Docker Environment

```bash
# Everything at once
docker compose up -d --build

# Or with logs
docker compose up --build
```

### Run All Tests

```bash
# Backend tests
cd src
dotnet test

# Frontend tests
cd client
npm run test:run
```
