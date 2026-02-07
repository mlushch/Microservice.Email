# Task 22: Setup Docker Containerization

## Description
Create Docker configuration for containerizing both backend and frontend applications.

## Requirements
- Create `Dockerfile` for backend:
  - Use .NET 9 base image
  - Multi-stage build (build stage and runtime stage)
  - Set working directory
  - Copy project files and restore dependencies
  - Build application
  - Expose port 5000 (or configured port)
- Create `Dockerfile` for frontend:
  - Use Node.js base image for build
  - Build React application
  - Use lightweight image for serving (Nginx)
  - Configure Nginx to proxy API requests
- Create `.dockerignore` files
- Create `docker-compose.yml` for orchestrating services:
  - Backend service
  - Frontend service
  - PostgreSQL database
  - RabbitMQ
  - MinIO
  - Prometheus (optional)
- Add environment configuration files for containers
- Document port mappings and network setup

## Acceptance Criteria
- Docker images build successfully
- Containers run without errors
- Docker Compose orchestrates all services
- Services can communicate with each other
- Data persists properly
