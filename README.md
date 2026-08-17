# FitRadar — Backend
 
FitRadar helps users compare gym and sports facility subscriptions available in Warsaw. It shows which facilities are covered by which subscription packages, at what price, and which ones are currently accessible to a user based on their active subscriptions. (In Progress)
 
This repository contains the backend API. The frontend (map view, comparison UI) lives in a separate repository
 
## Overview
 
Gym and sports subscription providers (e.g. multi-benefit cards, single-club memberships) often cover different, overlapping sets of facilities at different prices and access conditions. FitRadar centralizes this information so users can:
 
- See which facilities are covered by a given subscription package
- Compare packages by price and coverage
- Check which facilities are accessible to them right now, based on their own active subscriptions
## Features
 
- **JWT authentication** with short-lived access tokens and long-lived, rotating refresh tokens
- **Email verification** on registration via SendGrid (6-digit code, valid for 1 hour)
- **Google Sign-In** as an alternative to email/password login
- **Role-based authorization** (Admin / User), enforced on write endpoints — e.g. only Admins can create, update, or delete facilities
- **Facility, Package, and Provider management** exposed through a REST API
- **Swagger / OpenAPI UI** for exploring and testing endpoints directly from the browser
## Tech Stack
 
- **.NET** / ASP.NET Core Web API
- **Entity Framework Core** with SQL Server
- **ASP.NET Core Identity** for user management and password hashing
- **JWT Bearer authentication** with a custom refresh token flow
- **SendGrid** for transactional email (verification codes)
- **Google OAuth 2.0** (ID token verification) for social login
- **Swashbuckle / Swagger** for API documentation
## Architecture
 
The backend is organized as a layered API, with a clear separation between data access, business logic, and HTTP concerns:
 
```
FitRadar-backend/
├── Data/            → EF Core DbContext, Identity integration
├── Repositories/    → data-access abstractions + EF implementations
├── Services/        → business logic (facilities, packages, providers, users, auth)
├── Shared/
│   ├── Models/      → domain entities (Facility, Package, Provider, User, ...)
│   ├── DTOs/        → request/response contracts
│   └── Settings/    → strongly-typed configuration (e.g. JwtSettings)
├── Extensions/      → DI/setup extension methods (auth, Swagger, CORS, repositories, services)
├── Controllers/     → API endpoints
└── Program.cs       → app composition and middleware pipeline
```
 
 
## Getting Started
 
### Prerequisites
 
- [.NET SDK](https://dotnet.microsoft.com/download) (8 or later)
- SQL Server (LocalDB, Express, or a full instance)
- A SendGrid account + API key (for sending verification emails)
- (Optional) A Google OAuth Client ID, if you want to test Google Sign-In
### Setup
 
1. Clone the repository:
```bash
   git clone https://github.com/<your-username>/FitRadar-backend.git
   cd FitRadar-backend
```
 
2. Configure `appsettings.json` (or `appsettings.Development.json`):
```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.;Database=FitRadar;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;"
     },
     "JwtSettings": {
       "Issuer": "FitRadar",
       "Audience": "FitRadarClient",
       "AccessTokenExpirationMinutes": 15,
       "RefreshTokenExpirationDays": 7
     },
     "SendGrid": {
       "FromEmail": "your-sender@example.com",
       "FromName": "FitRadar"
     }
   }
```
 
3. Configure secrets locally using [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) — these should never be committed to source control:
```bash
   dotnet user-secrets set "JwtSettings:SecretKey" "<a random string, at least 32 characters>"
   dotnet user-secrets set "SendGrid:ApiKey" "<your SendGrid API key>"
```
 
4. Apply database migrations:
```bash
   dotnet ef database update
```
 
5. Run the API:
```bash
   dotnet run
```
 
6. Open Swagger UI at `https://localhost:<port>/swagger` to explore and test the available endpoints. Default `Admin` and `User` roles are seeded automatically on first run.
