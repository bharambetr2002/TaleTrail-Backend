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

// 🔐 Load Environment Variables
DotNetEnv.Env.Load();

// ✅ Configure JSON serialization to handle Supabase models
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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TaleTrail API", Version = "v1" });

    // ✅ FIXED: Better JWT Authentication configuration for Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\""
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

// ✅ CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ✅ JWT Authentication Configuration
var jwtSecret = Environment.GetEnvironmentVariable("SUPABASE_JWT_SECRET");
if (!string.IsNullOrEmpty(jwtSecret))
{
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
}

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

// ✅ Middleware pipeline
app.UseCors();
app.UseRateLimiter();

// ✅ Error Handling Middleware
app.UseMiddleware<ErrorHandlerMiddleware>();

// ✅ Authentication & Authorization
app.UseAuthentication();
app.UseMiddleware<SupabaseAuthMiddleware>();
app.UseAuthorization();

// ✅ Always enable Swagger (in dev or prod)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaleTrail API V1");
    c.RoutePrefix = "swagger"; // Swagger UI at /swagger
});

app.MapControllers();
app.MapGet("/", () => "🎉 TaleTrail backend is live!");

// ✅ Health Check Endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run();