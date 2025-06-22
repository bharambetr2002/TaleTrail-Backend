# 🚀 Initial Setup

> Follow these steps to get started with the TaleTrail backend development:

1. **Clone this repository**

   ```bash
   git clone https://github.com/your-org/taletrail-backend.git
   cd taletrail-backend
   ```

2. Mail me your GitHub ID
   Send your GitHub username to: bharambetr2002@gmail.com
   I’ll add you as a collaborator to the repository.

3. Create your own branch
   ```bash
   git checkout -b branch_name
   ```
   all changes you do are to be done in this branch

# 📦 NuGet Packages Used in TaleTrail Backend (.NET)

This document explains all the NuGet packages installed in the **TaleTrail Backend** project, including their purpose, usage, and future relevance.

---

## 📋 Package Overview

| Package Name                                       | Version | Description                                                                 | Current Usage                                                                                |
| -------------------------------------------------- | ------- | --------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------- |
| **DotNetEnv**                                      | `3.1.1` | Loads environment variables from a `.env` file for local development.       | ✅ Actively used to inject Supabase credentials (`superbaseUrl`, `superbaseKey`).            |
| **Microsoft.AspNetCore.Authentication.JwtBearer**  | `8.0.5` | Enables JWT authentication middleware. Useful for protecting APIs.          | 🔜 Reserved for future token validation (currently using Supabase directly).                 |
| **Microsoft.AspNetCore.OpenApi**                   | `8.0.5` | Adds native support for OpenAPI metadata in .NET 8 APIs.                    | ✅ Complements Swagger for accurate schema generation.                                       |
| **Microsoft.Extensions.Configuration.UserSecrets** | `8.0.0` | Enables secure local secrets storage via `dotnet user-secrets`.             | 🔁 Optional; not currently used since `.env` is preferred.                                   |
| **Supabase**                                       | `1.1.1` | Official .NET SDK for Supabase services (Auth, Realtime, Database, Storage) | 🔁 Auth endpoints are currently built using `HttpClient`. Will use SDK for DB/storage later. |
| **Swashbuckle.AspNetCore**                         | `6.5.0` | Generates Swagger UI and OpenAPI documentation from controllers.            | ✅ Used to auto-generate API docs at `/swagger`.                                             |

---

## ✅ Summary of Usage

| Feature                    | Package(s) Used                                  | Status      |
| -------------------------- | ------------------------------------------------ | ----------- |
| Load environment variables | `DotNetEnv`                                      | ✅ Active   |
| Supabase integration       | `Supabase`, manual `HttpClient`                  | ⚙️ Partial  |
| API documentation          | `Swashbuckle.AspNetCore`, `OpenApi`              | ✅ Active   |
| JWT support (planned)      | `Microsoft.AspNetCore.Authentication.JwtBearer`  | 🕓 Planned  |
| Local secrets (optional)   | `Microsoft.Extensions.Configuration.UserSecrets` | 🔁 Optional |

---

## 📦 For Team Members

- Use the `.env` file to manage your local secrets. Add:

  ```env
  superbaseUrl=https://your-project.supabase.co
  superbaseKey=your-anon-public-api-key
  ```

  - Mail me for the .env file on : bharambetr2002@gmail.com

- Do not commit your .env file to GitHub.

- You can explore Supabase SDK later for:

  1. Realtime updates
  2. PostgreSQL queries via Supabase ORM
  3. File storage (e.g., book cover images)

- JWT validation setup is possible if we move towards a more secure backend session system.
