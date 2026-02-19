# Copilot Instructions: Skill Extraction Tool

## Architecture Overview

This is a **Clean Architecture + CQRS** application with strict layer dependencies flowing inward:

```
API → Infrastructure → Application → Domain
```

**Never** reference outer layers from inner layers. Domain has no dependencies, Application depends only on Domain, etc.

## Project Structure

### Backend (.NET 10)
- `SkillExtraction.Domain/` - Entities only (User). No logic, no dependencies.
- `SkillExtraction.Application/` - Commands, Queries, Handlers, DTOs, Validators, Interfaces
- `SkillExtraction.Infrastructure/` - Concrete implementations (repositories, text extraction, Excel export)
- `SkillExtraction.Api/` - Controllers, API models, Swagger config
- `SkillExtraction.Tests.Unit/` - 92 unit tests with NSubstitute mocks
- `SkillExtraction.Tests.Integration/` - 20 integration tests with WebApplicationFactory

### Frontend (Angular 18)
- Standalone components (no NgModules)
- `src/app/core/services/` - AuthService, SkillsService with BehaviorSubject state
- `src/app/core/guards/` - JWT auth guard
- `src/app/core/interceptors/` - Automatic JWT header injection

## Key Patterns & Conventions

### 1. Commands and Queries (CQRS)
All operations use MediatR:
```csharp
// Command structure (in Application layer)
public sealed record SignUpCommand(string Username, string Password) : IRequest<AuthTokenDto>;

// Handler
public class SignUpCommandHandler : IRequestHandler<SignUpCommand, AuthTokenDto>
{
    public SignUpCommandHandler(IUserRepository repo, IPasswordHasher hasher, ITokenService token) { }
    public async Task<AuthTokenDto> Handle(SignUpCommand request, CancellationToken ct) { }
}

// Register in DependencyInjection.cs via assembly scanning
```

### 2. Validation
Every command has a FluentValidation validator:
```csharp
public class SignUpCommandValidator : AbstractValidator<SignUpCommand>
{
    public SignUpCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3).Matches("^[a-zA-Z0-9_-]+$");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}
```
Validators run automatically via `ValidationBehavior<TRequest, TResponse>` pipeline behavior.

### 3. DTO Boundaries
**Never expose Domain entities to API**:
```
HTTP Request → API Model → Command → Handler → Domain Entity → Result DTO → HTTP Response
```
Example: `ExtractSkillsRequest` (API) → `ExtractSkillsCommand` (Application) → `ExtractSkillsDto` (Application)

### 4. Repository Pattern
Only `IUserRepository` has persistence (SQLite). Skills use in-memory dictionary (`InMemorySkillDictionary`).
Configuration in `appsettings.json` controls provider:
```json
{
  "DatabaseSettings": {
    "Provider": "Sqlite",  // or "InMemory" for tests
    "ConnectionString": "Data Source=skillextraction.db"
  }
}
```

### 5. Testing Patterns
**Unit Tests** (with NSubstitute):
```csharp
_userRepository.GetByUsernameAsync(username, Arg.Any<CancellationToken>()).Returns(user);
_passwordHasher.VerifyPassword(password, hash).Returns(true);
```

**Integration Tests** (with WebApplicationFactory):
```csharp
var factory = new CustomWebApplicationFactory(); // Uses InMemory storage
var client = factory.CreateClient();
var response = await client.PostAsJsonAsync("/api/auth/signup", command);
```

### 6. Exception Handling Pattern
**Custom Domain Exceptions** (in Application/Exceptions/):
```csharp
public class UserAlreadyExistsException : Exception
{
    public string Username { get; }
    public UserAlreadyExistsException(string username) 
        : base($"Username '{username}' is already taken.") 
    { Username = username; }
}
```

**Global Exception Filter** handles all exceptions centrally:
- Registered in `Program.cs`: `builder.Services.AddControllers(options => options.Filters.Add<GlobalExceptionFilter>());`
- Maps domain exceptions to HTTP status codes (409 Conflict, 401 Unauthorized, etc.)
- Controllers don't need try-catch blocks - just throw domain exceptions
- Structured error responses with type, title, message

**Constructor Null Guards** (defensive programming):
```csharp
public SignUpCommandHandler(IUserRepository repository)
{
    _repository = repository ?? throw new ArgumentNullException(nameof(repository));
}
```

## Common Development Tasks

### Adding a New Feature (e.g., "Get User Profile")

1. **Domain** (if needed): Create/modify entity
2. **Application**: 
   - Create `GetUserProfileQuery.cs` with `IRequest<UserProfileDto>`
   - Create `GetUserProfileQueryHandler.cs` implementing `IRequestHandler<,>`
   - Create `UserProfileDto` record
   - Create `GetUserProfileQueryValidator.cs` if needed
3. **Infrastructure**: Implement any new interfaces (e.g., `IProfileRepository`)
4. **API**: Create endpoint in appropriate controller
5. **DI**: Register new services in `DependencyInjection.cs`
6. **Tests**: Unit tests for handler, integration tests for endpoint

### Running the Application

**Backend**:
```powershell
cd backend/SkillExtraction.Api
dotnet run  # Runs on http://localhost:5080
```

**Frontend**:
```powershell
cd frontend/skill-extraction-ui
npm start  # Runs on http://localhost:4200
```

**Tests**:
```powershell
cd backend
dotnet test                                    # All tests
dotnet test SkillExtraction.Tests.Unit         # Unit only
dotnet test SkillExtraction.Tests.Integration  # Integration only
```

**Database Migrations** (SQLite):
```powershell
cd backend/SkillExtraction.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../SkillExtraction.Api
dotnet ef database update --startup-project ../SkillExtraction.Api
```

## Critical Files

- `backend/SkillExtraction.Application/DependencyInjection.cs` - Registers MediatR, validators, behaviors
- `backend/SkillExtraction.Infrastructure/DependencyInjection.cs` - Registers infrastructure services, DB provider selection
- `backend/SkillExtraction.Api/Program.cs` - JWT config, CORS, Swagger, auto-migrations
- `backend/SkillExtraction.Application/Behaviors/ValidationBehavior.cs` - Runs FluentValidation before handlers
- `frontend/src/app/core/services/auth.service.ts` - JWT state management with BehaviorSubject

## Data Flow Examples

### Authentication Flow
1. User signs up: `POST /api/auth/signup` → `SignUpCommand` → `SignUpCommandHandler`
2. Handler: validates username unique → hashes password → saves to repository → generates JWT
3. Returns: `AuthTokenDto` with token, username, expiration
4. Frontend: stores JWT in localStorage, updates `currentUserSubject` BehaviorSubject

### Skill Extraction Flow
1. Upload CV: `POST /api/skills/extract` (multipart/form-data with cvFile, optional ifuFile)
2. API validates: PDF/DOCX file types only
3. Handler: extracts text → matches against dictionary → returns skills with confidence scores
4. Frontend: displays in editable table → user reviews → exports to Excel

## Important Conventions

- **Async everywhere**: All handlers and services use `async Task<T>` with `CancellationToken`
- **Records for DTOs**: Use `record` types for immutable data transfer objects
- **Constructor injection**: Always use DI, never `new` for services
- **Nullable reference types**: Enabled project-wide, use `?` appropriately
- **Namespaces match folders**: `SkillExtraction.Application.Commands.Auth.SignUp` → `Commands/Auth/SignUp/`

## Known Limitations

- Skill dictionary is in-memory (no persistence)
- Text extraction requires real PDF/DOCX files (mocked in some integration tests)
- Single-tenant application (no organization separation)
- JWT stored in localStorage (consider HttpOnly cookies for production)

## Documentation

- `README.md` - Setup instructions, API endpoints, architecture overview
- `docs/INSIGHTS.md` - Detailed development insights, AI-assisted patterns, lessons learned
- `docs/PROMPT_LOG.md` - Complete history of AI interactions during development
- `docs/IMPLEMENTATION_PLAN.md` - Phase-by-phase development tracking

---

*This codebase achieved ~95% AI-generated code through incremental development with Clean Architecture constraints.*
