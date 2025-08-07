using Supabase.Gotrue;
using TaleTrail.API.DAO;
using TaleTrail.API.DTOs.Auth;
using TaleTrail.API.Services;

namespace TaleTrail.API.Services
{
    public class AuthService
    {
        private readonly Supabase.Client _supabaseClient;

        // We no longer need the UserDao here for signup.
        public AuthService(SupabaseService supabaseService)
        {
            _supabaseClient = supabaseService.Client;
        }

        public async Task<Session> SignupAsync(SignupDTO request)
        {
            // This method NOW ONLY creates the user in Supabase Auth.
            // The user profile in public.users will be created automatically on their first API call.
            var session = await _supabaseClient.Auth.SignUp(request.Email, request.Password, new SignUpOptions
            {
                Data = new Dictionary<string, object>
                {
                    { "full_name", request.FullName },
                    { "username", request.Username }
                }
            });

            if (session?.User?.Id == null)
            {
                throw new Exception("Supabase signup failed.");
            }

            return session;
        }

        public async Task<Session> LoginAsync(LoginDTO request)
        {
            // Login remains the same; it just validates credentials and returns a session.
            return await _supabaseClient.Auth.SignIn(request.Email, request.Password);
        }
    }
}