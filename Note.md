# 🧠 TaleTrail Backend - Developer Notes

This document explains how the backend of the TaleTrail project works, how to run it, and how it connects with the frontend and database.

---

## 1️⃣ Backend Setup (For Development)

To run the backend on your machine, make sure you have the following installed:

- **.NET 9 SDK (v9.0.301)** - Required for building and running the backend. // but the project is gonna run on dotnet 8
- **MySQL** (only needed for local optional DB testing)
- **Visual Studio Code** with essential extensions (listed below)
- **Git** - To clone the project repo
- **EF Core CLI** - Installed globally as `dotnet-ef`
- **ASP.NET Code Generator CLI** - Installed globally as `dotnet-aspnet-codegenerator`

> After opening the project in VS Code, create the environment file `appsettings.Development.json` or `.env` to store secrets like:

- Supabase URL
- Supabase Service Key
- JWT Issuer and Audience (e.g. "supabase", "authenticated")

Run the backend using:

```bash
cd TaleTrail.APIa
dotnet run
```

This starts the API locally.

---

## 2️⃣ Project Structure

```
TaleTrail.API/
├── Controllers/          # All API controllers
├── Models/               # Entity models like Book, Author, User, etc.
├── DTOs/                 # Data Transfer Objects (DTOs)
├── Services/             # Business logic (e.g., SupabaseService)
├── Data/                 # DbContext (TaleTrailDbContext.cs)
├── Enums/                # Enum types (e.g., ReadingStatus)
├── appsettings.json      # Global configuration
├── Properties/           # Launch settings
├── Program.cs            # Entry point
├── TaleTrail.API.csproj  # Project definition
└── .env / .dev.env       # Env variables
```

---

## 3️⃣ NuGet Packages Installed

Your `.csproj` includes these dependencies:

- `Microsoft.AspNetCore.Authentication.JwtBearer` (8.0.5)
- `Microsoft.AspNetCore.OpenApi` (8.0.5)
- `Microsoft.EntityFrameworkCore.Design` (9.0.6)
- `Microsoft.EntityFrameworkCore.Tools` (9.0.6)
- `Npgsql.EntityFrameworkCore.PostgreSQL` (9.0.4)
- `Microsoft.VisualStudio.Web.CodeGeneration.Design` (9.0.0)
- `Microsoft.Extensions.Configuration.UserSecrets` (8.0.0)
- `Swashbuckle.AspNetCore` (6.5.0)
- `DotNetEnv` (3.1.1)
- `Supabase` (1.1.1)

---

## 4️⃣ Important Tools & Extensions

Global tools installed:

- `dotnet-ef` (9.0.6)
- `dotnet-aspnet-codegenerator` (9.0.0)

Recommended VS Code extensions:

- `ms-dotnettools.csharp`
- `ms-dotnettools.csdevkit`
- `jmrog.vscode-nuget-package-manager`
- `kreativ-software.csharpextensions`
- `josefpihrt-vscode.roslynator`
- `eamodio.gitlens`, `esbenp.prettier-vscode`, `dbaeumer.vscode-eslint`

---

## 5️⃣ What is Render?

Render is our backend deployment host. After deployment, it gives a public URL which the frontend uses to hit API endpoints.

Example:

```
https://taletrail-api.onrender.com/api/books
```

Make sure to configure this base URL in the frontend's Axios or fetch configs.

---

## 6️⃣ API Routing Examples

| Purpose          | Endpoint                | Method |
| ---------------- | ----------------------- | ------ |
| Sign Up          | /auth/signup            | POST   |
| Login            | /auth/login             | POST   |
| Get Current User | /me                     | GET    |
| List Books       | /api/books              | GET    |
| Add Book         | /api/books              | POST   |
| Review a Book    | /api/books/{id}/reviews | POST   |

---

## 7️⃣ Docker Usage

We’ve added Docker support to:

- Isolate backend from local machine
- Ensure consistent dev & prod environments
- Make onboarding easier for new teammates

Running Docker will spin up a container with all necessary backend dependencies so everyone can test without setup issues.

---

## ✅ Current Status (as of June 26, 2025)

- ✅ .NET 8 environment configured
- ✅ Supabase Auth & DB connected
- ✅ DTOs, Models, Enums created
- ✅ `TaleTrailDbContext` implemented
- ✅ SupabaseService done
- ✅ AuthController ready
- ⚠️ REST Controllers pending manual creation (due to codegen issues)
- ✅ Swagger, JWT setup underway
- ✅ VS Code extensions installed
- 🚧 CI/CD partly integrated

---

## 🧠 Notes for Developers

- Always keep your `.env` or `appsettings.Development.json` synced with Supabase values.
- Avoid committing secrets or service keys.
- Use Postman/ThunderClient to test endpoints locally.
- Scaffold or manually write controllers for models like `Book`, `Author`, etc.
- If you face codegen issues with PostgreSQL, fallback to manual API building.
- Keep Docker in sync for consistent builds.

---

You're ready to contribute to TaleTrail's backend 🚀
