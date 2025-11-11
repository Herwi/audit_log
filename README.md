# Audit Log

Application for tracking and displaying user actions within organizations based on audit log entries.

## Prerequisites

- .NET 9.0 SDK
- Node.js 18+ and npm
- PostgreSQL database

## Database Setup

Update the connection string in `AuditLog/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=RekrutacjaDb;Username=your_user;Password=your_password"
}
```

## Running Locally

### Backend API

```bash
cd AuditLog
dotnet run --launch-profile https
```

API will be available at:
- HTTPS: `https://localhost:7041`
- HTTP: `http://localhost:5292`

### Frontend

```bash
cd AuditLog.Client
npm install
npm run dev
```

Frontend will be available at: `http://localhost:5173`

## Running Tests

```bash
dotnet test AuditLog.SystemTests
```

Tests use Testcontainers with PostgreSQL and run sequentially to avoid database conflicts.

## API Endpoints

- `GET /api/v1/organizations` - Get distinct organization IDs
- `GET /api/v1/organizations/{organizationId}/user-actions?page=1&pageSize=10` - Get paginated user actions with metadata
- `GET /api/v1/organizations/{organizationId}/user-actions/{userActionId}/audit-logs` - Get detailed audit log entries for a specific user action
