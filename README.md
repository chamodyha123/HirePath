# HirePath AI - AI Powered Recruitment & Talent Management Platform

## Software Architecture (SE205.3)
NSBM Green University

---

# Project Overview

HirePath AI is an AI-powered Recruitment and Talent Management Platform developed as part of the Software Architecture (SE205.3) module.

The system modernizes recruitment by providing:

- Candidate Portal
- Recruiter Portal
- Hiring Manager Dashboard
- Administrator Dashboard
- AI-powered Job Matching
- Resume Management
- Interview Scheduling
- Recruitment Analytics

Backend is developed using **ASP.NET Core Web API** with **SQL Server**.

---

# Technologies

Backend
- ASP.NET Core 8 Web API
- Entity Framework Core
- SQL Server
- ASP.NET Identity
- JWT Authentication

Tools
- Visual Studio 2022
- SQL Server
- Git & GitHub
- Swagger

Architecture
- Repository Pattern
- Dependency Injection
- REST API
- Identity Authentication
- Role Based Authorization

---

# Current Progress

✅ Phase 01 Completed

- Project Structure
- Entity Models
- Enums
- ApplicationDbContext
- Relationships
- SQL Server Integration
- Migrations

✅ Phase 02 Completed

- ASP.NET Identity
- JWT Authentication
- Register API
- Login API
- Role Management
- Admin Seed
- Repository Pattern
- Swagger Authentication

---
## ✅ Phase 03 - Service Layer & API Architecture (Completed)

- Service Layer Implementation
- Business Logic Separation
- Repository-Service Architecture
- Dependency Injection for Services
- API Response Handling
- Clean Architecture Principles
- Reusable Business Services
- Controller-Service Communication
- Code Organization Improvements

---

## Current Architecture

```
Controllers
      │
      ▼
Services (Business Logic)
      │
      ▼
Repositories (Data Access)
      │
      ▼
Entity Framework Core
      │
      ▼
SQL Server
```

The project foundation is now complete. Team members can begin implementing their assigned modules without modifying the core architecture.

# Team Responsibilities

## Member 01 ( Chamodyha Peshan)

Responsible for the overall backend architecture, security, integration, and project management.

### Completed

* Project Architecture
* Database Design
* Entity Framework Core
* ApplicationDbContext
* ASP.NET Identity
* JWT Authentication
* Repository Pattern
* Service Layer
* Dependency Injection
* GitHub Repository Management
* Code Review
* Module Integration
* Phase 01 – Database Foundation
* Phase 02 – Authentication & Authorization
* Phase 03 – Core Backend Foundation

### Will Continue

* AI Service Integration
* Final System Integration
* Performance Optimization
* Bug Fixing
* Deployment
* Final Testing
* Code Review & Merge Requests

---

## Member 02 - sudeesha ravisara

### Candidate Module

Responsible for all candidate-related features.

Develop:

* Candidate Profile Management
* Skills Management
* Education Management
* Experience Management
* Resume Upload & Management
* Candidate Dashboard
* Candidate APIs
* Candidate Module Testing

---

## Member 03

### Recruiter Module

Responsible for recruiter operations and job management.

Develop:

* Company Management
* Department Management
* Job CRUD Operations
* Job Skills Management
* Job Search & Filtering
* Recruiter Dashboard
* Recruiter APIs
* Recruiter Module Testing

---

## Member 04

### Recruitment Workflow Module

Responsible for the complete hiring workflow.

Develop:

* Job Applications
* Application Tracking
* Application Status Management
* Interview Scheduling
* Interview Feedback
* Candidate Evaluation
* Hiring Workflow
* Recruitment Workflow APIs
* Workflow Testing

---

## Member 05

### AI & Analytics Module

Responsible for AI-powered features and reporting.

Develop:

* Resume Parsing
* Skill Extraction
* AI Job Recommendation
* Candidate Ranking
* Candidate–Job Matching
* Recruitment Analytics
* Hiring Reports
* AI APIs
* AI Module Testing

---

## Member 06

### External Integrations & Communication Module

Responsible for integrations with external services and notifications.

Develop:

* Resume File Storage
* Email Notifications
* Interview Reminder Emails
* Google Calendar Integration
* Microsoft Outlook Calendar Integration
* Cloud Storage Integration (Azure Blob / Cloudinary)
* Notification Services
* Integration Testing
* User Acceptance Testing (UAT)

---

# Branch Strategy

| Member    | Branch                     |
| --------- | -------------------------- |
| Member 01 | feature/project-foundation |
| Member 02 | feature/candidate-module   |
| Member 03 | feature/recruiter-module   |
| Member 04 | feature/application-module |
| Member 05 | feature/ai-analytics       |
| Member 06 | feature/integrations       |

---

# Development Workflow

1. Clone the repository.
2. Create or switch to your assigned branch.
3. Implement only your assigned module.
4. Commit changes with meaningful commit messages.
5. Push your branch to GitHub.
6. Create a Pull Request to the `main` branch.
7. The Project Lead will review and merge approved changes.
8. Resolve merge conflicts before submitting new Pull Requests.

**Important:** Team members should not modify another member's module without prior discussion to minimize merge conflicts and maintain clear individual contributions.


# Clone Project

Clone the repository

```bash
git clone https://github.com/USERNAME/HirePathAI.git
```

Open Visual Studio

Open

```
HirePathAI.sln
```

---

# Configure Database

Open

```
appsettings.json
```

Change SQL Server connection string

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=HirePathDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

---

# Create Database

Open Package Manager Console

Run

```powershell
Update-Database
```

---

# Run Project

Press

```
F5
```

or

```
Ctrl + F5
```

Swagger will open automatically.

---

# Default Admin

Email

```
admin@hirepath.com
```

Password

```
Admin@123
```

---

# Git Workflow

## First Time

```bash
git clone https://github.com/USERNAME/HirePathAI.git
```

---

## Before Starting Work

```bash
git pull origin main
```

---

## Create Your Branch

Member 2

```bash
git checkout -b candidate-module
```

Member 3

```bash
git checkout -b recruiter-module
```

Member 4

```bash
git checkout -b application-module
```

Member 5

```bash
git checkout -b admin-module
```

---

## Save Your Work

```bash
git add .
git commit -m "Completed Candidate Module"
```

---

## Push

```bash
git push origin candidate-module
```

Do NOT push directly to **main**.

---

# Coding Standards

- Follow existing folder structure.
- Use PascalCase for classes.
- Use async/await.
- Keep controllers thin.
- Business logic belongs in Services.
- Database access belongs in Repositories.
- Do not modify ApplicationDbContext without informing the team.
- Do not delete existing migrations.
- Do not rename existing entities without discussion.

---

# Important Rules

Before coding

Always

```bash
git pull origin main
```

Before pushing

- Build Solution
- Fix all compile errors
- Test your APIs using Swagger

---

# Folder Structure

```
Controllers/
Data/
DTOs/
Enums/
Models/
Repositories/
Services/
Middleware/
Migrations/
```

---

# Contact

Project Lead

Responsible for

- Architecture
- Database
- Authentication
- Repository Pattern
- GitHub Management

---

Let's build an excellent recruitment platform together!
