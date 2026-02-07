# Task 23: Create CI/CD Pipeline

## Description
Setup continuous integration and continuous deployment pipeline using GitHub Actions.

## Requirements
- Create `.github/workflows/` directory
- Implement workflows:
  - `build-and-test.yml` - Runs on pull requests
    - Checkout code
    - Build backend (.NET)
    - Run backend tests
    - Build frontend (React)
    - Run frontend tests
    - Report coverage
  - `docker-build.yml` - Builds and pushes Docker images
    - Build backend image
    - Build frontend image
    - Push to Docker registry
  - `deploy.yml` - Deployment workflow (optional)
- Configure:
  - Build matrix for different .NET/Node versions (optional)
  - Test result reporting
  - Code coverage reporting
  - Docker registry credentials
  - Secrets management
- Add status checks for pull requests
- Document workflow triggers and dependencies

## Acceptance Criteria
- Workflows execute successfully
- Tests run on pull requests
- Docker images are built and pushed
- Pipeline reports test results and coverage
- Workflows are properly documented
