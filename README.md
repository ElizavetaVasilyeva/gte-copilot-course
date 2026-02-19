# Skill Extraction Tool

A complete web application for extracting and managing technical skills from CV documents, built with Clean Architecture principles and CQRS pattern.

## 🎯 Overview

This application allows HR teams and recruiters to:
- Upload CV documents (PDF/DOCX) with optional IFU files
- Automatically extract technical skills using dictionary-based matching
- Review and edit extracted skills in an interactive table
- Export results to Excel for further processing
- Browse the complete skill dictionary (85+ predefined skills)

## 🏗️ Architecture

### Backend (.NET 10)
- **Clean Architecture** with clear layer separation
- **CQRS** pattern using MediatR
- **FluentValidation** for request validation
- **JWT Authentication** for secure API access
- **RESTful API** with Swagger documentation
- **SQLite Persistence** with EF Core (auto-migrations)
- **Global Exception Filter** for consistent error handling
- **Custom Domain Exceptions** for type-safe error management
- **Comprehensive Testing** (92 unit tests + 20 integration tests)

**Project Structure:**
```
backend/
├── SkillExtraction.Domain/          # Entities and value objects
├── SkillExtraction.Application/     # Commands, Queries, DTOs
├── SkillExtraction.Infrastructure/  # External services, persistence
├── SkillExtraction.Api/             # Web API endpoints
├── SkillExtraction.Tests.Unit/      # 92 unit tests (100% passing)
└── SkillExtraction.Tests.Integration/ # 20 integration tests + 3 skipped
```

### Frontend (Angular 18)
- **Standalone components** with functional routing
- **RxJS BehaviorSubject** for state management
- **Reactive forms** with validation
- **JWT storage** in localStorage
- **HTTP interceptor** for authentication

**Project Structure:**
```
frontend/skill-extraction-ui/
├── src/app/
│   ├── core/                # Services, guards, models
│   │   ├── guards/         # Auth guard
│   │   ├── interceptors/   # JWT interceptor
│   │   ├── models/         # TypeScript interfaces
│   │   └── services/       # HTTP services, state management
│   └── features/           # Feature modules
│       ├── auth/           # Sign up, Sign in
│       ├── upload/         # File upload
│       ├── skills-review/  # Interactive skills table
│       └── admin/          # Dictionary browser
```

## 🚀 Getting Started

### Prerequisites
- **.NET 10 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/10.0)
- **Node.js 22.x** - [Download](https://nodejs.org/)
- **npm 10.x** or higher

### Backend Setup

1. Navigate to backend directory:
```powershell
cd backend
```

2. Restore dependencies and build:
```powershell
dotnet restore
dotnet build
```

3. Run the API (default port: 5080):
```powershell
cd SkillExtraction.Api
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5080`
- Swagger UI: `http://localhost:5080/swagger`

### Frontend Setup

1. Navigate to frontend directory:
```powershell
cd frontend\skill-extraction-ui
```

2. Install dependencies:
```powershell
npm install
```

3. Start the development server:
```powershell
npm start
```

The application will be available at `http://localhost:4200`

## 📖 User Guide

### 1. Authentication
- **Sign Up**: Create a new account with username and password
- **Sign In**: Log in with existing credentials
- JWT token is stored in localStorage (60-minute expiration)

### 2. Upload CV
- Navigate to **Upload** page
- Select CV file (required) - PDF or DOCX format
- Optionally select IFU file for additional context
- Click **Extract Skills** to process documents

### 3. Review Skills
- View extracted skills in interactive table
- **Edit** any field: name, category, confidence, snippet, notes
- **Add** new skills manually
- **Delete** unwanted skills
- Double-click any cell to edit
- All changes are validated before saving

### 4. Export to Excel
- Click **Export to Excel** button
- Download styled .xlsx file with:
  - All skill fields
  - Formatted confidence percentages
  - Professional styling and borders
  - Auto-fit columns

### 5. Browse Dictionary
- Navigate to **Dictionary** page
- View all 85+ predefined skills
- Filter by category:
  - Programming Languages
  - Frameworks
  - Databases
  - Cloud & DevOps
  - Methodologies
  - Testing
  - Tools
- See skill aliases used for matching

## 🔑 Key Features

### Skill Extraction
- **Text Extraction**: Supports PDF and DOCX formats
- **Dictionary Matching**: 85+ skills with multiple aliases
- **Confidence Scoring**: 95% for exact matches, 85% for aliases
- **Context Snippets**: Shows where skill was found in document
- **Categories**: Organized into 7 technical categories

### Skill Management
- **Full CRUD Operations**: Create, Read, Update, Delete
- **In-line Editing**: Double-click to edit any cell
- **Validation**: Name required, confidence 0-1, snippet required
- **State Management**: Skills preserved during navigation

### Excel Export
- **Styled Workbook**: Professional formatting
- **Formatted Confidence**: Displayed as percentage
- **Auto-fit Columns**: Optimal width for all content
- **Borders & Styling**: Clear visual structure
- **Timestamped Filename**: `skills_export_YYYYMMDD_HHMMSS.xlsx`

### User Interface
- **Responsive Design**: Works on desktop and tablet
- **Modern Styling**: Blue theme (#0066cc)
- **Smooth Animations**: Hover effects and transitions
- **Clear Navigation**: Header with Upload/Review/Dictionary links
- **Loading States**: Visual feedback for async operations
- **Error Handling**: User-friendly error messages

## 🛠️ Technology Stack

### Backend
- **.NET 10** - Latest .NET platform
- **ASP.NET Core** - Web API framework
- **MediatR 15.2.0** - CQRS implementation
- **FluentValidation 11.11.0** - Request validation
- **Entity Framework Core 10.0.0** - ORM for database access
- **SQLite** - Lightweight file-based database (default)
- **Swashbuckle 10.1.2** - OpenAPI/Swagger
- **DocumentFormat.OpenXml 3.2.0** - DOCX parsing
- **PdfPig 0.1.9** - PDF parsing
- **ClosedXML** - Excel generation
- **ASP.NET Core Identity 2.3.9** - Password hashing
- **System.IdentityModel.Tokens.Jwt 8.16.0** - JWT tokens

### Frontend
- **Angular 18.2.21** - Modern web framework
- **TypeScript 5.5** - Type-safe JavaScript
- **RxJS** - Reactive programming
- **Standalone Components** - No NgModules needed

## 📝 API Endpoints

### Authentication (No Auth Required)
- `POST /api/auth/signup` - Register new user
- `POST /api/auth/signin` - Login and get JWT token

### Skills (Auth Required)
- `POST /api/skills/extract` - Extract skills from documents (multipart/form-data)
- `POST /api/skills/export` - Export skills to Excel (returns blob)
- `GET /api/skills/dictionary` - Get complete skill dictionary

## 🔒 Security

- **JWT Authentication**: All skill endpoints protected
- **Password Hashing**: ASP.NET Core Identity with PBKDF2
- **CORS**: Configured for localhost:4200
- **Validation**: FluentValidation on all requests
- **File Validation**: Type and size checks (PDF/DOCX, 10MB max)

## 💾 Data Storage

### User Persistence

The application supports **two storage modes** for user data, configurable via `appsettings.json`:

#### 1. **SQLite** (Default in Development)
- File-based database: `skillextraction.db`
- Survives API restarts
- Automatic schema migrations via Entity Framework Core
- Production-ready for small to medium workloads
- Configuration in `appsettings.Development.json`:
  ```json
  {
    "DatabaseSettings": {
      "Provider": "Sqlite",
      "ConnectionString": "Data Source=skillextraction.db"
    }
  }
  ```

#### 2. **In-Memory** (Available for Testing)
- Stored in `ConcurrentDictionary`
- Cleared on API restart
- No database file required
- Fast for unit tests
- Configuration:
  ```json
  {
    "DatabaseSettings": {
      "Provider": "InMemory"
    }
  }
  ```

### Other Data
- **Skills Dictionary**: Preloaded singleton with 85+ skills (in-memory)
- **Extracted Skills**: Not persisted; processed and exported per request

### Database Migrations

When using SQLite, migrations are applied automatically on startup. To manage migrations manually:

```powershell
# Create a new migration
cd backend/SkillExtraction.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../SkillExtraction.Api

# Update database
dotnet ef database update --startup-project ../SkillExtraction.Api

# Remove last migration
dotnet ef migrations remove --startup-project ../SkillExtraction.Api
```

### Production Considerations

For production deployments, consider:
- **SQL Server / PostgreSQL**: Enterprise-grade relational databases
- **Azure SQL / AWS RDS**: Managed database services
- **Skills History**: Add database storage for extraction history
- **Audit Logging**: Track user actions for compliance
- **Backup Strategy**: Automated database backups

## 🧪 Testing

Comprehensive test suite with **112 total tests** covering application functionality:

### Test Coverage

**Unit Tests** (92 tests - 100% passing)

**Authentication Tests** (11 tests)
- SignUpCommandHandler: Valid signup, duplicate username, validation
- SignInCommandHandler: Valid credentials, invalid credentials, user not found

**Skill Extraction Tests** (4 tests)
- ExtractSkillsCommandHandler: CV only, CV + IFU, empty text, extraction errors

**Excel Export Tests** (7 tests)
- ExportSkillsCommandHandler: Valid export, empty list, large lists, special characters, error handling

**Validator Tests** (30 tests)
- SignUpCommandValidator:Valid inputs, empty fields, min/max length, valid formats
- SignInCommandValidator: Valid credentials, empty fields, multiple errors
- ExtractSkillsCommandValidator: Valid files, invalid extensions, missing fields, IFU validation

**Infrastructure Tests** (40 tests)
- PasswordHasherAdapter: Hash generation, verification, edge cases, special characters
- JwtTokenService: Token generation, claims validation, expiration, configuration

**Integration Tests** (20 tests passing + 3 skipped)

**Authentication Integration** (11 tests)
- SignUp: Valid registration, duplicate username, invalid input variations
- SignIn: Valid/invalid credentials, nonexistent user, complete auth flow

**Skills API Integration** (9 tests)
- Authentication: Unauthorized access prevention
- Dictionary: Authenticated dictionary retrieval
- Export: Valid export, empty skills handling
- File validation: Proper file type enforcement
- 3 skipped tests requiring real DOCX files (covered by unit tests)

### Running Tests

```powershell
cd backend

# Run all tests (unit + integration)
dotnet test

# Run only unit tests
dotnet test SkillExtraction.Tests.Unit

# Run only integration tests
dotnet test SkillExtraction.Tests.Integration
```

**Test Summary:**
- **Unit Tests**: 92 passed (100%)
- **Integration Tests**: 20 passed + 3 skipped
- **Total**: 112 tests, ~6 seconds
- **Coverage**: Commands, Validators, Infrastructure, HTTP endpoints

### Test Framework
- **xUnit 2.9.3**: Testing framework
- **NSubstitute 5.3.0**: Mocking framework
- **FluentAssertions 7.0.0**: Readable assertions
- **WebApplicationFactory**: Integration testing with in-memory server
- **coverlet.collector**: Code coverage

## 📚 Development Notes

### Clean Architecture Benefits
- **Testability**: Business logic isolated from infrastructure
- **Maintainability**: Clear separation of concerns
- **Flexibility**: Easy to swap implementations (e.g., add database)

### CQRS Pattern
- **Commands**: ExtractSkillsCommand, ExportSkillsCommand
- **Queries**: GetSkillDictionaryQuery
- **Handlers**: Single responsibility per operation
- **Validation**: Pipeline behavior for all requests

### Exception Handling Architecture
- **Global Exception Filter**: Centralized error handling for all endpoints
- **Custom Domain Exceptions**: Type-safe exceptions (UserAlreadyExistsException, InvalidCredentialsException, etc.)
- **Structured Error Responses**: Consistent JSON error format across all APIs
- **HTTP Status Mapping**: Domain exceptions automatically mapped to appropriate status codes (409 Conflict, 401 Unauthorized, etc.)
- **Fail-Fast Validation**: Constructor null guards for defensive programming

## 🎨 UI Features

### Custom File Upload
- Hidden native inputs with styled labels
- Upload icon (SVG)
- Gradient blue buttons with hover effects
- File info display with document icon and size
- Disabled state styling

### Interactive Skills Table
- Sortable columns
- Inline editing with validation
- Add/delete rows
- Edit button (✏️) per row
- Double-click to edit
- Auto-edit on add

### Dictionary Browser
- Category filter buttons
- Skill count badges
- Grid layout with responsive cards
- Alias tags
- Hover effects
- Professional card styling

### Navigation Header
- Sticky header with blue background
- Active route highlighting
- Username display
- Logout button
- Responsive layout

---

## 📚 Additional Documentation

For detailed development insights and AI interaction history:

- **[Implementation Plan](docs/IMPLEMENTATION_PLAN.md)** - Complete project roadmap and phase breakdown
- **[AI Development Insights](docs/INSIGHTS.md)** - What worked, challenges, best practices, and key learnings
- **[Prompt Log](docs/PROMPT_LOG.md)** - Detailed history of AI interactions and prompts used

### Project Achievements

✅ **Clean Architecture** - Clear separation of concerns across 4 layers  
✅ **CQRS Pattern** - Commands and queries with MediatR  
✅ **Global Exception Handling** - Custom domain exceptions with centralized error filter  
✅ **90%+ AI-Generated** - Approximately 95% of code generated by AI tools  
✅ **Comprehensive Tests** - 112 tests (92 unit + 20 integration) with 100% pass rate  
✅ **Production-Ready** - SQLite persistence, HTTPS enforcement, security hardening  
✅ **Modern Stack** - .NET 10, Angular 18, latest libraries  
✅ **Full Documentation** - README, insights, and prompt log
