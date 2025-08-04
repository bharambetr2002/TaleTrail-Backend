using TaleTrail.API.Services;
using TaleTrail.API.Middleware;
using DotNetEnv;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load environment variables from .env
Env.Load();

// ✅ Read Supabase env variables
var supabaseUrl = Environment.GetEnvironmentVariable("superbaseUrl");
var supabaseKey = Environment.GetEnvironmentVariable("superbaseKey");

// ✅ Log Supabase key presence for debugging
Console.WriteLine($"Supabase URL: {supabaseUrl}");
Console.WriteLine($"Supabase Key: {(string.IsNullOrEmpty(supabaseKey) ? "MISSING" : "LOADED")}");

// ✅ Add Supabase config to builder config
builder.Configuration["Supabase:Url"] = supabaseUrl;
builder.Configuration["Supabase:Key"] = supabaseKey;

// ✅ Register services
builder.Services.AddSingleton<SupabaseService>();

// Register business services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<BlogService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<WatchlistService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TaleTrail API",
        Version = "v1",
        Description = "API documentation for TaleTrail backend using Supabase"
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
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
            new string[] {}
        }
    });
});

var app = builder.Build();

// ✅ Configure HTTP pipeline

// Add error handling middleware first
app.UseMiddleware<ErrorHandlerMiddleware>();

// ✅ Always enable Swagger (for testing)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaleTrail API V1");
    c.RoutePrefix = "swagger"; // Swagger URL = /swagger
});

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Enable CORS
app.UseCors("AllowAll");

app.UseRouting();

// Add auth middleware (optional for now)
// app.UseMiddleware<SupabaseAuthMiddleware>();

app.UseAuthorization();

// ✅ Enable routing to API controllers
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

Console.WriteLine("🚀 TaleTrail API is running!");
Console.WriteLine("📖 Swagger UI: /swagger");
Console.WriteLine("🏥 Health Check: /health");

app.Run();