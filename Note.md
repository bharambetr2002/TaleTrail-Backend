# 🧠 TaleTrail Backend - Developer Notes

This document explains how the backend of the TaleTrail project works, how to run it, and how it connects with the frontend and database.

---

## 1️⃣ Backend Setup (For Development)

To run the backend on your machine, you need a few things installed first:

- .NET 8 SDK (required to run the backend code)
- MySQL (only needed if testing with a local database)
- Any code editor like Visual Studio or VS Code
- Git to get the project files from the repository

After opening the project in your editor, you must create a settings file to store your Supabase and JWT configuration. This is usually done using a file named `appsettings.Development.json` or `.env`.

Example values you need to configure:

- Supabase URL
- Supabase service key
- JWT Issuer and Audience (usually "supabase" and "authenticated")

Once your environment is ready, use your IDE to build and run the backend. This will launch the backend locally so you can test it.

---

## 2️⃣ What is Render?

Render is the hosting platform we are using to deploy the backend on the internet.

Once deployed, Render gives us a public link where all the backend APIs are available. This link is shared with the frontend so it can communicate with the backend.

All backend routes work under this base URL.

---

## 3️⃣ Backend API Routing

Below are examples of backend API routes that the frontend will use:

| Purpose          | Endpoint                | Method |
| ---------------- | ----------------------- | ------ |
| Sign Up          | /auth/signup            | POST   |
| Login            | /auth/login             | POST   |
| Get Current User | /me                     | GET    |
| List Books       | /api/books              | GET    |
| Add Book         | /api/books              | POST   |
| Review a Book    | /api/books/{id}/reviews | POST   |

The frontend connects to these routes using fetch or axios. You must make sure the base URL is set to the Render link inside the frontend code.

---

## 4️⃣ What is Docker and Why Are We Using It?

Docker is a tool that helps package our application so it works the same on any computer.

We added Docker to this project for three main reasons:

1. It creates a clean environment that avoids version or setup issues.
2. It allows anyone on the team to run the backend without worrying about local configuration.
3. It makes deployment easier and more consistent on platforms like Render.

When Docker is used, it creates a container (like a lightweight virtual machine) with everything the backend needs already installed. This helps reduce errors and improves teamwork.

---

## 5️⃣ Summary for Teammates

- The backend is built with .NET 8 and connects to Supabase for data and authentication.
- The project is deployed on Render and accessible via a public URL.
- The frontend should call backend APIs using the Render link.
- Docker is used to make the backend portable, consistent, and easier to run or deploy.

---
