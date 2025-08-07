using TaleTrail.API.Services;
using TaleTrail.API.DAO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Load .env file for local development
DotNetEnv.Env.Load();

// 1. Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT Authentication
var jwtSecret = Environment.GetEnvironmentVariable("SUPABASE_JWT_SECRET")
    ?? throw new InvalidOperationException("SUPABASE_JWT_SECRET is missing.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });
builder.Services.AddAuthorization();


// --- DEPENDENCY INJECTION ---
builder.Services.AddSingleton<SupabaseService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserDao>();
builder.Services.AddScoped<BookDao>();
builder.Services.AddScoped<BookService>();

// --- THIS IS THE FIX ---
// We must register BookAuthorDao because BookService needs it.
builder.Services.AddScoped<BookAuthorDao>();
// ----------------------


var app = builder.Build();

// 2. Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();