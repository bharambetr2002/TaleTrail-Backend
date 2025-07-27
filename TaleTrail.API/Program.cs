using TaleTrail.API.Services;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<SupabaseService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Env.Load();
builder.Configuration["Supabase:Key"] = Environment.GetEnvironmentVariable("superbaseKey");

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();