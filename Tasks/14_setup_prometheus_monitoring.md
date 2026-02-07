# Task 14: Setup Prometheus Monitoring

## Description
Configure Prometheus metrics collection using prometheus-net for application monitoring.

## Requirements
- Install prometheus-net NuGet package
- Configure Prometheus in `Program.cs`
- Create custom metrics:
  - Counter for emails sent (success/failure)
  - Counter for template operations
  - Histogram for email send duration
  - Gauge for queue depth (RabbitMQ)
  - Gauge for storage operations
- Expose metrics endpoint at `/metrics`
- Add middleware for request/response metrics
- Track:
  - HTTP request duration
  - Exception counts by type
  - Active connections
- Configure scrape interval in Prometheus configuration file
- Create Grafana dashboard configuration (optional)

## Acceptance Criteria
- Metrics endpoint is accessible
- Custom metrics are being collected
- Request metrics are recorded
- Prometheus can scrape the endpoint
- Metrics include relevant business operations
