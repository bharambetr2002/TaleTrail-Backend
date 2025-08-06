using TaleTrail.API.Services;
using TaleTrail.API.Middleware;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// 🔐 Load Environment Variables
DotNetEnv.Env.Load();

// ✅ Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TaleTrail API", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddScoped<SupabaseService>();
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
app.UseMiddleware<JwtMiddleware>();

// ✅ Always enable Swagger (in dev or prod)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaleTrail API V1");
    c.RoutePrefix = "swagger"; // Swagger UI at /swagger
});

app.MapControllers();
app.MapGet("/", () => "🎉 TaleTrail backend is live!");

app.Run();
