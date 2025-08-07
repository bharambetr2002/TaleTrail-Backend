using Supabase;
using Microsoft.Extensions.Logging;

namespace TaleTrail.API.Services;

public class SupabaseService
{
    public Client Supabase { get; }
    private readonly ILogger<SupabaseService> _logger;

    public SupabaseService(ILogger<SupabaseService> logger)
    {
        _logger = logger;

        var url = Environment.GetEnvironmentVariable("SUPABASE_URL")
            ?? throw new InvalidOperationException("SUPABASE_URL environment variable is missing.");
        var key = Environment.GetEnvironmentVariable("SUPABASE_KEY")
            ?? throw new InvalidOperationException("SUPABASE_KEY environment variable is missing.");

        _logger.LogInformation("Initializing Supabase connection to: {Url}", url);

        var options = new SupabaseOptions
        {
            AutoConnectRealtime = false,
            AutoRefreshToken = false
        };

        try
        {
            Supabase = new Client(url, key, options);
            Supabase.InitializeAsync().Wait();
            _logger.LogInformation("Supabase client initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Supabase client");
            throw new InvalidOperationException("Failed to initialize Supabase connection", ex);
        }
    }
}