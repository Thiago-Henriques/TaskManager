# TaskManager

A robust task management API built with .NET 9, following Clean Architecture principles and secure JWT authentication. This project is designed for scalability, maintainability, and testability.

## Technologies Used

- **.NET 9**: Modern, cross-platform framework for building high-performance applications.
- **ASP.NET Core Web API**: Framework for building RESTful APIs.
- **JWT Authentication**: Secure token-based authentication for API endpoints.
- **Swagger/OpenAPI**: Interactive API documentation and testing interface.
- **Serilog**: Structured logging to console and files.
- **xUnit**: Unit and integration testing framework.

## Solution Structure & Architecture

The solution is organized using Clean Architecture, separating concerns into distinct layers:

- **API Layer (`TaskManager.Api`)**: Handles HTTP requests, responses, and API configuration. Controllers do not access repositories directly, but interact with services via interfaces.
- **Application Layer (`TaskManager.Application`)**: Contains business logic, service implementations, and interfaces. Responsible for orchestrating domain operations and enforcing application rules.
- **Domain Layer (`TaskManager.Domain`)**: Defines core entities, value objects, and domain logic. Contains business rules and models.
- **Infrastructure Layer (`TaskManager.Infrastructure`)**: Implements data access, repositories, and external service integrations.
- **Tests (`TaskManager.Tests`)**: Contains unit and integration tests for controllers, services, and repositories.

### Dependency Inversion & Best Practices
- Controllers depend on services via interfaces, not directly on repositories.
- Dependency Injection is used throughout the solution for loose coupling.
- ActionResult is adopted for controller responses, improving flexibility and error handling.
- Services encapsulate business logic and coordinate between controllers and repositories.

### Database Design
- Indices on tables are optimized for query performance.
- Task status is normalized into a separate table, rather than using a raw integer field, improving data integrity and extensibility.

## Setup & Running

1. **Install .NET 9 SDK**
2. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/TaskManager.git
   cd TaskManager
   ```
3. **Configure the database connection**
   - Update the connection string in `src/TaskManager.Api/appsettings.json` as needed.
4. **Start the API**
   ```bash
   dotnet run --project src/TaskManager.Api
   ```
5. **Access the API and documentation**
   - Swagger UI: [https://localhost:5001/swagger](https://localhost:5001/swagger)
   - API Endpoints: [https://localhost:5001/api](https://localhost:5001/api)

## Authentication

- The API uses JWT tokens for authentication.
- To access protected endpoints:
  1. Register a new user via the API.
  2. Login to receive a JWT token.
  3. Include the token in the `Authorization` header as `Bearer {token}`.

## Testing

- Run all tests:
  ```bash
  dotnet test
  ```
- Tests cover controllers, services, and repository logic using xUnit and Moq.

## Logging

- Serilog is configured for console and file logging.
- Logs are stored in the `logs` directory with daily rolling files.

## Development Process & Decisions

This project was developed through a collaborative and iterative process with AI assistance:

1. **Architecture Discussion**: Debated project architecture with AI, exploring strategies and best practices for maintainability and scalability.
2. **Layer & Project Planning**: Decided on Clean Architecture, defining solution layers and individual projects.
3. **Initial Setup**: Created the solution, projects, and initial files for each layer.
4. **Iterative Development**: Generated files one by one, reviewing and refining AI-generated code for correctness and alignment with architectural goals.
5. **Dependency Inversion**: Adjusted code to ensure controllers do not access repositories directly, introduced interfaces and service layers for proper separation of concerns.
6. **Controller Improvements**: Adopted `ActionResult` for controller responses, improving error handling and API flexibility.
7. **Database Adjustments**: Optimized table indices and normalized the status field into a dedicated table for better data integrity.
