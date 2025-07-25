using TaleTrail.API.Services;
using DotNetEnv;
using TaleTrail.API.Services.Interfaces; // For loading environment variables from a .env file

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------
// Add services to the DI container
// -----------------------------------------

builder.Services.AddControllers(); // Add support for MVC controllers

builder.Services.AddSingleton<SupabaseAuthService>(); // Supabase Auth handler

builder.Services.AddScoped<IBookService, BookService>(); // ✅ Register BookService for BookController

// Future Services to register here:
// builder.Services.AddScoped<IBlogService, BlogService>();
// builder.Services.AddScoped<IReviewService, ReviewService>();
// etc.

builder.Services.AddEndpointsApiExplorer(); // Enables Swagger
builder.Services.AddSwaggerGen();           // Generates Swagger UI

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

app.UseSwagger();         // Enable Swagger docs
app.UseSwaggerUI();       // Serve Swagger UI

app.UseAuthorization();   // Apply authorization middleware

app.MapControllers();     // Map controller endpoints

app.MapGet("/", () => "📚 TaleTrail API is up and running!"); // Health check root

app.Urls.Add("http://*:8080"); // Bind for Render (host:port)

app.Run(); // Run the app
