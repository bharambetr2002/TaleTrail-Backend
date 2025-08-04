using TaleTrail.API.Services;
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
});

var app = builder.Build();

// ✅ Always enable Swagger (for testing)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaleTrail API V1");
    c.RoutePrefix = "swagger"; // Swagger URL = /swagger
});

// ✅ Configure HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// ✅ Enable routing to API controllers
app.MapControllers();

app.Run();