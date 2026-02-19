# Skill Extraction UI

Angular frontend for the Skill Extraction Tool.

## Prerequisites

- Node.js 22.12+ or 20.19+
- npm 8.0+
- Angular CLI 19.2+

## Setup

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 18.2.21 and upgraded to 19.2.18.

### Install Dependencies

```bash
npm install
```

**Note:** If you encounter npm installation issues, ensure Node.js and npm are properly installed and configured.

### Development Server

Run the development server:

```bash
npm start
```

Or with proxy to backend API:

```bash
ng serve --proxy-config proxy.conf.json
```

Navigate to `http://localhost:4200/`. The application will automatically reload if you change any of the source files.

## Project Structure

```
src/
├── app/
│   ├── core/                 # Core services, guards, interceptors
│   │   ├── guards/          # Route guards (AuthGuard)
│   │   ├── interceptors/    # HTTP interceptors (JWT)
│   │   └── services/        # Core services (AuthService, SkillsService)
│   ├── shared/              # Shared components and models
│   │   ├── components/      # Reusable components
│   │   └── models/          # TypeScript interfaces/models
│   ├── layouts/             # Layout components
│   │   ├── main-layout/     # Main application layout
│   │   └── admin-layout/    # Admin section layout
│   └── features/            # Feature modules
│       ├── auth/            # Authentication (signup, signin)
│       ├── upload/          # File upload page
│       ├── skills-review/   # Skills review/edit page
│       └── admin/           # Admin features
│           └── dictionary/  # Skill dictionary management
```

## Routes

- `/signup` - User registration
- `/signin` - User login
- `/upload` - Upload CV and IFU files (protected)
- `/skills` - Review and edit extracted skills (protected)
- `/admin/dictionary` - Manage skill dictionary (protected)

## API Configuration

The Angular app proxies API requests to the backend server. The proxy configuration is in `proxy.conf.json`:

- API Base URL: `http://localhost:5080`
- All requests to `/api/*` are proxied to the backend

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory.

## Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.

