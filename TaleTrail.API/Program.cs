using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TaleTrail.API.DAO;
using TaleTrail.API.Data;
using TaleTrail.API.Services;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

// Add services to the container.
builder.Services.AddControllers();

// Register SupabaseService (Singleton)
builder.Services.AddSingleton<SupabaseService>();

// Register all DAOs (Scoped)
builder.Services.AddScoped<AuthorDao>();
builder.Services.AddScoped<BlogDao>();
builder.Services.AddScoped<BlogLikeDao>();
builder.Services.AddScoped<BookDao>();
builder.Services.AddScoped<PublisherDao>();
builder.Services.AddScoped<ReviewDao>();
builder.Services.AddScoped<UserBookDao>();
builder.Services.AddScoped<UserDao>();

// Register all Services (Scoped)
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthorService>();
builder.Services.AddScoped<BlogService>();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<PublisherService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<UserBookService>();
builder.Services.AddScoped<UserService>();

// Register DataSeeder
builder.Services.AddScoped<DataSeeder>();

// Configure JWT Authentication
var jwtSecret = Environment.GetEnvironmentVariable("SUPABASE_JWT_SECRET");
if (string.IsNullOrEmpty(jwtSecret))
{
    throw new InvalidOperationException("SUPABASE_JWT_SECRET environment variable is missing.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero // Reduces the default 5 minute clock skew
        };
    });

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid JWT token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
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
            new string[]{}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaleTrail API V1");
        c.RoutePrefix = string.Empty; // Makes Swagger available at the root
    });
}

//app.UseHttpsRedirection();

// Authentication & Authorization (order matters!)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed the database on startup
using (var scope = app.Services.CreateScope())
{
    try
    {
        var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
        // Don't throw - let the app start even if seeding fails
    }
}

app.Run();