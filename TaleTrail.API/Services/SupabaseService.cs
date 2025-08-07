using Supabase;

namespace TaleTrail.API.Services
{
    public class SupabaseService
    {
        private readonly Client _client;

        public SupabaseService()
        {
            // Get credentials from environment variables (Render will provide these)
            var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL")
                ?? throw new InvalidOperationException("SUPABASE_URL environment variable is missing");

            var supabaseKey = Environment.GetEnvironmentVariable("SUPABASE_KEY")
                ?? throw new InvalidOperationException("SUPABASE_KEY environment variable is missing");

            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = false // Disable realtime for API-only usage
            };

            _client = new Client(supabaseUrl, supabaseKey, options);

            try
            {
                // Initialize the client
                Task.Run(async () => await _client.InitializeAsync()).Wait(TimeSpan.FromSeconds(30));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to initialize Supabase client: {ex.Message}", ex);
            }
        }

        public Client Client => _client;
    }
}