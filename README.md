# HirePath AI – Developer Setup Guide

## Project Structure

```text
HirePath/
│
├── HirepathBackend/
│   └── HirePath/
│       ├── Controllers/
│       ├── Data/
│       ├── DTOs/
│       ├── Enums/
│       ├── Migrations/
│       ├── Models/
│       ├── Repositories/
│       ├── Services/
│       ├── Middleware/
│       ├── wwwroot/
│       ├── appsettings.json
│       ├── Program.cs
│       └── HirePath.csproj
│
└── HirePathFrontend/
    ├── public/
    ├── src/
    │   ├── api/
    │   ├── assets/
    │   ├── components/
    │   ├── layouts/
    │   ├── pages/
    │   │   ├── auth/
    │   │   ├── candidate/
    │   │   ├── company-admin/
    │   │   ├── recruiter/
    │   │   ├── hiring-manager/
    │   │   └── platform-admin/
    │   ├── services/
    │   ├── styles/
    │   ├── App.jsx
    │   ├── index.css
    │   └── main.jsx
    ├── package.json
    └── vite.config.js
```

---

# Prerequisites

Before running HirePath AI, make sure the following software is installed:

- .NET 8 SDK
- Visual Studio 2022
- SQL Server / SQL Server Express
- SQL Server Management Studio (SSMS)
- Node.js
- npm
- Git

---

# Backend Setup

## 1. Open the Backend

Navigate to:

```powershell
cd HirepathBackend/HirePath
```

Open the project in Visual Studio or use the terminal.

---

## 2. Restore NuGet Packages

Run:

```powershell
dotnet restore
```

If required packages are missing, they can be installed using:

```powershell
Install-Package Microsoft.EntityFrameworkCore
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package Microsoft.AspNetCore.Identity.EntityFrameworkCore
Install-Package Microsoft.AspNetCore.Authentication.JwtBearer
Install-Package Swashbuckle.AspNetCore
```

When using the .NET CLI instead of Visual Studio Package Manager Console:

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Swashbuckle.AspNetCore
```

Use package versions compatible with .NET 8 and the existing project.

---

# Database Configuration

## 3. Configure appsettings.json

Open:

```text
HirepathBackend/HirePath/appsettings.json
```

Configure the SQL Server connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<SERVER_NAME>;Database=HirePathDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },

  "Jwt": {
    "Key": "<JWT_SECRET_KEY>",
    "Issuer": "<JWT_ISSUER>",
    "Audience": "<JWT_AUDIENCE>",
    "DurationInMinutes": 120
  },

  "EmailSettings": {
    "Host": "<SMTP_HOST>",
    "Port": 587,
    "SenderName": "HirePath AI",
    "SenderEmail": "<SENDER_EMAIL>",
    "Username": "<SMTP_USERNAME>",
    "Password": "<SMTP_APP_PASSWORD>",
    "EnableSSL": true
  },

  "CloudinarySettings": {
    "CloudName": "<CLOUDINARY_CLOUD_NAME>",
    "ApiKey": "<CLOUDINARY_API_KEY>",
    "ApiSecret": "<CLOUDINARY_API_SECRET>"
  }
}
```

Replace all placeholder values with your own local development configuration.

> **Important:** Never commit real passwords, JWT secrets, SMTP passwords, Cloudinary API secrets, or other sensitive credentials to GitHub.

For production environments, use environment variables or another secure secret-management solution.

---

# Database Setup

## 4. Apply Existing Migrations

Make sure SQL Server is running.

Using Visual Studio Package Manager Console:

```powershell
Update-Database
```

Or using the .NET CLI:

```bash
dotnet ef database update
```

This creates or updates the `HirePathDB` database using the project's Entity Framework Core migrations.

If the EF CLI tool is not installed:

```bash
dotnet tool install --global dotnet-ef
```

---

# Run Backend

## 5. Build the Backend

```bash
dotnet build
```

If the build succeeds, run:

```bash
dotnet run
```

Alternatively, open the project in Visual Studio 2022 and press:

```text
F5
```

or:

```text
Ctrl + F5
```

The backend API and Swagger interface should start using the URL configured in the project's launch settings.

---

# Frontend Setup

## 6. Navigate to Frontend

Open a new terminal:

```bash
cd HirePathFrontend
```

---

## 7. Install Frontend Dependencies

Install all dependencies defined in `package.json`:

```bash
npm install
```

Normally, this is all that is required.

If individual packages are missing, install them manually:

```bash
npm install axios
npm install react-router-dom
npm install react-icons
```

---

# Frontend API Configuration

The frontend communicates with the ASP.NET Core backend through Axios.

Check the shared Axios/API configuration and make sure the backend base URL matches the backend URL running on your computer.

Example:

```javascript
const API_BASE_URL = "http://localhost:<BACKEND_PORT>/api";
```

Do not hardcode different backend URLs throughout individual React pages. Use the project's shared Axios/API configuration.

JWT authentication should also use the centralized Axios configuration so authenticated requests send the required authorization token.

---

# Run Frontend

Start the Vite development server:

```bash
npm run dev
```

The frontend should normally be available at:

```text
http://localhost:5173
```

Open:

```text
http://localhost:5173
```

Do not use:

```text
https://localhost:5173
```

unless HTTPS has specifically been configured for the Vite development server.

---

# Backend + Frontend Run Order

Both the backend and frontend must be running for the complete HirePath AI system to work.

Use the following order:

```text
1. Start SQL Server
        ↓
2. Start HirePath Backend
        ↓
3. Confirm Backend / Swagger is running
        ↓
4. Open a second terminal
        ↓
5. Navigate to HirePathFrontend
        ↓
6. Run npm install if required
        ↓
7. Run npm run dev
        ↓
8. Open http://localhost:5173
```

---

# System Login Flow

After both applications are running:

```text
User opens HirePath AI
        ↓
Registration / Login
        ↓
ASP.NET Identity validates account
        ↓
JWT token generated
        ↓
Frontend stores authentication information
        ↓
Role is identified
        ↓
Protected route validation
        ↓
User redirected to correct dashboard
```

Supported system roles include:

- Candidate
- CompanyAdmin
- Recruiter
- HiringManager
- PlatformAdmin / SuperAdmin

---

# Role-Based Dashboards

After successful authentication, users are redirected according to their role.

```text
Candidate
    → Candidate Dashboard

CompanyAdmin
    → Company Admin Dashboard

Recruiter
    → Recruiter Dashboard

HiringManager
    → Hiring Manager Dashboard

PlatformAdmin / SuperAdmin
    → Platform Admin Dashboard
```

---

# Development Checklist

Before running the system, verify:

- SQL Server is running.
- The connection string is correct.
- Required database migrations are applied.
- Backend packages are restored.
- Backend builds successfully.
- Frontend dependencies are installed.
- Backend API is running.
- Axios base URL points to the correct backend.
- Frontend is running on port 5173.
- JWT configuration is available.
- Email configuration is available if testing OTP/email features.
- Browser console has no critical errors.
- Swagger API requests work correctly.

---

# Important Security Rules

Never commit the following to GitHub:

```text
Database passwords
JWT secret keys
SMTP passwords
Gmail App Passwords
Cloudinary API secrets
External AI service keys
Production administrator passwords
```

Use placeholders in committed configuration files:

```text
<JWT_SECRET_KEY>
<SMTP_APP_PASSWORD>
<CLOUDINARY_API_SECRET>
<API_KEY>
```

---

# Quick Start

For developers who have already configured the project:

## Backend

```bash
cd HirepathBackend/HirePath
dotnet restore
dotnet ef database update
dotnet run
```

## Frontend

Open another terminal:

```bash
cd HirePathFrontend
npm install
npm run dev
```

Then open:

```text
http://localhost:5173
```

---

# Troubleshooting

If the backend fails to start:

```bash
dotnet restore
dotnet build
```

Check the SQL Server connection string and confirm that SQL Server is running.

If the database schema is outdated:

```bash
dotnet ef database update
```

If the frontend fails to start:

```bash
npm install
npm run dev
```

If frontend API requests fail, verify:

```text
Backend is running
Backend port is correct
Axios base URL is correct
JWT token is available
CORS is correctly configured
Database is available
```

---

# HirePath AI

AI-Powered Recruitment & Talent Management Platform  
Software Architecture – SE205.3  
NSBM Green University
