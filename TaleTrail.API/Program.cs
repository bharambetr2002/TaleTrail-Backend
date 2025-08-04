using TaleTrail.API.Services;
using TaleTrail.API.Middleware;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ✅ Configure Supabase from environment variables or appsettings
var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL")
    ?? builder.Configuration["Supabase:Url"];
var supabaseKey = Environment.GetEnvironmentVariable("SUPABASE_KEY")
    ?? builder.Configuration["Supabase:Key"];

if (string.IsNullOrEmpty(supabaseUrl) || string.IsNullOrEmpty(supabaseKey))
{
    throw new InvalidOperationException("Supabase URL and Key must be configured");
}

// ✅ Add Supabase config to builder configuration
builder.Configuration["Supabase:Url"] = supabaseUrl;
builder.Configuration["Supabase:Key"] = supabaseKey;

// ✅ Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ✅ Register services
builder.Services.AddSingleton<SupabaseService>();

// Register business services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<BlogService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<WatchlistService>();

// ✅ Add health checks
builder.Services.AddHealthChecks()
    .AddCheck("supabase", () =>
    {
        return !string.IsNullOrEmpty(supabaseUrl) ?
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy() :
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy();
    });

// ✅ Add rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("ApiPolicy", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 10;
    });
});

// ✅ Configure CORS for production
var allowedOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?
    .Split(',', StringSplitOptions.RemoveEmptyEntries)
    ?? new[] { "http://localhost:3000", "https://localhost:3001" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionCors", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    // Keep AllowAll for development
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ✅ Configure Swagger for different environments
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TaleTrail API",
        Version = "v1.0.0",
        Description = "A comprehensive API for book tracking and reviews",
        Contact = new OpenApiContact
        {
            Name = "TaleTrail Support",
            Email = "support@taletrail.com"
        }
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
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

// ✅ Configure HTTP pipeline

// Add security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

    if (!app.Environment.IsDevelopment())
    {
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    }

    await next();
});

// Add error handling middleware first
app.UseMiddleware<ErrorHandlerMiddleware>();

// ✅ Configure Swagger based on environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaleTrail API V1");
        c.RoutePrefix = "swagger";
    });
}
else
{
    // In production, only enable Swagger if explicitly configured
    var enableSwagger = Environment.GetEnvironmentVariable("ENABLE_SWAGGER") == "true";
    if (enableSwagger)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaleTrail API V1");
            c.RoutePrefix = "docs"; // Change route for production
        });
    }

    app.UseHsts();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

// ✅ Enable CORS based on environment
var corsPolicy = app.Environment.IsDevelopment() ? "AllowAll" : "ProductionCors";
app.UseCors(corsPolicy);

// ✅ Add rate limiting
app.UseRateLimiter();

app.UseRouting();

// Add auth middleware (commented out until JWT implementation is complete)
// app.UseMiddleware<SupabaseAuthMiddleware>();

app.UseAuthorization();

// ✅ Map health checks
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live");

// ✅ Apply rate limiting to API routes
app.MapControllers().RequireRateLimiting("ApiPolicy");

// ✅ Root endpoint
app.MapGet("/", () => Results.Ok(new
{
    service = "TaleTrail API",
    version = "1.0.0",
    environment = app.Environment.EnvironmentName,
    timestamp = DateTime.UtcNow,
    status = "healthy"
}));

// ✅ Version endpoint
app.MapGet("/version", () => Results.Ok(new
{
    version = "1.0.0",
    buildDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
    environment = app.Environment.EnvironmentName
}));

// ✅ Startup logging
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("🚀 TaleTrail API starting up...");
logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
logger.LogInformation("Supabase URL: {SupabaseUrl}", supabaseUrl);

if (app.Environment.IsDevelopment())
{
    logger.LogInformation("📖 Swagger UI: /swagger");
}

logger.LogInformation("🏥 Health checks: /health, /health/ready, /health/live");
logger.LogInformation("🌐 API Base URL: {BaseUrl}", app.Urls.FirstOrDefault() ?? "Not configured");

app.Run();