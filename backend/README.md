# Backend API

This folder contains the ASP.NET Core API for the leave management application.

## Prerequisites

Install the following tools:

- .NET 10 SDK
- Docker Desktop with Docker Compose
- Entity Framework Core CLI

Install the EF CLI if it is not already available:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.11
```

Verify the installation:

```powershell
dotnet --version
dotnet ef --version
```

## Start SQL Server

From the repository root, start the SQL Server container:

```powershell
docker compose up -d
```

The container uses these development settings:

```text
Server: localhost,1433
Database: myappdb
Username: SA
Password: MySecurePassword123!
Container: local-mssql
```

Check the container status:

```powershell
docker compose ps
```

To stop the database without deleting its data:

```powershell
docker compose stop
```

The database data is persisted in the `mssql_data` Docker volume.

## Apply Database Migrations

From the repository root:

```powershell
dotnet ef database update `
  --project .\backend\Api\Api.csproj `
  --startup-project .\backend\Api\Api.csproj
```

Or from the `backend/Api` directory:

```powershell
cd backend\Api
dotnet ef database update
```

The migrations create and seed:

- `Users`
- `Lookup.LeaveType`
- `LeaveApplications`

## Run the API

From the repository root:

```powershell
dotnet run --project .\backend\Api\Api.csproj
```

Or from the `backend/Api` directory:

```powershell
cd backend\Api
dotnet run
```

The API is available at:

```text
http://localhost:5260
```

Swagger is available in Development mode at:

```text
http://localhost:5260/swagger
```

To stop the API, press `Ctrl+C` in the terminal running it.

## Authentication

Protected endpoints require a JWT bearer token.

First, authenticate through:

```http
POST http://localhost:5260/api/auth/login
Content-Type: application/json

{
  "username": "jdoe",
  "password": "ChangeMe123!"
}
```

The seeded development users use the same password. Available usernames include:

```text
jdoe
jsmith
msantos
rhale
pnair
dkim
```

Use the returned token on protected requests:

```http
Authorization: Bearer <access-token>
```

Only active users can authenticate.

## Main Endpoints

```text
POST /api/auth/login
GET  /api/Lookup/Users
GET  /api/Lookup/leave-types
GET  /api/leave-applications
POST /api/leave-applications
GET  /api/leave-applications/{id}
```

The leave applications list supports optional pagination:

```text
GET /api/leave-applications?page=1&pageSize=20
```

When `page` and `pageSize` are omitted, all applications are returned. If pagination is used, both values must be positive and supplied together.

## Configuration

Development configuration is stored in `Api/appsettings.json`.

Important settings include:

- `ConnectionStrings:DefaultConnection`: SQL Server connection string
- `Jwt:Key`: JWT signing key
- `Jwt:Issuer`: JWT issuer
- `Jwt:Audience`: JWT audience
- `Jwt:ExpirationMinutes`: token lifetime
- `PublicHolidays`: dates excluded from leave start/return calculations

Example holiday configuration:

```json
"PublicHolidays": [
  "2026-12-25",
  "2026-12-26"
]
```

Do not use the development database password, JWT key, or seeded password outside local development. Store production secrets in environment variables or a secret manager.

## Troubleshooting

### Port 5260 is already in use

Find and stop the process listening on the API port:

```powershell
Get-NetTCPConnection -LocalPort 5260 -State Listen
```

Then stop the process using its `OwningProcess` ID:

```powershell
Stop-Process -Id <process-id> -Force
```

### Database connection fails

Check that the container is running and healthy:

```powershell
docker compose ps
docker logs local-mssql
```

### Protected endpoint returns 401

Authenticate first, copy the `accessToken` from the login response, and send it as a bearer token. Also confirm that the user is active and that the token has not expired.

### Developer Notes

Should Sick leaves be excluded from the "Minimum Advance Notice" rule? As a sick person couldnt possible file a sick leave 2 days in advance

If holidays only populated dates for the current calendar year and the leave application ends on December 31st, any non-weekend days in January will be treated as working days (unless added to holidays). This won't cause a loop issue, but it could produce incorrect leave calculations across new year boundaries. Code could be refactored to exclude years on the checking of the holiday date.

Other CRUD operations are already in place. Just take care into using entities retrieved as majority (if not all) of the retrievals have `AsNoTracking()` enabled.

Pagination is enabled for the application to scale. Currently defaulted to retrieving all of the results. Optional parameters are available if pagination is implemented.