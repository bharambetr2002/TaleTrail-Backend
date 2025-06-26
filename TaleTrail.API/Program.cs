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
// Load environment variables from .env file
// -----------------------------------------

Env.Load(); // Load .env file into environment variables

// Manually assign environment variables to configuration keys
builder.Configuration["Supabase:Url"] = Environment.GetEnvironmentVariable("superbaseUrl");
builder.Configuration["Supabase:Key"] = Environment.GetEnvironmentVariable("superbaseKey");

// -----------------------------------------
// Build the application
// -----------------------------------------

var app = builder.Build();

// -----------------------------------------
// Configure middleware pipeline
// -----------------------------------------

app.UseSwagger();    // Generate Swagger JSON (enabled for all environments)
app.UseSwaggerUI();  // Serve Swagger UI

app.UseAuthorization();    // Add middleware for handling authorization

app.MapControllers();      // Map controller routes to endpoints

app.MapGet("/", () => "📚 TaleTrail API is up and running!"); // Test root route for Render health check

app.Urls.Add("http://*:8080"); // Tell Render to bind to port 8080

app.Run(); // Run the application
