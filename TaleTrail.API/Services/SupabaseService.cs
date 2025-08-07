// File: Services/SupabaseService.cs
using Supabase;
using Postgrest;

namespace TaleTrail.API.Services;

public class SupabaseService
{
    public Client Supabase { get; }

    public SupabaseService()
    {
        var url = Environment.GetEnvironmentVariable("SUPABASE_URL") ?? "";
        var key = Environment.GetEnvironmentVariable("SUPABASE_KEY") ?? "";

        var options = new SupabaseOptions
        {
            AutoConnectRealtime = false
        };

        Supabase = new Client(url, key, options);
        Supabase.InitializeAsync().Wait();
    }
}