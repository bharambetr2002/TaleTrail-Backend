# 📚 TaleTrail API

A comprehensive book tracking and blogging platform built with **ASP.NET Core 8** and **Supabase**. TaleTrail allows users to manage their reading journey, write blogs, review books, and maintain personalized watchlists.

## 🚀 Features

### Core Functionality
- **User Authentication** - Secure signup/login with Supabase Auth
- **Book Management** - Add, update, delete, and search books
- **Personal Blogs** - Write and manage personal reading blogs
- **Book Reviews** - Rate and review books (1-5 stars)
- **Reading Watchlist** - Track books with status (to_read, reading, completed, dropped)
- **Author & Publisher Management** - Manage book metadata
- **Categories** - Organize books by genres/categories
- **Feedback System** - User feedback collection
- **Subscription Management** - User plan management

### Technical Features
- **RESTful API** with Swagger documentation
- **Modular Architecture** - Controllers, Services, DAOs, DTOs
- **Exception Handling** with custom middleware
- **Data Validation** with attributes and custom validators
- **CORS Support** for frontend integration
- **Environment Configuration** with .env files
- **Health Check** endpoint

## 🛠️ Tech Stack

- **Framework**: ASP.NET Core 8
- **Database**: Supabase (PostgreSQL)
- **Authentication**: Supabase Auth
- **Documentation**: Swagger/OpenAPI
- **Environment**: DotNetEnv
- **Validation**: Data Annotations + Custom Validators

## 📁 Project Structure

```
TaleTrail.API/
├── Controllers/          # API Controllers
├── Services/            # Business Logic Layer
├── DAO/                 # Data Access Objects
├── DTOs/                # Data Transfer Objects
├── Models/              # Entity Models
├── Middleware/          # Custom Middleware
├── Validations/         # Validation Logic
├── Helpers/             # Utility Classes
├── Exceptions/          # Custom Exceptions
├── Constants/           # Application Constants
└── Enums/              # Enumerations
```

## 🚦 Getting Started

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022 or VS Code
- Supabase Account

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd TaleTrail
   ```

2. **Create environment file**
   ```bash
   cp TaleTrail.API/.env.example TaleTrail.API/.env
   ```

3. **Configure Supabase**
   - Create a new project on [Supabase](https://supabase.com)
   - Update the `.env` file with your Supabase URL and API key
   ```
   superbaseUrl=your-supabase-url
   superbaseKey=your-supabase-anon-key
   ```

4. **Restore dependencies**
   ```bash
   dotnet restore
   ```

5. **Run the application**
   ```bash
   cd TaleTrail.API
   dotnet run
   ```

6. **Access the application**
   - API: `https://localhost:7055`
   - Swagger: `https://localhost:7055/swagger`
   - Health Check: `https://localhost:7055/health`

## 📖 API Documentation

### Authentication Endpoints
- `POST /api/auth/signup` - User registration
- `POST /api/auth/login` - User login
- `POST /api/auth/logout` - User logout

### Book Management
- `GET /api/book` - Get all books
- `GET /api/book/{id}` - Get book by ID
- `POST /api/book` - Create new book
- `PUT /api/book/{id}` - Update book
- `DELETE /api/book/{id}` - Delete book

### Blog Management
- `GET /api/blog` - Get all blogs
- `GET /api/blog/user/{userId}` - Get user blogs
- `POST /api/blog` - Create new blog
- `PUT /api/blog/{id}` - Update blog
- `DELETE /api/blog/{id}` - Delete blog

### Reviews
- `GET /api/review/book/{bookId}` - Get book reviews
- `GET /api/review/user/{userId}` - Get user reviews
- `POST /api/review` - Create review
- `PUT /api/review/{id}` - Update review
- `DELETE /api/review/{id}` - Delete review

### Watchlist
- `GET /api/watchlist/user/{userId}` - Get user watchlist
- `POST /api/watchlist` - Add to watchlist
- `PUT /api/watchlist/{id}` - Update watchlist status
- `DELETE /api/watchlist/{id}` - Remove from watchlist

## 🗄️ Database Schema

The application uses Supabase with the following main tables:
- `users` - User profiles
- `books` - Book information
- `authors` - Author details
- `publishers` - Publisher information
- `categories` - Book categories
- `reviews` - Book reviews
- `blogs` - User blogs
- `watchlist` - Reading watchlist
- `feedback` - User feedback
- `subscriptions` - User subscriptions

## 🔧 Configuration

### Environment Variables
```bash
# Supabase
superbaseUrl=your-supabase-url
superbaseKey=your-supabase-key

# Application
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=https://localhost:7055;http://localhost:5082
```

### appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

## 🚀 Deployment

### Using GitHub Actions
The project includes a GitHub Actions workflow for automatic deployment:

```yaml
name: 🚀 Deploy .NET Backend
on:
  push:
    branches: [main]
jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v4
    - uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'
    - run: dotnet restore TaleTrail.API/TaleTrail.API.csproj
    - run: dotnet build TaleTrail.API/TaleTrail.API.csproj --configuration Release
    - run: dotnet publish TaleTrail.API/TaleTrail.API.csproj -c Release -o ./publish
```

### Manual Deployment
1. **Build for production**
   ```bash
   dotnet publish -c Release -o publish
   ```

2. **Deploy to your hosting provider**
   - Upload the `publish` folder contents
   - Set environment variables on the server
   - Configure the web server (IIS, Nginx, etc.)

## 🧪 Testing

### Running Tests
```bash
dotnet test
```

### API Testing with Swagger
1. Navigate to `/swagger`
2. Test endpoints directly from the browser
3. Use the "Try it out" feature for each endpoint

### Sample API Calls

**User Registration:**
```json
POST /api/auth/signup
{
  "email": "user@example.com",
  "password": "password123",
  "fullName": "John Doe"
}
```

**Create Book:**
```json
POST /api/book
{
  "title": "The Great Gatsby",
  "description": "A classic American novel",
  "language": "English",
  "publicationYear": 1925
}
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

If you encounter any issues or have questions:
1. Check the [Issues](https://github.com/username/taletrail/issues) page
2. Create a new issue with detailed information
3. Contact the development team

## 🎯 Roadmap

- [ ] JWT token refresh mechanism
- [ ] Email verification
- [ ] Password reset functionality
- [ ] Book recommendation system
- [ ] Social features (following users)
- [ ] Advanced search and filtering
- [ ] Image upload for book covers
- [ ] Export reading data
- [ ] Mobile app integration

## 🌟 Acknowledgments

- Built with [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)
- Database and Auth by [Supabase](https://supabase.com/)
- Documentation with [Swagger](https://swagger.io/)

---

**Made with ❤️ for book lovers everywhere!**