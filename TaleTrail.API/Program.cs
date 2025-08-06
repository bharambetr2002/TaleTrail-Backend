using TaleTrail.API.Services;
using TaleTrail.API.Middleware;
using TaleTrail.API.DAO;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// Load environment variables FIRST
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// --- Configuration ---
var supabaseJwtSecret = Environment.GetEnvironmentVariable("SUPABASE_JWT_SECRET")
    ?? throw new InvalidOperationException("SUPABASE_JWT_SECRET is missing from your .env file.");

// --- Service Registration ---

// Add .NET Core Authentication with proper Supabase claim mapping
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(supabaseJwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            // Map Supabase claims to .NET claims
            NameClaimType = "sub", // Maps 'sub' claim to NameIdentifier
            RoleClaimType = "role" // Maps 'role' claim to Role
        };
    });

builder.Services.AddAuthorization();

// Register Core Services
builder.Services.AddSingleton<SupabaseService>();
builder.Services.AddSingleton<JwtService>();

// Register all DAOs
builder.Services.AddScoped<AuthorDao>();
builder.Services.AddScoped<BlogDao>();
builder.Services.AddScoped<BlogLikeDao>();
builder.Services.AddScoped<BookDao>();
builder.Services.AddScoped<BookAuthorDao>();
builder.Services.AddScoped<PublisherDao>();
builder.Services.AddScoped<ReviewDao>();
builder.Services.AddScoped<UserDao>();
builder.Services.AddScoped<UserBookDao>();

// Register all Business Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthorService>();
builder.Services.AddScoped<BlogLikeService>();
builder.Services.AddScoped<BlogService>();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<PublisherService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<UserBookService>();
builder.Services.AddScoped<UserService>();

// Add health check
builder.Services.AddHealthChecks();

// Rate limiting
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

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AppCors", policy =>
    {
        policy.AllowAnyOrigin() // Allows requests from any origin
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "TaleTrail API", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

// --- Middleware Pipeline (ORDER MATTERS!) ---
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseCors("AppCors");
app.UseRateLimiter();

if (app.Environment.IsDevelopment() || Environment.GetEnvironmentVariable("ENABLE_SWAGGER") == "true")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}


// CRITICAL: Use built-in Authentication and Authorization middleware (removed custom middleware)
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers().RequireRateLimiting("ApiPolicy");

app.MapGet("/", () => "Welcome to TaleTrail API!");

app.Run();