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
│       ├── Migrations/
│       ├── Models/
│       ├── Repositories/
│       ├── Services/
│       ├── wwwroot/
│       ├── appsettings.json
│       ├── Program.cs
│       └── HirePath.csproj
│
└── HirePathFrontend/
    ├── public/
    ├── src/
    │   ├── api/
    │   │   └── axios.js
    │   ├── assets/
    │   ├── pages/
    │   │   ├── Home.jsx
    │   │   └── auth/
    │   │       ├── Login.jsx
    │   │       ├── Register.jsx
    │   │       ├── VerifyEmail.jsx
    │   │       ├── ForgotPassword.jsx
    │   │       ├── ResetPassword.jsx
    │   │       └── Auth.css
    │   ├── App.jsx
    │   ├── index.css
    │   └── main.jsx
    ├── package.json
    └── vite.config.js
#in backend you should install these pakages
Install-Package Microsoft.EntityFrameworkCore
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package Microsoft.AspNetCore.Identity.EntityFrameworkCore
Install-Package Microsoft.AspNetCore.Authentication.JwtBearer
Install-Package Swashbuckle.AspNetCore

Frontend Packages

Go to the frontend folder:

cd HirePathFrontend

Install all packages:

npm install

If packages are missing, install manually:

npm install axios
npm install react-router-dom
npm install react-icons
Run Frontend
npm run dev

Frontend URL:

http://localhost:5173

Do not use https://localhost:5173.

Backend + Frontend Run Order

Run both projects:

1. Start Backend from Visual Studio
2. Start Frontend using npm run dev
3. Open http://localhost:5173
