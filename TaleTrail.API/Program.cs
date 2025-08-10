using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using TaleTrail.API.DAO;
using TaleTrail.API.Data;
using TaleTrail.API.Services;
using TaleTrail.API.Extensions;
using TaleTrail.API.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables
DotNetEnv.Env.Load();

// Configure Serilog logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/taletrail-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = false;
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(
            "https://taletrail-app.netlify.app",
            "http://localhost:8080"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// Register Supabase + DAOs + Services
builder.Services.AddSingleton<SupabaseService>();

builder.Services.AddScoped<AuthorDao>();
builder.Services.AddScoped<BookDao>();
builder.Services.AddScoped<PublisherDao>();
builder.Services.AddScoped<ReviewDao>();
builder.Services.AddScoped<UserBookDao>();
builder.Services.AddScoped<UserDao>();
builder.Services.AddScoped<BlogDao>();
builder.Services.AddScoped<BlogLikeDao>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthorService>();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<PublisherService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<UserBookService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<BlogService>();

builder.Services.AddScoped<DataSeeder>();

// JWT authentication
var jwtSecret = Environment.GetEnvironmentVariable("SUPABASE_JWT_SECRET");
if (string.IsNullOrEmpty(jwtSecret))
    throw new InvalidOperationException("SUPABASE_JWT_SECRET is missing");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Health checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy())
    .AddCheck<SupabaseHealthCheck>("supabase");

// Swagger configuration (always enabled)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "TaleTrail API",
        Description = "A clean, simple book tracking API"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

var app = builder.Build();

// Exception middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// CORS — must be before Swagger, Authentication, Authorization
app.UseCors("AllowSpecificOrigins");

// Swagger always available
app.UseSwagger();
app.UseSwaggerUI();

// Auth
app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapHealthChecks("/health");
app.MapControllers();

// Seed data
using (var scope = app.Services.CreateScope())
{
    try
    {
        var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        await seeder.SeedAsync();
        Log.Information("Database seeding completed");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Database seeding failed");
    }
}

Log.Information("TaleTrail API is running!");
app.Run();
