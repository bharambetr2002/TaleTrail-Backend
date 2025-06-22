using TaleTrail.API.Services;
using DotNetEnv; // For loading environment variables from a .env file

// Create a new WebApplicationBuilder instance
var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------
// Add services to the DI container
// -----------------------------------------

builder.Services.AddControllers(); // Add support for MVC controllers

builder.Services.AddSingleton<SupabaseAuthService>(); // Register SupabaseAuthService as a singleton

builder.Services.AddEndpointsApiExplorer(); // Enables discovery of minimal API endpoints (used for Swagger)
builder.Services.AddSwaggerGen(); // Register Swagger generator for API documentation

// -----------------------------------------
// Build the application
// -----------------------------------------
var app = builder.Build();

// -----------------------------------------
// Load environment variables from .env file
// -----------------------------------------
Env.Load(); // Load .env file into environment variables

// Manually assign environment variables to configuration keys
builder.Configuration["Supabase:Url"] = Environment.GetEnvironmentVariable("superbaseUrl");
builder.Configuration["Supabase:Key"] = Environment.GetEnvironmentVariable("superbaseKey");

// -----------------------------------------
// Configure middleware pipeline
// -----------------------------------------

// Enable Swagger only in development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();    // Generate Swagger JSON
    app.UseSwaggerUI();  // Serve Swagger UI
}

app.UseAuthorization();    // Add middleware for handling authorization

app.MapControllers();      // Map controller routes to endpoints

app.Run();                 // Run the application
