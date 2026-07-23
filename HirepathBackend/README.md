# HirePath AI

## AI-Powered Recruitment & Talent Management Platform

### Software Architecture – SE205.3
### NSBM Green University

---

## Project Overview

HirePath AI is an AI-powered Recruitment and Talent Management Platform developed for the Software Architecture (SE205.3) module.

The platform connects candidates, companies, recruiters, hiring managers, and platform administrators through a centralized recruitment workflow.

The system provides:

- Candidate Portal
- Company Admin Portal
- Recruiter Portal
- Hiring Manager Portal
- Platform Admin Portal
- Company Registration and Approval
- Recruiter and Hiring Manager Invitations
- Job Posting and Management
- Candidate Profile and Resume Management
- Job Application Tracking
- Interview Scheduling
- Candidate Evaluation and Feedback
- Application Status Management
- Recruitment Analytics
- AI-Powered Candidate and Job Matching
- Email and OTP Verification
- Role-Based Authentication and Authorization

The backend is developed using ASP.NET Core 8 Web API, Entity Framework Core, and SQL Server.

The frontend is developed using React and Vite.

---

## Technologies

### Backend

- ASP.NET Core 8 Web API
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- JWT Authentication
- Repository Pattern
- Service Layer
- Dependency Injection
- REST APIs
- Swagger / OpenAPI
- Cloudinary File Storage
- SMTP Email Services

### Frontend

- React
- Vite
- Axios
- React Router
- CSS
- Role-Protected Routes
- Local Storage JWT Management

### Development Tools

- Visual Studio 2022
- Visual Studio Code
- SQL Server Management Studio
- Git
- GitHub
- Swagger
- Postman
- Figma

---

## Architecture and Design Concepts

- Layered Architecture
- Repository Pattern
- Service Layer Pattern
- Dependency Injection
- REST API Architecture
- DTO Pattern
- Role-Based Authorization
- ASP.NET Identity
- Separation of Concerns
- Object-Oriented Programming
- Centralized API Communication
- Frontend Component-Based Architecture

---

## Current Architecture
React Frontend
│
▼
Axios API Services
│
▼
ASP.NET Core Controllers
│
▼
Services / Business Logic
│
▼
Repositories / Data Access
│
▼
Entity Framework Core
│
▼
SQL Server Database

text

---

## Project Progress

### Phase 01 – Database Foundation

**Completed:**

- Project Structure
- Entity Models
- Enums
- ApplicationDbContext
- Entity Relationships
- SQL Server Integration
- Entity Framework Core Migrations
- Database Creation
- Foreign Key Configuration
- Identity Tables

### Phase 02 – Authentication and Authorization

**Completed:**

- ASP.NET Core Identity
- JWT Authentication
- User Registration
- User Login
- Role Management
- Role-Based Authorization
- Protected Backend APIs
- Protected Frontend Routes
- Admin Account Seeding
- Email Verification
- OTP Verification
- Forgot Password
- Password Reset
- Invitation Account Activation
- JWT Storage and Axios Interceptor

### Phase 03 – Service Layer and API Architecture

**Completed:**

- Service Layer Implementation
- Repository-Service Architecture
- Business Logic Separation
- Dependency Injection
- DTO-Based API Communication
- Controller-Service Communication
- API Error Handling
- Code Organization Improvements
- Reusable Services
- Centralized Frontend API Configuration

### Phase 04 – Recruitment Modules

**Completed or integrated:**

- Platform Admin Management
- Company Registration and Approval
- Company Admin Dashboard
- Recruiter Invitation Management
- Hiring Manager Invitation Management
- Candidate Profile Management
- Education Management
- Experience Management
- Skills Management
- Resume Upload and Management
- Job CRUD Operations
- Active Job Search
- Candidate Job Applications
- Application Tracking
- Recruiter Candidate Pipeline
- Application Status Management
- Interview Scheduling
- Interview Feedback
- Candidate Evaluation
- Hiring Decision Workflow
- Recruitment Analytics

---

## User Roles

- SuperAdmin
- Admin
- PlatformAdmin
- CompanyAdmin
- Recruiter
- HiringManager
- Candidate

Each role has separate permissions, routes, dashboards, and workflows.

---

## User Registration and Login Flows

### Candidate Registration Flow
Candidate Registration
│
▼
Candidate enters personal information
│
▼
System sends email verification OTP
│
▼
Candidate enters OTP
│
▼
Email is verified
│
▼
Candidate account is activated
│
▼
Candidate logs in
│
▼
Redirect to Candidate Dashboard

text

**Candidate dashboard route:**

`/candidate-dashboard`

**Candidate functions:**

- Manage profile
- Add education
- Add experience
- Add skills
- Upload CV
- Set primary CV
- Search active jobs
- Apply for jobs
- Track application status
- View interview details

---

### Company Registration Flow
Company Registration
│
▼
Company representative submits registration
│
▼
Registration stored as Pending
│
▼
Platform Admin reviews request
│
├── Reject
│
└── Approve
│
▼
Company account is approved
│
▼
Company Admin account is activated
│
▼
Company Admin logs in
│
▼
Redirect to Company Admin Dashboard

text

**Company Admin dashboard route:**

`/company-admin/dashboard`

**Company Admin functions:**

- View company dashboard
- Manage company information
- Invite recruiters
- Invite hiring managers
- Activate or deactivate company members
- Manage company jobs
- View company-related recruitment data

---

### Recruiter Invitation Flow
Company Admin
│
▼
Enters recruiter name and email
│
▼
System creates invitation token
│
▼
Invitation email is sent
│
▼
Recruiter opens invitation link
│
▼
Recruiter creates username and password
│
▼
Recruiter account is activated
│
▼
Recruiter logs in
│
▼
Redirect to Recruiter Dashboard

text

**Recruiter dashboard route:**

`/recruiter-dashboard`

**Recruiter functions:**

- Create jobs
- Edit jobs
- Delete jobs
- View active jobs
- View candidate applications
- Review candidate profiles
- View and download candidate CVs
- Move applications to Under Review
- Shortlist candidates
- Reject candidates
- Schedule interviews
- Add recruiter notes
- Monitor recruitment statistics

---

### Hiring Manager Invitation Flow
Company Admin
│
▼
Enters hiring manager name and email
│
▼
System creates invitation token
│
▼
Invitation email is sent
│
▼
Hiring Manager opens invitation link
│
▼
Hiring Manager creates username and password
│
▼
Hiring Manager account is activated
│
▼
Hiring Manager logs in
│
▼
Redirect to Hiring Manager Dashboard

text

**Hiring Manager dashboard route:**

`/hiring-dashboard`

**Hiring Manager functions:**

- View shortlisted candidates
- View scheduled interviews
- Review candidate information
- Review candidate CV
- Submit interview feedback
- Evaluate candidates
- Mark candidates as interviewed
- Recommend candidates
- Reject candidates
- Make an offer
- Mark candidates as hired

---

### Platform Admin Login Flow
Platform Admin Login
│
▼
System validates email and password
│
▼
JWT token is generated
│
▼
Role is verified
│
▼
Redirect to Platform Admin Dashboard

text

**Platform Admin dashboard route:**

`/platform-admin/dashboard`

**Platform Admin functions:**

- View dashboard statistics
- Review pending company requests
- Approve companies
- Reject companies
- Suspend companies
- Activate companies
- Delete companies
- View all users
- Search and filter users
- Edit user information
- Change user roles
- Suspend or activate users
- Delete users where allowed
- View platform analytics
- Monitor jobs, applications, candidates, recruiters, and hiring managers

---

## Complete Recruitment Workflow
Company registers
│
▼
Platform Admin approves company
│
▼
Company Admin activates account
│
▼
Company Admin invites Recruiter
│
▼
Company Admin invites Hiring Manager
│
▼
Recruiter creates a job
│
▼
Job is saved in SQL Server
│
▼
Candidate views active job
│
▼
Candidate uploads CV
│
▼
Candidate applies for job
│
▼
JobApplication record is created
│
▼
Recruiter sees application
│
├── Reject Candidate
│
├── Move to Under Review
│
└── Shortlist Candidate
│
▼
Recruiter schedules interview
│
▼
Candidate sees interview details
│
▼
Hiring Manager sees interview
│
▼
Hiring Manager submits feedback
│
▼
Candidate is evaluated
│
├── Rejected
│
├── Offered
│
└── Hired
│
▼
Candidate application tracker is updated

text

---

## Application Status Flow
Applied
│
▼
UnderReview
│
├── Rejected
│
▼
Shortlisted
│
▼
InterviewScheduled
│
▼
Interviewed
│
├── Rejected
│
▼
Offered
│
├── Rejected
│
▼
Hired

text

Additional status: `Withdrawn`

The recruiter and hiring manager control application statuses according to their permissions. Candidates can view the current status through the Candidate Application Tracker.

---

## Team Responsibilities

### Member 01 – Chamodyha Peshan

**Project Lead, Architecture, Security and System Integration**

Responsible for overall project architecture, authentication, authorization, module integration, backend foundation, frontend-backend communication, and project management.

**Completed Contributions**

- Initial Project Setup
- Backend Architecture
- Database Design
- Entity Framework Core Integration
- ApplicationDbContext Configuration
- Entity Relationships
- ASP.NET Core Identity
- JWT Authentication
- User Registration
- Login API
- Email Verification
- OTP Verification
- Forgot Password
- Reset Password
- Role-Based Authorization
- Protected Routes
- Repository Pattern
- Service Layer
- Dependency Injection
- Swagger JWT Configuration
- Admin Account Seeding
- Central Axios Configuration
- Frontend and Backend API Integration
- Company Registration Integration
- Company Approval Workflow Integration
- Recruiter Invitation Integration
- Hiring Manager Invitation Integration
- Invitation Account Activation
- GitHub Repository Management
- Branch Management
- Pull Request Review
- Merge Conflict Resolution
- Code Review
- Security Configuration
- Sensitive Configuration Cleanup

**Continuing Responsibilities**

- Final System Integration
- Performance Optimization
- Security Review
- Bug Fixing
- Deployment
- Final Testing
- User Acceptance Testing Support
- Code Review and Merge Requests
- Production Configuration
- AI Service Integration Support

---

### Member 02 – Sudeesha Ravisara

**Candidate Module**

Responsible for candidate-related functionality.

**Developed or contributed to:**

- Candidate Profile Management
- Candidate Dashboard
- Education Management
- Experience Management
- Skills Management
- Resume Upload
- Resume Management
- Primary Resume Selection
- Candidate APIs
- Assisted with identifying bugs across the system
- Helped troubleshoot frontend and backend issues
- Supported API error detection and correction
- Helped resolve frontend–backend integration problems
- Assisted with debugging candidate module issues
- Helped test system workflows and identify broken features
- Job Search
- Candidate Module Testing

---

### Member 03 – Kavishka Dewuduni

**Recruiter and Job Management Module**

Responsible for recruiter operations and job-related functionality.

**Developed or contributed to:**

- Company Management
- Department Management
- Job CRUD Operations
- Job Skills Management
- Job Search and Filtering
- Recruiter Dashboard
- Recruiter APIs
- Candidate Search
- Recruiter Module Testing

---

### Member 04 – Hansi

**Recruitment Workflow Module**

Responsible for the recruitment and hiring workflow.

**Developed or contributed to:**

- Job Applications
- Application Tracking
- Application Status Management
- Candidate Shortlisting
- Candidate Rejection
- Interview Scheduling
- Interview Rescheduling
- Interview Cancellation
- Interview Feedback
- Candidate Evaluation
- Offer Workflow
- Hiring Workflow
- Recruitment Workflow APIs
- Workflow Testing

---

### Member 05 – Sachintha

**AI, Analytics and System Integration Support Module**

Responsible for AI-powered recruitment features, analytics, integration support, and project-wide technical improvements.

**Developed or contributed to:**

- Resume Parsing
- Skill Extraction
- AI Job Recommendation
- Candidate Ranking
- Candidate–Job Matching
- Recruitment Analytics
- Hiring Reports
- AI APIs
- AI Module Testing
- Module Error Fixing
- API Route Corrections
- Database Migration Support
- Frontend and Backend Build Fixes
- Platform Admin Integration
- Code Review
- Final Module Integration
- Security Configuration
- Sensitive Configuration Cleanup

---

### Member 06 – Sashin

**Platform Admin Module**

Responsible for managing the HirePath platform, companies, users, and system-level analytics.

**Developed or contributed to:**

- Platform Admin Dashboard
- Platform Admin Sidebar and Layout
- Company Management
- Pending Company Requests
- Company Approval
- Company Rejection
- Company Activation
- Company Suspension
- Company Deletion
- Global User Management
- User Search
- Role Filtering
- Status Filtering
- User Editing
- User Role Management
- User Activation and Suspension
- User Deletion Handling
- Platform Analytics
- Dashboard Statistics
- User Distribution Analytics
- Recruitment Analytics Display
- Platform Admin APIs
- Platform Admin Frontend Integration
- Platform Admin Module Testing

---

## Development Workflow

1. Clone the repository.
2. Pull the latest changes from main.
3. Switch to the assigned development branch.
4. Implement the assigned module.
5. Build and test the project.
6. Commit changes using a meaningful commit message.
7. Push the development branch.
8. Create a Pull Request.
9. Test merged modules in the test branch.
10. Merge the tested system into main.

**Team members should not modify another member's module without prior discussion.**

---

## Clone Project

```bash
git clone https://github.com/chamodyha123/HirePath.git
Open the solution in Visual Studio:

text
HirePath.sln
Backend Configuration
Open:

text
appsettings.json
Configure the database using placeholders or user secrets:

json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<SERVER_NAME>;Database=<DATABASE_NAME>;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
Do not commit real passwords, API keys, SMTP passwords, Cloudinary secrets, or JWT secrets to GitHub.

Create and Update Database
bash
dotnet ef database update
Alternatively, use Visual Studio Package Manager Console:

text
Update-Database
Run Backend
bash
dotnet restore
dotnet build
dotnet run
Run Frontend
bash
npm install
npm run dev
Default frontend URL:

text
http://localhost:5173
Default Development Administrator
Production credentials must not be placed inside the README.

Email: <ADMIN_EMAIL>
Password: <ADMIN_PASSWORD>

Git Workflow
Update Local Repository
bash
git checkout main
git pull origin main
Create Development Branch
bash
git checkout -b dev-yourname
Save Changes
bash
git add .
git commit -m "Complete assigned module"
Push Development Branch
bash
git push -u origin dev-yourname
Merge Process
text
Development Branch
        │
        ▼
Pull Request
        │
        ▼
Code Review
        │
        ▼
Test Branch
        │
        ▼
Integration Testing
        │
        ▼
Main Branch
Do not push directly to main unless the project lead explicitly approves it.

Coding Standards
Follow the existing folder structure.

Use PascalCase for C# classes and methods.

Use camelCase for JavaScript variables and functions.

Use async and await for database operations.

Keep controllers thin.

Store business logic inside services.

Store database access inside repositories.

Use DTOs for API requests and responses.

Apply input validation.

Use role-based authorization.

Use the shared Axios API instance.

Do not hardcode JWT tokens.

Do not hardcode backend URLs inside pages.

Do not use mock data in production modules.

Do not expose database entities unnecessarily.

Do not commit secrets.

Do not delete existing migrations without team approval.

Do not rename shared entities without discussion.

Important Development Rules
Before coding:

bash
git pull origin main
Before pushing:

Build the backend.

Build the frontend.

Fix compile errors.

Fix runtime errors.

Test APIs with Swagger.

Test frontend pages.

Verify role permissions.

Check browser console errors.

Check database updates.

Confirm no secrets are committed.

Backend Folder Structure
text
Controllers/
Data/
DTOs/
Enums/
Models/
Repositories/
Services/
Middleware/
Migrations/
Frontend Folder Structure
text
src/
├── api/
├── components/
├── layouts/
├── pages/
├── services/
├── styles/
└── routes/
Security
The system uses:

ASP.NET Identity

Password Hashing

JWT Authentication

Role-Based Authorization

Protected API Endpoints

Protected React Routes

OTP Verification

Email Verification

Input Validation

Secure File Upload Validation

Centralized Error Handling

CORS Configuration

Sensitive data must be stored using:

.NET User Secrets

Environment Variables

Azure Key Vault or another production secret manager

Default Development Accounts
Platform Administrator
Role

text
PlatformAdmin
Email

text
admin@hirepath.com
Password

text
Admin@123
Notes
These credentials are intended for development and testing only.

For production deployments:

Change the default administrator password.

Store credentials securely.

Do not expose real passwords in public repositories.
