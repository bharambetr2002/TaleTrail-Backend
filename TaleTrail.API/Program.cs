using TaleTrail.API.Services;
using TaleTrail.API.Middleware;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

// ✅ Load required environment variables
var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL");
var supabaseKey = Environment.GetEnvironmentVariable("SUPABASE_KEY");
var supabaseJwtSecret = Environment.GetEnvironmentVariable("SUPABASE_JWT_SECRET");

if (string.IsNullOrEmpty(supabaseUrl) || string.IsNullOrEmpty(supabaseKey))
    throw new InvalidOperationException("Supabase credentials are missing");

if (string.IsNullOrEmpty(supabaseJwtSecret))
    throw new InvalidOperationException("SUPABASE_JWT_SECRET environment variable is missing. Get it from Supabase Dashboard > Settings > API");

// ✅ Register core services
builder.Services.AddSingleton<SupabaseService>();
builder.Services.AddSingleton<JwtService>();

// ✅ Register all business services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<BlogService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<WatchlistService>();
builder.Services.AddScoped<FeedbackService>();
builder.Services.AddScoped<SubscriptionService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthorService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<PublisherService>();

// ✅ Add health check for Supabase
builder.Services.AddHealthChecks().AddCheck("supabase", () =>
    !string.IsNullOrEmpty(supabaseUrl)
        ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy()
        : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy());

// ✅ Rate limiting
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

// ✅ CORS policy
var allowedOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?
    .Split(',', StringSplitOptions.RemoveEmptyEntries)
    ?? new[] { "http://localhost:3000" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AppCors", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TaleTrail API",
        Version = "v1",
        Description = "Backend API for book tracking with JWT Authentication",
        Contact = new OpenApiContact
        {
            Name = "TaleTrail",
            Email = "support@taletrail.com"
        }
    });

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// ✅ Configure Kestrel for deployment
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(int.Parse(port));
});

var app = builder.Build();

// ✅ Middleware pipeline (ORDER MATTERS!)
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseCors("AppCors");
app.UseRateLimiter();

// ✅ Enable Swagger
if (app.Environment.IsDevelopment() || Environment.GetEnvironmentVariable("ENABLE_SWAGGER") == "true")
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaleTrail API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseStaticFiles();
app.UseRouting();

// ✅ CRITICAL: Enable JWT Authentication Middleware
app.UseMiddleware<SupabaseAuthMiddleware>();

app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers().RequireRateLimiting("ApiPolicy");

app.MapGet("/", () => Results.Ok(new
{
    service = "TaleTrail API",
    version = "1.0.0",
    status = "healthy",
    environment = app.Environment.EnvironmentName,
    time = DateTime.UtcNow,
    port = port,
    authentication = "JWT-Enabled"
}));

app.Run();