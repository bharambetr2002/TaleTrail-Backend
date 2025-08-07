using Supabase;

namespace TaleTrail.API.Services;

public class SupabaseService
{
    public Client Supabase { get; }

    public SupabaseService()
    {
        var url = Environment.GetEnvironmentVariable("SUPABASE_URL")
            ?? throw new InvalidOperationException("SUPABASE_URL environment variable is missing.");
        var key = Environment.GetEnvironmentVariable("SUPABASE_KEY")
            ?? throw new InvalidOperationException("SUPABASE_KEY environment variable is missing.");

        var options = new SupabaseOptions
        {
            AutoConnectRealtime = false
        };

        Supabase = new Client(url, key, options);
        Supabase.InitializeAsync().Wait();
    }
}
