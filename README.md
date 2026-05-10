# Car Service Booking System API

A production-ready backend API built with ASP.NET Core and Clean Architecture for managing car service bookings, user vehicles, payments, notifications, and service operations.

---

# Overview

This project is designed as a scalable SaaS-ready backend system for car service businesses.

The API provides:

- Secure authentication and authorization
- Vehicle management
- Booking scheduling
- Stripe payment integration
- Email notifications
- Background job processing
- Caching and pagination
- Soft delete and audit tracking
- Docker support
- Automated testing and CI/CD support

---

# Tech Stack

## Backend

- ASP.NET Core Web API
- .NET 10
- Clean Architecture
- Entity Framework Core
- SQL Server

## Authentication & Security

- ASP.NET Core Identity
- JWT Authentication
- Refresh Tokens
- Role-Based Authorization

## Payments

- Stripe PaymentIntent API
- Stripe Webhooks

## Background Jobs

- Hangfire
- SMTP Email Service

## Validation & Logging

- FluentValidation
- Serilog

## Testing & DevOps

- xUnit
- Moq
- FluentAssertions
- GitHub Actions
- Docker & Docker Compose

## Documentation & Utilities

- Swagger / OpenAPI
- API Versioning
- MemoryCache

---

# Project Structure

```txt
CarServiceBookingSystem.Domain
CarServiceBookingSystem.Application
CarServiceBookingSystem.Infrastructure
CarServiceBookingSystem.API
CarServiceBookingSystem.Tests
```

---

# Architecture

The project follows Clean Architecture principles.

## Domain Layer

Contains:
- Entities
- Enums
- Core business rules

## Application Layer

Contains:
- DTOs
- Interfaces
- Validators
- Business contracts

## Infrastructure Layer

Contains:
- EF Core
- Identity
- JWT implementation
- Stripe integration
- Hangfire jobs
- Email services
- Caching
- External integrations

## API Layer

Contains:
- Controllers
- Middleware
- Filters
- Swagger configuration
- API versioning

## Tests Layer

Contains:
- Unit tests
- Validation tests
- Service tests
- Authentication tests

---

# Features

## Authentication

- User registration
- User login
- JWT access tokens
- Refresh token rotation
- Logout with token revocation
- Role-based authorization

## User Vehicles

Users can:
- Add vehicles
- Update vehicles
- Delete vehicles
- View personal vehicles

## Vehicle Lookup System

Dynamic lookup flow:

```txt
Brand → Model → Year → Trim
```

## Services Management

Admin capabilities:
- Create services
- Update services
- Delete services

Public capabilities:
- View services
- Pagination
- Filtering
- Sorting

## Booking System

Users can:
- Create bookings
- View personal bookings

Admins can:
- View all bookings
- Filter bookings
- Update booking status

## Payments

- Stripe PaymentIntent integration
- Stripe webhook processing
- Payment status synchronization
- Booking auto-confirmation after successful payment

## Email Notifications

- Booking confirmation emails
- Background email processing with Hangfire

## System Features

- Global exception handling
- Validation handling
- Memory caching
- API versioning
- Soft delete
- Audit fields
- Request logging

---

# Database Features

## Soft Delete

Entities are not permanently removed from the database.

## Audit Fields

Tracks:
- CreatedAt
- UpdatedAt
- DeletedAt
- CreatedBy
- UpdatedBy
- DeletedBy

---

# Local Development Setup

## 1. Clone Repository

```bash
git clone https://github.com/YOUR_USERNAME/CarServiceBookingSystem.git
cd CarServiceBookingSystem
```

---

## 2. Configure Database

Update:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=CarServiceBookingDb;Trusted_Connection=True;TrustServerCertificate=True"
}
```

inside:

```txt
appsettings.json
```

---

## 3. Configure JWT

```json
"JwtSettings": {
  "Secret": "YOUR_SECRET_KEY",
  "Issuer": "CarServiceBookingSystem",
  "Audience": "CarServiceBookingSystemUsers",
  "ExpiryMinutes": 60
}
```

---

## 4. Configure Stripe

```json
"StripeSettings": {
  "SecretKey": "sk_test_xxxxx",
  "WebhookSecret": "whsec_xxxxx",
  "Currency": "aed"
}
```

---

## 5. Configure Email

```json
"EmailSettings": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "FromEmail": "your-email@gmail.com",
  "FromName": "Car Service Booking",
  "Password": "your-app-password",
  "EnableSsl": true
}
```

---

## 6. Apply Database Migrations

```powershell
Update-Database `
-Project CarServiceBookingSystem.Infrastructure `
-StartupProject CarServiceBookingSystem.API `
-Context ApplicationDbContext
```

---

## 7. Run API

```bash
dotnet run --project CarServiceBookingSystem.API
```

---

## 8. Open Swagger

```txt
https://localhost:xxxx/swagger
```

---

# Docker Setup

## Run API + SQL Server

```bash
docker compose up --build
```

---

## Swagger

```txt
http://localhost:8080/swagger
```

---

## Hangfire Dashboard

```txt
http://localhost:8080/hangfire
```

---

## SQL Server Connection (SSMS)

```txt
Server: localhost,14333
Authentication: SQL Server Authentication
Login: sa
Password: YourStrong!Passw0rd
Database: CarServiceBookingDb
```

---

## Stop Containers

```bash
docker compose down
```

---

## Stop Containers and Delete Database

```bash
docker compose down -v
```

---

## Notes

- EF Core migrations run automatically on startup
- Docker SQL Server is separate from LocalDB or local SQL Server installations
- Docker uses persistent volumes for database storage

---

# Default Admin Account

```txt
Email: admin@carservice.com
Password: Admin123!
```

---

# Stripe Testing

## Start Stripe Webhook Listener

```bash
stripe listen --events payment_intent.succeeded,payment_intent.payment_failed --forward-to http://localhost:8080/api/v1/stripe-webhook
```

---

## Stripe Test Card

```txt
4242 4242 4242 4242
Any future expiry
Any 3 digits CVC
Any ZIP code
```

---

# API Endpoints

## Authentication

```txt
POST /api/v1/auth/register
POST /api/v1/auth/login
POST /api/v1/auth/refresh-token
POST /api/v1/auth/logout
```

## Car Lookups

```txt
GET /api/v1/car-lookups/brands
GET /api/v1/car-lookups/models/{brandId}
GET /api/v1/car-lookups/years/{modelId}
GET /api/v1/car-lookups/trims/{yearId}
```

## Cars

```txt
POST   /api/v1/cars
PUT    /api/v1/cars/{id}
DELETE /api/v1/cars/{id}
GET    /api/v1/cars/my-cars
```

## Services

```txt
GET    /api/v1/services
POST   /api/v1/services
PUT    /api/v1/services/{id}
DELETE /api/v1/services/{id}
```

## Bookings

```txt
POST /api/v1/bookings
GET  /api/v1/bookings/my-bookings
GET  /api/v1/bookings
PUT  /api/v1/bookings/{id}/status
```

## Payments

```txt
POST /api/v1/payments/create-intent/{bookingId}
POST /api/v1/stripe-webhook
```

---

# Unit Testing

Run tests:

```bash
dotnet test
```

Generate coverage:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

---

# CI/CD

GitHub Actions automatically:
- Restore packages
- Build solution
- Run unit tests

Workflow file:

```txt
.github/workflows/dotnet.yml
```

---

# Logging

Serilog logs are stored in:

```txt
Logs/log-yyyyMMdd.txt
```

---

# Validation

Validation is implemented using:
- FluentValidation
- Custom validation filters
- Centralized API responses

---

# Security Notes

## Never Commit

Do not commit:
- Stripe secret keys
- JWT secrets
- SMTP passwords
- Production connection strings

Use:
- User Secrets
- Environment Variables
- Azure Key Vault
- Secure secret storage

---

# Future Improvements

- SignalR real-time notifications
- Redis distributed caching
- Integration tests
- Docker production optimization
- Kubernetes deployment
- Mobile app integration
- File upload support
- Dashboard analytics
- Multi-tenant support
- Full CI/CD deployment pipeline

---

# Author

Mahmoud Almahmoud

Backend Developer (.NET / ASP.NET Core)

Abu Dhabi, UAE