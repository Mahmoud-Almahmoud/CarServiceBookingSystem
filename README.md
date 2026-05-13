# Car Service Booking System API

Production-ready backend API built with ASP.NET Core and Clean Architecture for managing car service bookings, vehicles, authentication, payments, notifications, and background processing.

---

# Features

## Authentication & Authorization

- ASP.NET Core Identity
- JWT Authentication
- Refresh Tokens
- Role-Based Authorization
- Secure Protected Endpoints

---

## Vehicle Management

- Add vehicles
- Update vehicles
- Delete vehicles
- View user vehicles

Dynamic lookup hierarchy:

```txt
Brand → Model → Year → Trim
```

---

## Services Management

- Create services
- Update services
- Delete services
- Pagination
- Filtering
- Search
- Sorting

---

## Booking System

- Create bookings
- User bookings
- Admin bookings management
- Booking status updates

---

## Stripe Payments

- Stripe PaymentIntent integration
- Stripe Webhooks
- Payment confirmation
- Booking payment synchronization

---

## Background Jobs

- Hangfire integration
- Background email sending
- Dashboard monitoring

---

## System Features

- Clean Architecture
- Global Exception Handling
- Validation Pipeline
- Soft Delete
- Audit Fields
- Memory Caching
- Response Compression
- Rate Limiting
- Health Checks
- Docker Support
- Integration Testing
- GitHub Actions CI/CD
- Serilog Logging

---

# Architecture

Project follows Clean Architecture principles.

```txt
CarServiceBookingSystem.Domain
CarServiceBookingSystem.Application
CarServiceBookingSystem.Infrastructure
CarServiceBookingSystem.API
CarServiceBookingSystem.Tests
CarServiceBookingSystem.IntegrationTests
```

---

# Tech Stack

## Backend

- ASP.NET Core Web API
- .NET 10
- Entity Framework Core
- SQL Server

## Security

- ASP.NET Core Identity
- JWT Bearer Authentication

## Payments

- Stripe.NET

## Background Jobs

- Hangfire

## Validation

- FluentValidation

## Logging

- Serilog

## Testing

- xUnit
- FluentAssertions
- Moq

## DevOps

- Docker
- Docker Compose
- GitHub Actions

---

# Project Structure

## Domain Layer

Contains:
- Entities
- Enums
- Core business logic

---

## Application Layer

Contains:
- DTOs
- Interfaces
- Validators
- Business contracts

---

## Infrastructure Layer

Contains:
- EF Core
- Identity
- JWT
- Stripe
- Hangfire
- Email services
- External integrations

---

## API Layer

Contains:
- Controllers
- Middleware
- Filters
- Swagger configuration
- Versioning
- Rate limiting
- Health checks

---

## Testing Layers

### Unit Tests

- Service tests
- Validation tests
- Authentication tests

### Integration Tests

- Authentication endpoints
- Protected endpoints
- Bookings
- Services
- Car lookups

---

# API Features

## Authentication

```txt
POST /api/v1/auth/register
POST /api/v1/auth/login
POST /api/v1/auth/refresh-token
POST /api/v1/auth/logout
```

---

## Profile

```txt
GET /api/v1/profile/me
```

---

## Car Lookups

```txt
GET /api/v1/car-lookups/brands
GET /api/v1/car-lookups/models/{brandId}
GET /api/v1/car-lookups/years/{modelId}
GET /api/v1/car-lookups/trims/{yearId}
```

---

## Cars

```txt
POST   /api/v1/cars
PUT    /api/v1/cars/{id}
DELETE /api/v1/cars/{id}
GET    /api/v1/cars/my-cars
```

---

## Services

```txt
GET    /api/v1/services
POST   /api/v1/services
PUT    /api/v1/services/{id}
DELETE /api/v1/services/{id}
```

---

## Bookings

```txt
POST /api/v1/bookings
GET  /api/v1/bookings/my-bookings
GET  /api/v1/bookings
PUT  /api/v1/bookings/{id}/status
```

---

## Payments

```txt
POST /api/v1/payments/create-intent/{bookingId}
POST /api/v1/stripe-webhook
```

---

# Security Features

## JWT Authentication

- Secure token generation
- Refresh token rotation
- Protected endpoints

---

## Rate Limiting

Global fixed-window rate limiting:

```txt
100 requests per minute per IP
```

Protects against:
- abuse
- brute force attacks
- excessive traffic

---

## Security Headers

Includes:
- X-Frame-Options
- X-Content-Type-Options
- Referrer-Policy
- HSTS

---

## Forwarded Headers Support

Supports:
- reverse proxies
- Nginx
- Cloudflare
- Docker deployments

---

# Database Features

## Soft Delete

Entities are not permanently removed.

---

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

## 2. Configure appsettings

### appsettings.Development.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=CarServiceBookingDb;Trusted_Connection=True;TrustServerCertificate=True"
  },

  "JwtSettings": {
    "Secret": "YOUR_SECRET_KEY",
    "Issuer": "CarServiceBookingSystem",
    "Audience": "CarServiceBookingSystemUsers",
    "ExpiryMinutes": 60
  },

  "StripeSettings": {
    "SecretKey": "sk_test_xxx",
    "WebhookSecret": "whsec_xxx",
    "Currency": "aed"
  },

  "EmailSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "FromEmail": "your-email@gmail.com",
    "FromName": "Car Service Booking",
    "Password": "your-password",
    "EnableSsl": true
  }
}
```

---

## 3. Apply Migrations

```powershell
Update-Database `
-Project CarServiceBookingSystem.Infrastructure `
-StartupProject CarServiceBookingSystem.API `
-Context ApplicationDbContext
```

---

## 4. Run API

```bash
dotnet run --project CarServiceBookingSystem.API
```

---

## 5. Swagger

```txt
https://localhost:xxxx/swagger
```

---

# Docker Setup

## Environment Variables

Create:

```txt
.env
```

Example:

```env
SA_PASSWORD=YourStrong!Passw0rd
JWT_SECRET=YOUR_LONG_SECRET
STRIPE_SECRET_KEY=sk_test_xxx
STRIPE_WEBHOOK_SECRET=whsec_xxx
EMAIL_FROM=your-email@gmail.com
EMAIL_PASSWORD=your-password
```

---

## Run Docker

```bash
docker compose up --build
```

---

## Swagger

```txt
http://localhost:8080/swagger
```

---

## Health Check

```txt
http://localhost:8080/health
```

---

## Hangfire Dashboard

```txt
http://localhost:8080/hangfire
```

---

## SQL Server Access

Connect using SSMS:

```txt
Server: localhost,14333
Authentication: SQL Server Authentication
Login: sa
Password: YourStrong!Passw0rd
```

---

## Stop Containers

```bash
docker compose down
```

---

## Delete Database Volume

```bash
docker compose down -v
```

---

# Production Docker

Run production compose:

```bash
docker compose -f docker-compose.prod.yml up --build -d
```

Stop:

```bash
docker compose -f docker-compose.prod.yml down
```

---

# Health Checks

Built-in health endpoint:

```txt
GET /health
```

Supports:
- Docker health checks
- load balancers
- cloud monitoring

---

# Response Compression

Enabled using ASP.NET Core response compression middleware.

Benefits:
- smaller JSON payloads
- faster API responses
- lower bandwidth usage

---

# Testing

## Unit Tests

Run:

```bash
dotnet test CarServiceBookingSystem.Tests
```

---

## Integration Tests

Run:

```bash
dotnet test CarServiceBookingSystem.IntegrationTests
```

---

## Code Coverage

```bash
dotnet test --collect:"XPlat Code Coverage"
```

---

# Postman Collection

Import:

```txt
postman_collection.json
```

into Postman.

Includes:
- authentication
- bookings
- services
- cars
- payments

---

# Stripe Testing

## Start Webhook Listener

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

# CI/CD

GitHub Actions automatically:

- restore dependencies
- build solution
- run unit tests
- run integration tests
- verify Docker build

Workflow:

```txt
.github/workflows/dotnet.yml
```

---

# Logging

Serilog logs stored in:

```txt
Logs/log-yyyyMMdd.txt
```

---

# Deployment Options

## Docker VPS

Recommended for:
- DigitalOcean
- Hetzner
- AWS EC2
- Azure VM

Run:

```bash
docker compose -f docker-compose.prod.yml up --build -d
```

---

## Azure App Service

Use:
- environment variables
- managed secrets
- SQL Server/Azure SQL

---

## Render / Railway

Simple deployment for portfolio/demo environments.

---

# Security Notes

Never commit:
- JWT secrets
- Stripe secrets
- SMTP passwords
- production connection strings

Use:
- environment variables
- Docker secrets
- cloud secret storage

---

# Future Improvements

- Redis distributed caching
- SignalR notifications
- File uploads
- Analytics dashboard
- Mobile app support
- Kubernetes deployment
- Multi-tenant architecture
- Full production CI/CD pipeline

---

# Author

Mahmoud Almahmoud

Backend Developer (.NET / ASP.NET Core)
