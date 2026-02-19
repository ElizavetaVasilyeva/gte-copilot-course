# Testing Auth Endpoints

## Running the API

From the backend directory:

```bash
cd SkillExtraction.Api
dotnet run
```

The API will start on:
- HTTP: `http://localhost:5080`
- HTTPS: `https://localhost:7080` (if configured)

Swagger UI will be available at: `http://localhost:5080/swagger`

## Endpoints

### POST /api/auth/signup
Register a new user.

**Request Body:**
```json
{
  "username": "testuser",
  "password": "password123"
}
```

**Success Response (200 OK):**
```json
{
  "token": "eyJhbGc...",
  "expiresInSeconds": 3600
}
```

**Error Responses:**
- `400 Bad Request` - Validation errors
- `409 Conflict` - Username already exists

### POST /api/auth/signin
Sign in an existing user.

**Request Body:**
```json
{
  "username": "testuser",
  "password": "password123"
}
```

**Success Response (200 OK):**
```json
{
  "token": "eyJhbGc...",
  "expiresInSeconds": 3600
}
```

**Error Responses:**
- `400 Bad Request` - Validation errors
- `401 Unauthorized` - Invalid credentials

## Testing with cURL

### Sign Up
```bash
curl -X POST http://localhost:5080/api/auth/signup \
  -H "Content-Type: application/json" \
  -d "{\"username\":\"testuser\",\"password\":\"password123\"}"
```

### Sign In
```bash
curl -X POST http://localhost:5080/api/auth/signin \
  -H "Content-Type: application/json" \
  -d "{\"username\":\"testuser\",\"password\":\"password123\"}"
```

## Testing with PowerShell

### Sign Up
```powershell
$body = @{
    username = "testuser"
    password = "password123"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5080/api/auth/signup" `
    -Method Post `
    -Body $body `
    -ContentType "application/json"
```

### Sign In
```powershell
$body = @{
    username = "testuser"
    password = "password123"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5080/api/auth/signin" `
    -Method Post `
    -Body $body `
    -ContentType "application/json"
```

## Validation Rules

### Sign Up
- **Username**: Required, 3-50 characters, alphanumeric with hyphens/underscores only
- **Password**: Required, 6-100 characters

### Sign In
- **Username**: Required
- **Password**: Required
