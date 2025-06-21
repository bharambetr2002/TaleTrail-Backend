using TaleTrail.API.Services;
using DotNetEnv;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddSingleton<SupabaseAuthService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
Env.Load();
builder.Configuration["Supabase:Url"] = Environment.GetEnvironmentVariable("superbaseUrl");
builder.Configuration["Supabase:Key"] = Environment.GetEnvironmentVariable("superbaseKey");

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
