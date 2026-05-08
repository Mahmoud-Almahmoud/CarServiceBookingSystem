# Car Service Booking System API

A production-ready backend API built with ASP.NET Core and Clean Architecture for managing car service bookings, payments, user vehicles, and service operations.

---

# Overview

This project is a scalable and maintainable backend system designed for car service businesses and SaaS platforms.

The system provides:

- Secure authentication and authorization
- Vehicle management
- Booking scheduling
- Stripe payment integration
- Email notifications
- Background job processing
- Admin management features
- Caching and pagination
- Audit logging and soft delete support

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

## Background Processing

- Hangfire
- SMTP Email Service

## Validation & Logging

- FluentValidation
- Serilog

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
```

---

# Architecture

The project follows Clean Architecture principles:

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
- Database access
- Identity implementation
- JWT generation
- Stripe integration
- Hangfire jobs
- Email services
- Caching
- External services

## API Layer

Contains:
- Controllers
- Middleware
- Filters
- Swagger configuration
- API versioning

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

- Add vehicle
- Update vehicle
- Delete vehicle
- Get user vehicles

## Vehicle Lookup System

Dynamic dropdown flow:

```txt
Brand → Model → Year → Trim
```

## Services Management

Admin capabilities:
- Create service
- Update service
- Delete service

Public capabilities:
- View available services
- Pagination
- Filtering
- Sorting

## Booking System

Users can:
- Create booking
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
- Background email jobs using Hangfire

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

# Getting Started

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
  "AccessTokenExpirationMinutes": 60,
  "RefreshTokenExpirationDays": 7
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

# Default Admin Account

```txt
Email: admin@carservice.com
Password: Admin123!
```

---

# Stripe Testing

## Start Stripe Webhook Listener

```bash
stripe listen --events payment_intent.succeeded,payment_intent.payment_failed --forward-to https://localhost:xxxx/api/v1/stripe-webhook
```

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

# Background Jobs Dashboard

Hangfire dashboard:

```txt
https://localhost:xxxx/hangfire
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


# Author

Mahmoud Almahmoud

Backend Developer (.NET / ASP.NET Core)
Abu Dhabi, UAE