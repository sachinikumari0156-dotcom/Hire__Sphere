# HireSphere - AI-Powered Recruitment Platform

A comprehensive full-stack recruitment platform built with .NET 10 and React, featuring AI-powered job matching, interview scheduling, and analytics.

## Architecture Overview

### Backend (HireSphere.Api)
- **Framework**: ASP.NET Core 10.0
- **Database**: SQLite with Entity Framework Core
- **Authentication**: JWT (JSON Web Tokens)
- **API Pattern**: RESTful

### Frontend (HireSphere.Client)
- **Framework**: React 18 with TypeScript
- **Styling**: Tailwind CSS
- **State Management**: Zustand
- **Routing**: React Router v6
- **Build Tool**: Vite

## Project Structure

### Backend

```
HireSphere.Api/
├── Common/
│   └── ApiResponse.cs          # Generic API response wrapper
├── Data/
│   ├── Entities/               # Domain models
│   │   ├── Identity.cs         # User, Role
│   │   ├── Profile.cs          # CandidateProfile, RecruiterProfile
│   │   ├── Company.cs          # Company entity
│   │   ├── JobAndSkill.cs      # Job, Skill, JobSkill, CandidateSkill
│   │   ├── ApplicationAndInterview.cs  # Application, Interview, Resume
│   │   └── NotificationAndAudit.cs    # Notification, AuditLog
│   ├── DatabaseSeeder.cs       # Sample data initialization
│   └── HireSphereDbContext.cs  # EF Core DbContext
├── Features/
│   ├── Auth/                   # Authentication feature
│   │   ├── Controllers/
│   │   ├── Services/
│   │   └── Dtos/
│   ├── Company/                # Company management
│   ├── Job/                    # Job posting management
│   └── Application/            # Job applications
├── Middleware/
│   └── GlobalExceptionHandler.cs
├── Program.cs                  # Application startup
└── appsettings.json           # Configuration
```

### Frontend

```
HireSphere.Client/
├── src/
│   ├── components/             # Reusable React components
│   │   ├── Navbar.tsx
│   │   └── ProtectedRoute.tsx
│   ├── pages/                  # Page components
│   │   ├── LoginPage.tsx
│   │   ├── RegisterPage.tsx
│   │   ├── DashboardPage.tsx
│   │   ├── JobListingPage.tsx
│   │   └── ... (other pages)
│   ├── services/               # API integration
│   │   └── api.ts
│   ├── store/                  # State management
│   │   └── authStore.ts
│   ├── types/                  # TypeScript interfaces
│   │   └── index.ts
│   ├── App.tsx
│   ├── main.tsx
│   └── index.css
├── index.html
├── package.json
├── tsconfig.json
├── vite.config.ts
└── tailwind.config.js
```

## Key Features

### Authentication & Authorization
- User registration (Admin, Recruiter, Candidate)
- JWT-based authentication
- Role-based access control

### Company Management
- Create and manage company profiles
- Associate recruiters with companies
- View company details and job postings

### Job Management
- Post job vacancies
- Define required skills
- Track applications
- Close/reopen positions

### Candidate Features
- Create candidate profiles
- Upload resume/CV
- Search and filter jobs
- Apply for positions
- Track application status
- View interview feedback

### Recruiter Features
- Manage company profile
- Post and manage job listings
- Review applicants
- Schedule interviews
- Provide feedback

### Admin Features
- User management
- Company moderation
- System analytics
- Audit logs

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `POST /api/auth/verify-token` - Token verification

### Companies
- `GET /api/company` - List all companies
- `GET /api/company/{id}` - Get company details
- `POST /api/company` - Create company [Admin, Recruiter]
- `PUT /api/company/{id}` - Update company [Admin, Recruiter]
- `DELETE /api/company/{id}` - Delete company [Admin]

### Jobs
- `GET /api/job` - List jobs (with optional company filter)
- `GET /api/job/{id}` - Get job details
- `GET /api/job/active` - List active jobs
- `POST /api/job` - Post new job [Admin, Recruiter]
- `PUT /api/job/{id}` - Update job [Admin, Recruiter]
- `POST /api/job/{id}/close` - Close job [Admin, Recruiter]
- `DELETE /api/job/{id}` - Delete job [Admin]

### Applications
- `GET /api/application/{id}` - Get application details [Authenticated]
- `GET /api/application/job/{jobId}` - Job applications [Admin, Recruiter]
- `GET /api/application/candidate/{candidateId}` - Candidate applications [Admin, Candidate]
- `POST /api/application` - Submit application [Candidate]
- `PATCH /api/application/{id}/status` - Update status [Admin, Recruiter]
- `POST /api/application/{id}/withdraw` - Withdraw application [Candidate]
- `DELETE /api/application/{id}` - Delete application [Admin]

## Setup Instructions

### Backend Setup

1. Navigate to the backend directory:
```bash
cd to the project
```

2. Restore NuGet packages:
```bash
dotnet restore
```

3. Build the solution:
```bash
dotnet build
```

4. Run the API:
```bash
dotnet run --project HireSphere.Api
```

The API will start on `http://localhost:5000`

### Frontend Setup

1. Navigate to the frontend directory:
```bash
cd HireSphere.Client
```

2. Install dependencies:
```bash
npm install
```

3. Start development server:
```bash
npm run dev
```

The frontend will start on `http://localhost:5173`

## Database Schema

### Core Entities
- **User** - System users with roles
- **Role** - User roles (Admin, Recruiter, Candidate)
- **Company** - Organization profiles
- **Job** - Job postings
- **Skill** - Skills database
- **CandidateProfile** - Candidate information
- **RecruiterProfile** - Recruiter details
- **Application** - Job applications with matching scores
- **Interview** - Interview scheduling
- **Resume** - Candidate resumes/CVs
- **Notification** - User notifications
- **AuditLog** - System audit trail

### Relationships
- Company → Jobs (1:many)
- Job → Applications (1:many)
- Candidate → Applications (1:many)
- Application → Interviews (1:many)
- User ↔ Role (many:1)
- User ↔ CandidateProfile (1:1)
- User ↔ RecruiterProfile (1:1)
- Job ↔ Skills (many:many via JobSkill)
- Candidate ↔ Skills (many:many via CandidateSkill)

## Environment Configuration

### Backend (appsettings.json)
```json
{
  "Jwt": {
	"SecretKey": "your-secret-key-at-least-32-characters",
	"Issuer": "HireSphereAPI",
	"Audience": "HireSphereClients",
	"ExpirationMinutes": 60
  },
  "ConnectionStrings": {
	"DefaultConnection": "Data Source=hiresphere.db"
  }
}
```

### Frontend Environment
Update `vite.config.ts` to set API base URL:
```typescript
server: {
  proxy: {
	'/api': {
	  target: 'http://localhost:5000',
	  changeOrigin: true
	}
  }
}
```

## Development Workflow

### Adding New Features

1. **Backend**
   - Create DTOs in `Features/{Feature}/Dtos/{Feature}Dtos.cs`
   - Implement Service interface in `Features/{Feature}/Services/{Feature}Service.cs`
   - Create Controller in `Features/{Feature}/Controllers/{Feature}Controller.cs`
   - Register service in `Program.cs`

2. **Frontend**
   - Add API methods in `src/services/api.ts`
   - Create page components in `src/pages/`
   - Add routes in `src/App.tsx`
   - Update types if needed in `src/types/index.ts`

## Security Considerations

- JWT tokens stored in localStorage (client-side)
- Password hashing using SHA256
- Role-based authorization on all protected endpoints
- CORS configured for frontend domain only
- SQL injection prevention via EF Core parameterized queries

## Performance Optimizations

- Database indexing on frequently queried fields
- Lazy loading of related entities
- Response DTOs to avoid N+1 queries
- Frontend pagination for large lists
- Zustand for efficient state management

## Testing

### Backend Testing
```bash
# Run unit tests (to be implemented)
dotnet test
```

### Frontend Testing
```bash
# Run component tests (to be implemented)
npm run test
```

## Deployment

### Backend Deployment
```bash
dotnet publish -c Release
# Deploy to Azure App Service or similar
```

### Frontend Deployment
```bash
npm run build
# Deploy dist folder to CDN or static hosting
```

## Next Steps

1. Implement AI-powered job matching algorithm
2. Add real-time notifications using SignalR
3. Implement interview video integration
4. Build analytics dashboard
5. Add interview feedback system
6. Implement resume parsing
7. Complete UI implementations for all pages
8. Add comprehensive testing coverage
9. Performance profiling and optimization
10. Production deployment

## Contributing

1. Create a feature branch
2. Implement changes following the existing patterns
3. Test functionality
4. Submit pull request

## License

Proprietary - HireSphere

## Support

For issues and questions, contact the development team.
