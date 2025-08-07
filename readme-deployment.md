# 🚀 TaleTrail API - Complete Deployment Guide

## 📁 **File Structure After Updates**

Your project should now have these updated/new files:

```
TaleTrail.API/
├── Controllers/
│   ├── BaseController.cs (UPDATED) ✅
│   ├── DiagnosticController.cs (NEW) 🆕
│   └── UserBookController.cs (UPDATED) ✅
├── Middleware/
│   ├── ErrorHandlerMiddleware.cs (UPDATED) ✅
│   └── SupabaseAuthMiddleware.cs (UPDATED) ✅
├── Services/
│   ├── AuthService.cs (UPDATED) ✅
│   └── JwtService.cs (UPDATED) ✅
├── Program.cs (UPDATED) ✅
├── appsettings.Production.json (UPDATED) ✅
├── render.yaml (NEW) 🆕
├── .env.template (NEW) 🆕
└── README-DEPLOYMENT.md (NEW) 🆕
```

---

## 🏃‍♂️ **Quick Start - Local Development**

### 1. **Setup Environment**

```bash
# Copy environment template
cp .env.template .env

# Edit .env with your Supabase credentials
nano .env
```

### 2. **Install & Run**

```bash
# Restore packages
dotnet restore

# Run in development mode
dotnet run

# Your API will be available at:
# http://localhost:5082
# http://localhost:5082/swagger
```

### 3. **Test Your Setup**

```bash
# Test health endpoint
curl http://localhost:5082/health

# Test diagnostic endpoints
curl http://localhost:5082/api/diagnostic/health
curl http://localhost:5082/api/diagnostic/env-check
```

---

## 🌐 **Render.com Deployment**

### **Method 1: Using render.yaml (Recommended)**

1. **Add render.yaml to your repository root**
2. **Connect to Render:**

   - Go to [render.com](https://render.com)
   - Connect your GitHub repository
   - Render will automatically detect `render.yaml`

3. **Set Environment Variables in Render Dashboard:**
   ```bash
   SUPABASE_URL=https://your-project.supabase.co
   SUPABASE_KEY=your-supabase-anon-key
   SUPABASE_JWT_SECRET=your-jwt-secret
   ```

### **Method 2: Manual Setup**

1. **Create New Web Service on Render**
2. **Build Settings:**

   ```bash
   # Build Command:
   dotnet publish -c Release -o out

   # Start Command:
   dotnet out/TaleTrail.API.dll
   ```

3. **Environment Variables:**
   - Add all variables from `.env.template`
   - Set `ASPNETCORE_ENVIRONMENT=Production`

---

## 🧪 **Testing Your Deployed API**

### **1. Basic Health Check**

```bash
curl https://your-app.onrender.com/health
```

**Expected Response:**

```json
{
  "status": "healthy",
  "timestamp": "2024-01-20T10:30:00Z",
  "environment": "Production",
  "environmentVariables": {
    "SUPABASE_URL": true,
    "SUPABASE_KEY": true,
    "SUPABASE_JWT_SECRET": true
  }
}
```

### **2. System Diagnostic**

```bash
curl https://your-app.onrender.com/api/diagnostic/health
```

### **3. Complete Authentication Flow Test**

**Step 1 - Signup:**

```bash
curl -X POST https://your-app.onrender.com/api/auth/signup \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "password123",
    "fullName": "Test User"
  }'
```

**Step 2 - Login:**

```bash
curl -X POST https://your-app.onrender.com/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "password123"
  }'
```

**Step 3 - Test Protected Endpoint:**

```bash
curl -X GET https://your-app.onrender.com/api/user/profile/my-profile \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN_HERE"
```

---

## 🔧 **Configuration Details**

### **Environment Variables Explained**

| Variable                 | Description               | Example                      |
| ------------------------ | ------------------------- | ---------------------------- |
| `SUPABASE_URL`           | Your Supabase project URL | `https://abc123.supabase.co` |
| `SUPABASE_KEY`           | Supabase anon/public key  | `eyJhbGciOiJIUzI1NiIs...`    |
| `SUPABASE_JWT_SECRET`    | JWT signing secret        | `your-jwt-secret`            |
| `ASPNETCORE_ENVIRONMENT` | Application environment   | `Development` / `Production` |

### **Getting Supabase Credentials**

1. **Go to your Supabase Dashboard**
2. **Select your project**
3. **Navigate to Settings > API**
4. **Copy the following:**
   - **Project URL** → `SUPABASE_URL`
   - **anon/public key** → `SUPABASE_KEY`
   - **JWT Secret** → `SUPABASE_JWT_SECRET`

---

## 🛡️ **Security Checklist**

### **Before Going to Production:**

- [ ] **Remove/Secure Diagnostic Controller**

  ```csharp
  // Add to DiagnosticController.cs
  [Authorize(Roles = "admin")]
  public class DiagnosticController : BaseController
  ```

- [ ] **Update CORS Policy**

  ```csharp
  // In Program.cs, replace:
  policy.AllowAnyOrigin()

  // With:
  policy.WithOrigins("https://your-frontend-domain.com")
  ```

- [ ] **Review Logging Levels**

  ```json
  // In appsettings.Production.json
  "LogLevel": {
    "Default": "Warning",
    "TaleTrail.API": "Information"
  }
  ```

- [ ] **Enable HTTPS Only**
  - Render provides HTTPS automatically
  - Verify `RequireHttps` in production settings

---

## 🚨 **Troubleshooting Common Issues**

### **Issue: "Application failed to start"**

**Solutions:**

1. Check Render logs for specific error
2. Verify all environment variables are set
3. Test build locally: `dotnet publish -c Release`
4. Check `appsettings.Production.json` syntax

### **Issue: "Database connection failed"**

**Solutions:**

1. Verify `SUPABASE_URL` and `SUPABASE_KEY`
2. Check Supabase project is active
3. Test connection: `curl https://your-project.supabase.co/rest/v1/`

### **Issue: "JWT validation failed"**

**Solutions:**

1. Verify `SUPABASE_JWT_SECRET` matches Supabase exactly
2. Check token format at jwt.io
3. Use diagnostic endpoint: `/api/diagnostic/validate-token`

### **Issue: "CORS errors from frontend"**

**Solutions:**

1. Update CORS origins in `Program.cs`
2. Check preflight OPTIONS requests are allowed
3. Verify frontend is using correct API URL

---

## 📊 **Monitoring & Maintenance**

### **Health Monitoring Endpoints**

- `GET /health` - Basic health check
- `GET /api/diagnostic/health` - Detailed system health
- `GET /api/diagnostic/env-check` - Environment validation

### **Logging**

- Render provides automatic log collection
- View logs in Render dashboard
- Structured logging with user context included

### **Performance Monitoring**

```bash
# Check response times
curl -w "@curl-format.txt" -s -o /dev/null https://your-app.onrender.com/health

# Create curl-format.txt:
echo "time_total: %{time_total}" > curl-format.txt
```

---

## ✅ **Deployment Success Checklist**

Your TaleTrail API is successfully deployed when:

- [ ] ✅ Health endpoint returns 200 OK
- [ ] ✅ Swagger UI loads at `/swagger`
- [ ] ✅ User can signup and login
- [ ] ✅ JWT tokens work for protected endpoints
- [ ] ✅ Database operations complete successfully
- [ ] ✅ Error responses are consistent and informative
- [ ] ✅ CORS configured for your frontend domain
- [ ] ✅ Diagnostic endpoints secured or removed

**🎉 Your backend is now bulletproof and ready for production!**

---

## 📞 **Getting Help**

If you encounter issues:

1. Check the troubleshooting section above
2. Review Render deployment logs
3. Test locally first with the same configuration
4. Use diagnostic endpoints to isolate problems
5. Verify Supabase credentials and permissions

**Happy coding! 🚀**
