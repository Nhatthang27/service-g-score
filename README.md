# G-Score API

A RESTful API service for managing and analyzing Vietnamese high school national exam scores (THPT 2024). Built with ASP.NET Core 8.0, PostgreSQL, and Clean Architecture.

## Features

- **CSV Import** - Bulk import student scores from CSV files using PostgreSQL binary COPY for high performance
- **Score Lookup** - Query individual student scores by registration number
- **Statistics** - Score distribution analysis by subject (excellent / good / average / below average)
- **Top Students** - Retrieve top 10 students in Group A (Math, Physics, Chemistry)

## Tech Stack

- .NET 8.0 / ASP.NET Core
- PostgreSQL 16
- Entity Framework Core
- MediatR (CQRS)
- FluentValidation
- Docker & Docker Compose

## Getting Started

### Prerequisites

- [Docker](https://docs.docker.com/get-docker/) and Docker Compose

### Run with Docker Compose

```bash
git clone <repository-url>
cd service-g-score
docker-compose up --build
```

The API will be available at **http://localhost:8080** and Swagger UI at **http://localhost:8080/swagger**.

EF Core migrations are applied automatically on startup.

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/students/import` | Import student scores from CSV file |
| GET | `/api/students/{registrationNumber}` | Get scores by registration number |
| GET | `/api/students/top-group-a` | Get top 10 Group A students |
| GET | `/api/statistics/score-distribution` | Get score distribution by subject |

## Project Structure

```
src/
  GScore.Domain/          # Entities and constants
  GScore.Application/     # Use cases, DTOs, interfaces (CQRS)
  GScore.Infrastructure/  # Database, EF Core, CSV import service
  GScore.Presentation/    # API controllers, middleware, configuration
```
