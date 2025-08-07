# 🛠️ TaleTrail API Setup Guide

This guide will help you set up TaleTrail API for development and production.

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- ✅ [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- ✅ [Git](https://git-scm.com/)
- ✅ [Docker](https://www.docker.com/get-started) (optional but recommended)
- ✅ A [Supabase](https://supabase.com) account

## 🚀 Local Development Setup

### Step 1: Clone the Repository

```bash
git clone https://github.com/yourusername/taletrail-api.git
cd taletrail-api
```

### Step 2: Set Up Supabase

1. **Create a new Supabase project**

   - Go to [Supabase](https://supabase.com)
   - Click "New Project"
   - Choose your organization and set project details

2. **Get your Supabase credentials**

   - Go to Project Settings > API
   - Copy the `URL` and `anon/public` key
   - Go to Project Settings > Auth > JWT Settings
   - Copy the `JWT Secret`

3. **Set up the database schema**

   Run the following SQL in your Supabase SQL Editor:

   ```sql
   -- Create enum for reading status
   CREATE TYPE reading_status AS ENUM ('ToRead', 'InProgress', 'Completed', 'Dropped');

   -- Enable RLS on auth.users
   ALTER TABLE auth.users ENABLE ROW LEVEL SECURITY;

   -- Create users table (extends auth.users)
   CREATE TABLE users (
       id UUID PRIMARY KEY REFERENCES auth.users(id) ON DELETE CASCADE,
       email TEXT UNIQUE NOT NULL,
       full_name TEXT,
       username TEXT UNIQUE NOT NULL,
       bio TEXT,
       avatar_url TEXT,
       created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
       updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
   );

   -- Create other tables
   CREATE TABLE authors (
       id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
       name TEXT NOT NULL,
       bio TEXT,
       birth_date DATE,
       death_date DATE,
       created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
       updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
   );

   CREATE TABLE publishers (
       id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
       name TEXT NOT NULL,
       description TEXT,
       address TEXT,
       founded_year INTEGER,
       created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
       updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
   );

   CREATE TABLE books (
       id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
       title TEXT NOT NULL,
       description TEXT,
       language TEXT,
       cover_image_url TEXT,
       publication_year INTEGER,
       publisher_id UUID REFERENCES publishers(id),
       created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
       updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
   );

   CREATE TABLE book_authors (
       id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
       book_id UUID REFERENCES books(id) ON DELETE CASCADE,
       author_id UUID REFERENCES authors(id) ON DELETE CASCADE,
       created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
       UNIQUE(book_id, author_id)
   );

   CREATE TABLE user_books (
       id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
       user_id UUID REFERENCES users(id) ON DELETE CASCADE,
       book_id UUID REFERENCES books(id) ON DELETE CASCADE,
       reading_status reading_status DEFAULT 'ToRead',
       progress INTEGER DEFAULT 0 CHECK (progress >= 0 AND progress <= 100),
       started_at TIMESTAMP WITH TIME ZONE,
       completed_at TIMESTAMP WITH TIME ZONE,
       created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
       updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
       UNIQUE(user_id, book_id)
   );

   CREATE TABLE reviews (
       id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
       user_id UUID REFERENCES users(id) ON DELETE CASCADE,
       book_id UUID REFERENCES books(id) ON DELETE CASCADE,
       rating INTEGER NOT NULL CHECK (rating >= 1 AND rating <= 5),
       content TEXT,
       created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
       updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
       UNIQUE(user_id, book_id)
   );

   CREATE TABLE blogs (
       id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
       user_id UUID REFERENCES users(id) ON DELETE CASCADE,
       title TEXT NOT NULL,
       content TEXT NOT NULL,
       created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
       updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
   );

   CREATE TABLE blog_likes (
       id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
       blog_id UUID REFERENCES blogs(id) ON DELETE CASCADE,
       user_id UUID REFERENCES users(id) ON DELETE CASCADE,
       created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
       UNIQUE(blog_id, user_id)
   );

   -- Enable RLS on all tables
   ALTER TABLE users ENABLE ROW LEVEL SECURITY;
   ALTER TABLE authors ENABLE ROW LEVEL SECURITY;
   ALTER TABLE publishers ENABLE ROW LEVEL SECURITY;
   ALTER TABLE books ENABLE ROW LEVEL SECURITY;
   ALTER TABLE book_authors ENABLE ROW LEVEL SECURITY;
   ALTER TABLE user_books ENABLE ROW LEVEL SECURITY;
   ALTER TABLE reviews ENABLE ROW LEVEL SECURITY;
   ALTER TABLE blogs ENABLE ROW LEVEL SECURITY;
   ALTER TABLE blog_likes ENABLE ROW LEVEL SECURITY;

   -- Create RLS policies (basic read access for public data)
   CREATE POLICY "Public read access" ON authors FOR SELECT USING (true);
   CREATE POLICY "Public read access" ON publishers FOR SELECT USING (true);
   CREATE POLICY "Public read access" ON books FOR SELECT USING (true);
   CREATE POLICY "Public read access" ON book_authors FOR SELECT USING (true);
   CREATE POLICY "Public read access" ON reviews FOR SELECT USING (true);
   CREATE POLICY "Public read access" ON blogs FOR SELECT USING (true);
   CREATE POLICY "Public read access" ON users FOR SELECT USING (true);

   -- User-specific policies
   CREATE POLICY "Users can manage own data" ON users FOR ALL USING (auth.uid() = id);
   CREATE POLICY "Users can manage own books" ON user_books FOR ALL USING (auth.uid() = user_id);
   CREATE POLICY "Users can manage own reviews" ON reviews FOR ALL USING (auth.uid() = user_id);
   CREATE POLICY "Users can manage own blogs" ON blogs FOR ALL USING (auth.uid() = user_id);
   CREATE POLICY "Users can manage own likes" ON blog_likes FOR ALL USING (auth.uid() = user_id);

   -- Function to create user profile automatically
   CREATE OR REPLACE FUNCTION create_user_profile(
       user_id UUID,
       user_email TEXT,
       full_name TEXT,
       username TEXT
   ) RETURNS VOID AS $$
   BEGIN
       INSERT INTO users (id, email, full_name, username)
       VALUES (user_id, user_email, full_name, username)
       ON CONFLICT (id) DO UPDATE SET
           email = EXCLUDED.email,
           full_name = EXCLUDED.full_name,
           username = EXCLUDED.username,
           updated_at = NOW();
   END;
   $$ LANGUAGE plpgsql;
   ```

### Step 3: Configure Environment Variables

1. **Copy the example environment file**

   ```bash
   cp .env.example .env
   ```

2. **Edit the `.env` file**

   ```bash
   # Replace with your actual Supabase credentials
   SUPABASE_URL=https://your-project.supabase.co
   SUPABASE_KEY=your_supabase_anon_key
   SUPABASE_JWT_SECRET=your_supabase_jwt_secret

   # Set allowed origins for CORS
   ALLOWED_ORIGINS=http://localhost:3000,http://localhost:5173

   # Set environment
   ASPNETCORE_ENVIRONMENT=Development
   ```

### Step 4: Run the Application

#### Option A: Using .NET CLI (Recommended for development)

```bash
cd TaleTrail.API
dotnet restore
dotnet run
```

The API will be available at:

- **HTTP**: http://localhost:5198
- **HTTPS**: https://localhost:7203
- **Swagger**: http://localhost:5198/swagger

#### Option B: Using Docker

```bash
# Build and run with docker-compose
docker-compose up -d

# Or build and run manually
docker build -t taletrail-api -f TaleTrail.API/Dockerfile .
docker run -p 8080:8080 --env-file .env taletrail-api
```

The API will be available at http://localhost:8080

### Step 5: Verify Installation

1. **Check health endpoint**

   ```bash
   curl http://localhost:5198/health
   ```

   Should return: `Healthy`

2. **Check API documentation**
   Open http://localhost:5198/swagger in your browser

3. **Test basic endpoints**

   ```bash
   # Get all books
   curl http://localhost:5198/api/book

   # Get all authors
   curl http://localhost:5198/api/author
   ```

## 🌐 Production Deployment

### Deploy to Render

1. **Push your code to GitHub**

2. **Create a new Web Service on Render**

   - Connect your GitHub repository
   - Use Docker as the environment
   - Set the Docker command to: `dockerfile TaleTrail.API/Dockerfile`

3. **Set environment variables in Render**

   - Go to your service settings
   - Add all the environment variables from your `.env` file

4. **Deploy**
   - Render will automatically build and deploy your application
   - Your API will be available at `https://your-app-name.onrender.com`

### Deploy to Other Platforms

The application is containerized and can be deployed to any platform that supports Docker:

- **Heroku**: Use the Heroku Container Registry
- **Railway**: Connect your GitHub repo and use Docker
- **DigitalOcean App Platform**: Use Docker
- **AWS ECS/Fargate**: Use the Docker image
- **Google Cloud Run**: Use the Docker image

## 🔧 Development Tips

### Hot Reload

```bash
dotnet watch run
```

### View Logs

```bash
# In development
tail -f TaleTrail.API/logs/taletrail-*.txt

# With Docker
docker-compose logs -f taletrail-api
```

### Database Migrations

The application uses database seeding. Sample data will be automatically created on first run.

### Testing API Endpoints

Use the built-in Swagger UI at `/swagger` or tools like:

- Postman
- Insomnia
- Thunder Client (VS Code extension)
- curl

## 🚨 Troubleshooting

### Common Issues

1. **"Could not find the column in schema cache"**

   - Verify your database schema matches the entity models
   - Check that all tables exist in Supabase

2. **JWT Authentication Errors**

   - Verify your `SUPABASE_JWT_SECRET` is correct
   - Check that the JWT token is not expired

3. **CORS Errors**

   - Add your frontend domain to `ALLOWED_ORIGINS`
   - Verify CORS configuration in `Program.cs`

4. **Docker Build Issues**
   - Ensure all files are properly copied in Dockerfile
   - Check that the working directory is correct

### Getting Help

- Check the [Issues](https://github.com/yourusername/taletrail-api/issues) page
- Review the [API Documentation](./API.md)
- Check application logs for detailed error messages

## ✅ Next Steps

Once you have the API running:

1. **Test all endpoints** using Swagger UI
2. **Create your first user** via `/api/auth/signup`
3. **Add some books** to your reading list
4. **Write your first review**
5. **Create a blog post**
6. **Set up your frontend** to consume the API

Happy coding! 🚀
