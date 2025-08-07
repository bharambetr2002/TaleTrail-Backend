# 📚 TaleTrail API

> A clean, simple book tracking API built with .NET 8 and Supabase

[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Supabase](https://img.shields.io/badge/Supabase-Backend-green)](https://supabase.com)
[![Docker](https://img.shields.io/badge/Docker-Containerized-blue)](https://www.docker.com)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

TaleTrail is a modern book tracking API that allows users to manage their reading lists, write reviews, create blog posts, and track their reading progress. Built with simplicity and beginner-friendliness in mind.

## ✨ Features

- 🔐 **Authentication**: Secure user registration and login with JWT
- 📖 **Book Management**: Browse and search books with author and publisher information
- 📚 **Reading Lists**: Track books with different statuses (ToRead, InProgress, Completed, Dropped)
- ⭐ **Reviews**: Write and manage book reviews with ratings
- 📝 **Blog Posts**: Create and share blog posts about books
- 👤 **User Profiles**: Manage user profiles and public pages
- 🏥 **Health Checks**: Built-in health monitoring
- 📊 **Swagger Documentation**: Interactive API documentation

## 🚀 Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) (optional)
- [Supabase Account](https://supabase.com)

### 🏃‍♂️ Running Locally

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/taletrail-api.git
   cd taletrail-api
   ```

2. **Set up environment variables**
   ```bash
   cp .env.example .env
   # Edit .env with your Supabase credentials
   ```

3. **Run the application**
   ```bash
   cd TaleTrail.API
   dotnet restore
   dotnet run
   ```

4. **Open your browser**
   - API: http://localhost:5198
   - Swagger: http://localhost:5198

### 🐳 Running with Docker

1. **Using Docker Compose (Recommended)**
   ```bash
   docker-compose up -d
   ```

2. **Using Docker directly**
   ```bash
   docker build -t taletrail-api -f TaleTrail.API/Dockerfile .
   docker run -p 8080:8080 taletrail-api
   ```

## 📚 API Documentation

### Base URL
- Development: `http://localhost:5198`
- Production: `https://your-render-app.onrender.com`

### 🔑 Authentication Endpoints
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/signup` | Register new user |
| POST | `/api/auth/login` | Login user |

### 📖 Book Endpoints
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/book` | Get all books (with search) |
| GET | `/api/book/{id}` | Get book by ID |
| GET | `/api/book/by-author/{authorId}` | Get books by author |

### 📚 User Book Endpoints (Protected)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/userbook/my-books` | Get user's books |
| POST | `/api/userbook` | Add book to user's list |
| PUT | `/api/userbook/{bookId}` | Update book status |
| DELETE | `/api/userbook/{bookId}` | Remove book from list |

### ⭐ Review Endpoints
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/review/book/{bookId}` | Get reviews for a book |
| POST | `/api/review` | Create review 🔒 |
| PUT | `/api/review/{id}` | Update review 🔒 |
| DELETE | `/api/review/{id}` | Delete review 🔒 |

### 📝 Blog Endpoints
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/blog` | Get all blogs |
| GET | `/api/blog/{id}` | Get blog by ID |
| POST | `/api/blog` | Create blog post 🔒 |
| PUT | `/api/blog/{id}` | Update blog post 🔒 |
| DELETE | `/api/blog/{id}` | Delete blog post 🔒 |

*🔒 = Requires authentication*

For detailed API documentation, visit `/swagger` when the application is running.

## 🏗️ Architecture

```
TaleTrail.API/
├── Controllers/         # API endpoints
├── Services/           # Business logic
├── DAO/               # Data access layer
├── Model/             # Entity models and DTOs
├── Middleware/        # Custom middleware
├── Extensions/        # Extension methods
└── Data/             # Database seeding
```

## 🔧 Configuration

### Environment Variables

| Variable | Description | Required |
|----------|-------------|----------|
| `SUPABASE_URL` | Your Supabase project URL | ✅ |
| `SUPABASE_KEY` | Your Supabase anon key | ✅ |
| `SUPABASE_JWT_SECRET` | Your Supabase JWT secret | ✅ |
| `ALLOWED_ORIGINS` | CORS allowed origins | ❌ |
| `ASPNETCORE_ENVIRONMENT` | Environment (Development/Production) | ❌ |

### Database Schema

The application uses the following main entities:
- **Users**: User profiles and authentication
- **Books**: Book information with authors and publishers
- **UserBooks**: User's reading lists and progress
- **Reviews**: Book reviews and ratings
- **Blogs**: User blog posts
- **Authors**: Book authors
- **Publishers**: Book publishers

## 🚀 Deployment

### Deploy to Render

1. **Push to GitHub**: Ensure your code is in a GitHub repository
2. **Connect to Render**: Link your GitHub repo to Render
3. **Set Environment Variables**: Configure your Supabase credentials in Render
4. **Deploy**: Render will automatically build and deploy using Docker

### Deploy to Other Platforms

The application is containerized and can be deployed to:
- **Heroku**: Using Docker
- **Railway**: Using Docker
- **DigitalOcean App Platform**: Using Docker
- **AWS ECS/Fargate**: Using Docker
- **Google Cloud Run**: Using Docker

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

- 📖 [Documentation](./docs/)
- 🐛 [Issues](https://github.com/yourusername/taletrail-api/issues)
- 💬 [Discussions](https://github.com/yourusername/taletrail-api/discussions)

## 🙏 Acknowledgments

- [.NET Team](https://github.com/dotnet) for the amazing framework
- [Supabase](https://supabase.com) for the backend infrastructure
- [Swagger](https://swagger.io) for API documentation
- Community contributors and supporters

---

<p align="center">Made with ❤️ for book lovers</p># 📚 TaleTrail API

> A clean, simple book tracking API built with .NET 8 and Supabase

[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Supabase](https://img.shields.io/badge/Supabase-Backend-green)](https://supabase.com)
[![Docker](https://img.shields.io/badge/Docker-Containerized-blue)](https://www.docker.com)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

TaleTrail is a modern book tracking API that allows users to manage their reading lists, write reviews, create blog posts, and track their reading progress. Built with simplicity and beginner-friendliness in mind.

## ✨ Features

- 🔐 **Authentication**: Secure user registration and login with JWT
- 📖 **Book Management**: Browse and search books with author and publisher information
- 📚 **Reading Lists**: Track books with different statuses (ToRead, InProgress, Completed, Dropped)
- ⭐ **Reviews**: Write and manage book reviews with ratings
- 📝 **Blog Posts**: Create and share blog posts about books
- 👤 **User Profiles**: Manage user profiles and public pages
- 🏥 **Health Checks**: Built-in health monitoring
- 📊 **Swagger Documentation**: Interactive API documentation

## 🚀 Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) (optional)
- [Supabase Account](https://supabase.com)

### 🏃‍♂️ Running Locally

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/taletrail-api.git
   cd taletrail-api
   ```

2. **Set up environment variables**
   ```bash
   cp .env.example .env
   # Edit .env with your Supabase credentials
   ```

3. **Run the application**
   ```bash
   cd TaleTrail.API
   dotnet restore
   dotnet run
   ```

4. **Open your browser**
   - API: http://localhost:5198
   - Swagger: http://localhost:5198

### 🐳 Running with Docker

1. **Using Docker Compose (Recommended)**
   ```bash
   docker-compose up -d
   ```

2. **Using Docker directly**
   ```bash
   docker build -t taletrail-api -f TaleTrail.API/Dockerfile .
   docker run -p 8080:8080 taletrail-api
   ```

## 📚 API Documentation

### Base URL
- Development: `http://localhost:5198`
- Production: `https://your-render-app.onrender.com`

### 🔑 Authentication Endpoints
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/signup` | Register new user |
| POST | `/api/auth/login` | Login user |

### 📖 Book Endpoints
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/book` | Get all books (with search) |
| GET | `/api/book/{id}` | Get book by ID |
| GET | `/api/book/by-author/{authorId}` | Get books by author |

### 📚 User Book Endpoints (Protected)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/userbook/my-books` | Get user's books |
| POST | `/api/userbook` | Add book to user's list |
| PUT | `/api/userbook/{bookId}` | Update book status |
| DELETE | `/api/userbook/{bookId}` | Remove book from list |

### ⭐ Review Endpoints
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/review/book/{bookId}` | Get reviews for a book |
| POST | `/api/review` | Create review 🔒 |
| PUT | `/api/review/{id}` | Update review 🔒 |
| DELETE | `/api/review/{id}` | Delete review 🔒 |

### 📝 Blog Endpoints
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/blog` | Get all blogs |
| GET | `/api/blog/{id}` | Get blog by ID |
| POST | `/api/blog` | Create blog post 🔒 |
| PUT | `/api/blog/{id}` | Update blog post 🔒 |
| DELETE | `/api/blog/{id}` | Delete blog post 🔒 |

*🔒 = Requires authentication*

For detailed API documentation, visit `/swagger` when the application is running.

## 🏗️ Architecture

```
TaleTrail.API/
├── Controllers/         # API endpoints
├── Services/           # Business logic
├── DAO/               # Data access layer
├── Model/             # Entity models and DTOs
├── Middleware/        # Custom middleware
├── Extensions/        # Extension methods
└── Data/             # Database seeding
```

## 🔧 Configuration

### Environment Variables

| Variable | Description | Required |
|----------|-------------|----------|
| `SUPABASE_URL` | Your Supabase project URL | ✅ |
| `SUPABASE_KEY` | Your Supabase anon key | ✅ |
| `SUPABASE_JWT_SECRET` | Your Supabase JWT secret | ✅ |
| `ALLOWED_ORIGINS` | CORS allowed origins | ❌ |
| `ASPNETCORE_ENVIRONMENT` | Environment (Development/Production) | ❌ |

### Database Schema

The application uses the following main entities:
- **Users**: User profiles and authentication
- **Books**: Book information with authors and publishers
- **UserBooks**: User's reading lists and progress
- **Reviews**: Book reviews and ratings
- **Blogs**: User blog posts
- **Authors**: Book authors
- **Publishers**: Book publishers

## 🚀 Deployment

### Deploy to Render

1. **Push to GitHub**: Ensure your code is in a GitHub repository
2. **Connect to Render**: Link your GitHub repo to Render
3. **Set Environment Variables**: Configure your Supabase credentials in Render
4. **Deploy**: Render will automatically build and deploy using Docker

### Deploy to Other Platforms

The application is containerized and can be deployed to:
- **Heroku**: Using Docker
- **Railway**: Using Docker
- **DigitalOcean App Platform**: Using Docker
- **AWS ECS/Fargate**: Using Docker
- **Google Cloud Run**: Using Docker

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

- 📖 [Documentation](./docs/)
- 🐛 [Issues](https://github.com/yourusername/taletrail-api/issues)
- 💬 [Discussions](https://github.com/yourusername/taletrail-api/discussions)

## 🙏 Acknowledgments

- [.NET Team](https://github.com/dotnet) for the amazing framework
- [Supabase](https://supabase.com) for the backend infrastructure
- [Swagger](https://swagger.io) for API documentation
- Community contributors and supporters

---

<p align="center">Made with ❤️ for book lovers</p>