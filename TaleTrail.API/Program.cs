using TaleTrail.API.Services;
using TaleTrail.API.DAO;
using TaleTrail.API.Middleware;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// 🔐 Load Environment Variables FIRST
DotNetEnv.Env.Load();

// ✅ Validate Critical Environment Variables
var requiredEnvVars = new[] { "SUPABASE_URL", "SUPABASE_KEY", "SUPABASE_JWT_SECRET" };
foreach (var envVar in requiredEnvVars)
{
    if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(envVar)))
    {
        throw new InvalidOperationException($"Required environment variable {envVar} is missing");
    }
}

// ✅ Configure JSON serialization
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// ✅ Add services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TaleTrail API",
        Version = "v1",
        Description = "A comprehensive book tracking and social reading platform API"
    });

    // ✅ JWT Authentication configuration for Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token (without 'Bearer' prefix). Example: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ✅ CORS Configuration - Wide open for development, restrict in production
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            // TODO: Restrict CORS for production
            policy.WithOrigins("https://your-frontend-domain.com")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

// ✅ JWT Authentication Configuration
var jwtSecret = Environment.GetEnvironmentVariable("SUPABASE_JWT_SECRET");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // ✅ Enhanced event handling for debugging
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogWarning("JWT Authentication failed: {Error}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                var userId = context.Principal?.FindFirst("sub")?.Value;
                logger.LogDebug("JWT Token validated for user: {UserId}", userId);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// ✅ Register Core Services
builder.Services.AddScoped<SupabaseService>();
builder.Services.AddScoped<JwtService>();

// ✅ Register All DAOs
builder.Services.AddScoped<UserDao>();
builder.Services.AddScoped<AuthorDao>();
builder.Services.AddScoped<BookDao>();
builder.Services.AddScoped<BookAuthorDao>();
builder.Services.AddScoped<PublisherDao>();
builder.Services.AddScoped<ReviewDao>();
builder.Services.AddScoped<BlogDao>();
builder.Services.AddScoped<BlogLikeDao>();
builder.Services.AddScoped<UserBookDao>();

// ✅ Register All Business Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthorService>();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<PublisherService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<BlogService>();
builder.Services.AddScoped<BlogLikeService>();
builder.Services.AddScoped<UserBookService>();

// ✅ Rate Limiting
builder.Services.AddRateLimiter(_ => _
    .AddFixedWindowLimiter("fixed", options =>
    {
        options.Window = TimeSpan.FromSeconds(10);
        options.PermitLimit = 50;
        options.QueueLimit = 0;
    })
);

var app = builder.Build();

// ✅ CRITICAL: CORRECT Middleware pipeline order
app.UseCors();                                       // 1. CORS first

// ✅ Error Handling Middleware BEFORE authentication
app.UseMiddleware<ErrorHandlerMiddleware>();         // 2. Error handling early

app.UseRateLimiter();                                // 3. Rate limiting

// ✅ Request Logging for debugging
if (app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        logger.LogInformation("📨 {Method} {Path} | Auth: {Auth}",
            context.Request.Method,
            context.Request.Path,
            authHeader != null ? $"Bearer {authHeader[..Math.Min(20, authHeader.Length)]}..." : "Missing");
        await next(context);
    });
}

app.UseAuthentication();                             // 4. Authentication
app.UseMiddleware<SupabaseAuthMiddleware>();         // 5. Custom auth middleware  
app.UseAuthorization();                              // 6. Authorization last

// ✅ Always enable Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaleTrail API V1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "TaleTrail API Documentation";
});

app.MapControllers();

// ✅ Root endpoint
app.MapGet("/", () => Results.Json(new
{
    message = "🎉 TaleTrail backend is live!",
    version = "v1.0.0",
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName,
    endpoints = new
    {
        swagger = "/swagger",
        health = "/health"
    }
}));

// ✅ Health Check Endpoint with detailed info
app.MapGet("/health", () =>
{
    var envCheck = new Dictionary<string, bool>
    {
        ["SUPABASE_URL"] = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SUPABASE_URL")),
        ["SUPABASE_KEY"] = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SUPABASE_KEY")),
        ["SUPABASE_JWT_SECRET"] = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SUPABASE_JWT_SECRET"))
    };

    return Results.Json(new
    {
        status = "healthy",
        timestamp = DateTime.UtcNow,
        environment = app.Environment.EnvironmentName,
        environmentVariables = envCheck
    });
});

// ✅ Enhanced startup logging
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("🚀 TaleTrail API starting up...");
logger.LogInformation("📍 Environment: {Environment}", app.Environment.EnvironmentName);
logger.LogInformation("🔗 Swagger UI: {SwaggerUrl}", app.Environment.IsDevelopment() ? "http://localhost:5082/swagger" : "/swagger");

app.Run();