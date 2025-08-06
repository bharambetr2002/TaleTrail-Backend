📚 TaleTrail API
A modern, streamlined book tracking and social platform built with ASP.NET Core 8 and Supabase. TaleTrail allows users to manage their reading journey, write blogs, review books, and share their progress with a public profile.

🚀 Features
Core Functionality
User Authentication - Secure signup/login with Supabase Auth and JWTs.

Public User Profiles - Shareable profiles with reading stats (/profile/{username}).

Admin Content Management - Admins manage the global catalog of books, authors, and publishers.

Personal Blogs - Users can write, edit, and delete their own blog posts.

Book Reviews - Users can rate (1-5 stars) and write reviews for books.

Personal Reading Lists - Track books with statuses: wanna_read, in_progress (max 3), completed, dropped.

Technical Features
RESTful API with Swagger/OpenAPI documentation.

Layered Architecture - Controllers, Services, and DAOs for clean separation of concerns.

Role-Based Access Control - Secure endpoints for regular users and admins.

Custom Middleware for error handling and JWT validation.

CORS Support for seamless frontend integration.

Environment Configuration with .env files for local development.

Health Check endpoint for monitoring.

🛠️ Tech Stack
Framework: .NET 8

Database: Supabase (PostgreSQL)

Authentication: Supabase Auth (JWT-based)

Deployment: Docker, Render

Documentation: Swagger/OpenAPI

📁 Project Structure
TaleTrail.API/
├── Controllers/    # API Controllers (e.g., Books, Profile, UserBook)
├── Services/       # Business Logic Layer
├── DAO/            # Data Access Objects (interacts with Supabase)
├── DTOs/           # Data Transfer Objects (API contracts)
├── Models/         # Database Entity Models
├── Middleware/     # Custom Middleware (Errors, Auth)
├── Exceptions/     # Custom Exception Classes
└── Helpers/        # Utility Classes (e.g., ApiResponse)

🚦 Getting Started
Prerequisites
.NET 8 SDK

A Supabase Account

Docker (for local testing)

Installation
Clone the repository

git clone <repository-url>
cd TaleTrail-Backend

Create your environment file

In the TaleTrail.API directory, create a file named .env.

Add your Supabase credentials to this file:

SUPABASE_URL=your-supabase-url
SUPABASE_KEY=your-supabase-service-role-key
SUPABASE_JWT_SECRET=your-supabase-jwt-secret

Restore dependencies

dotnet restore

Run the application

cd TaleTrail.API
dotnet run

Access the application

API: https://localhost:7218 (or your configured port)

Swagger: https://localhost:7218/swagger

Health Check: https://localhost:7218/health

📖 API Documentation
A full list of endpoints is available via the Swagger UI.

Key Endpoints
POST /api/auth/signup - User registration.

POST /api/auth/login - User login.

GET /api/profile/{username} - Public: Get a user's shareable profile.

GET /api/user/profile/my-profile - Protected: Get the logged-in user's own profile.

PUT /api/user/profile/my-profile - Protected: Update the logged-in user's profile.

GET /api/book - Get all books (supports ?search=query).

POST /api/book - Admin: Create a new book.

GET /api/user-book/my-list - Protected: Get the user's reading list.

POST /api/user-book - Protected: Add or update a book on the user's list.

🚀 Deployment
This project is configured for easy deployment to Render using Docker.

Push your code to a GitHub repository.

Create a new "Web Service" on Render and connect it to your repository.

Set the environment to "Docker".

Add your SUPABASE_URL, SUPABASE_KEY, and SUPABASE_JWT_SECRET as environment variables in the Render dashboard.

Render will automatically build the Docker image and deploy your API.

Made with ❤️ for book lovers everywhere!