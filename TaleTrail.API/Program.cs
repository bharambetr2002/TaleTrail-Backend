using DotNetEnv;
using TaleTrail.API.Services;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load .env file
Env.Load();

// ✅ Read from environment variables and assign to configuration
string supabaseUrl = Environment.GetEnvironmentVariable("supabaseUrl");
string supabaseKey = Environment.GetEnvironmentVariable("supabaseKey");

builder.Configuration["Supabase:Url"] = supabaseUrl;
builder.Configuration["Supabase:Key"] = supabaseKey;

// ✅ Debug output
Console.WriteLine($"[DEBUG] Supabase URL: {supabaseUrl}");
Console.WriteLine($"[DEBUG] Supabase Key: {supabaseKey}");


// ✅ Register services
builder.Services.AddControllers();
builder.Services.AddSingleton<SupabaseAuthService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();