using TaleTrail.API.Services;
using TaleTrail.API.Middleware;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// 🛠 Supabase Setup
var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL")
    ?? builder.Configuration["Supabase:Url"];
var supabaseKey = Environment.GetEnvironmentVariable("SUPABASE_KEY")
    ?? builder.Configuration["Supabase:Key"];

if (string.IsNullOrWhiteSpace(supabaseUrl) || string.IsNullOrWhiteSpace(supabaseKey))
    throw new InvalidOperationException("Supabase URL and Key must be configured");

builder.Configuration["Supabase:Url"] = supabaseUrl;
builder.Configuration["Supabase:Key"] = supabaseKey;

// 🛠 Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// 🛠 Services
builder.Services.AddSingleton<SupabaseService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<BlogService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<WatchlistService>();

// 🛠 Health Checks
builder.Services.AddHealthChecks().AddCheck("supabase", () =>
    !string.IsNullOrEmpty(supabaseUrl)
        ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy()
        : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy());

// 🛠 Rate Limiting
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

// 🛠 CORS
var allowedOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?
    .Split(',', StringSplitOptions.RemoveEmptyEntries)
    ?? new[] { "http://localhost:3000" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionCors", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// 🛠 Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TaleTrail API",
        Version = "v1.0.0"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Bearer token format: Bearer {token}",
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

// 🧱 Secure headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

    if (!app.Environment.IsDevelopment())
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");

    await next();
});

// 🧱 Middleware
app.UseMiddleware<ErrorHandlerMiddleware>();

// 🧱 Swagger
var enableSwagger = app.Environment.IsDevelopment() ||
                    Environment.GetEnvironmentVariable("ENABLE_SWAGGER") == "true";

if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaleTrail API V1");
        c.RoutePrefix = app.Environment.IsDevelopment() ? "swagger" : "docs";
    });
}

// 🧱 Optional HTTPS redirection
if (!app.Environment.IsDevelopment())
{
    // Commented because Render or cloud handles HTTPS already
    // app.UseHttpsRedirection();
}

// 🧱 CORS + Routing
app.UseCors(app.Environment.IsDevelopment() ? "AllowAll" : "ProductionCors");
app.UseRateLimiter();
app.UseRouting();

// 🧱 (Auth Middleware later)
// app.UseMiddleware<SupabaseAuthMiddleware>();

app.UseAuthorization();

// ✅ Health Endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live");

// ✅ Controllers
app.MapControllers().RequireRateLimiting("ApiPolicy");

// ✅ Root + Version
app.MapGet("/", () => Results.Ok(new
{
    service = "TaleTrail API",
    version = "1.0.0",
    environment = app.Environment.EnvironmentName,
    timestamp = DateTime.UtcNow
}));

app.MapGet("/version", () => Results.Ok(new
{
    version = "1.0.0",
    buildDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
    environment = app.Environment.EnvironmentName
}));

// ✅ Startup log
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("🚀 TaleTrail API starting up...");
logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
logger.LogInformation("Supabase URL: {SupabaseUrl}", supabaseUrl);

app.Run();