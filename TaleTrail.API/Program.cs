using TaleTrail.API.Services;
using TaleTrail.API.DAO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// --- SWAGGER CONFIGURATION WITH AUTHORIZE BUTTON ---
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
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
// ---------------------------------------------------


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

        // --- THIS IS THE FINAL, CRITICAL FIX ---
        // This code runs after the token is validated.
        // It reads the custom role from the token's metadata and adds it
        // to the user's identity so [Authorize(Roles="...")] will work.
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                if (context.Principal?.Identity is ClaimsIdentity identity)
                {
                    // Look for the user_metadata claim
                    var metaDataClaim = identity.FindFirst("user_metadata");
                    if (metaDataClaim != null)
                    {
                        // Parse the JSON and find our custom_role
                        using var jsonDoc = JsonDocument.Parse(metaDataClaim.Value);
                        if (jsonDoc.RootElement.TryGetProperty("custom_role", out var roleElement))
                        {
                            var role = roleElement.GetString();
                            if (!string.IsNullOrEmpty(role))
                            {
                                // Add the role to the user's claims
                                identity.AddClaim(new Claim(ClaimTypes.Role, role));
                            }
                        }
                    }
                }
                return Task.CompletedTask;
            }
        };
        // ------------------------------------
    });

builder.Services.AddAuthorization();


// --- DEPENDENCY INJECTION (CORRECTED) ---
builder.Services.AddSingleton<SupabaseService>();

// DAOs
builder.Services.AddScoped<UserDao>();
builder.Services.AddScoped<BookDao>();
builder.Services.AddScoped<BookAuthorDao>();
builder.Services.AddScoped<AuthorDao>();
builder.Services.AddScoped<PublisherDao>();
builder.Services.AddScoped<ReviewDao>();
builder.Services.AddScoped<BlogDao>();
builder.Services.AddScoped<BlogLikeDao>();
builder.Services.AddScoped<UserBookDao>();

// Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthorService>();
builder.Services.AddScoped<PublisherService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<BlogService>();
builder.Services.AddScoped<BlogLikeService>();
builder.Services.AddScoped<UserBookService>();
// -----------------------------------------

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();