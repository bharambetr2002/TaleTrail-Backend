using Supabase.Gotrue;
using TaleTrail.API.DAO;
using TaleTrail.API.DTOs.Auth;
using UserModel = TaleTrail.API.Models.User;

namespace TaleTrail.API.Services
{
    public class AuthService
    {
        private readonly Supabase.Client _supabaseClient;
        private readonly UserDao _userDao;

        public AuthService(SupabaseService supabaseService, UserDao userDao)
        {
            _supabaseClient = supabaseService.Client;
            _userDao = userDao;
        }

        public async Task<Session> SignupAsync(SignupDTO request)
        {
            // 1. Create the user in Supabase Auth FIRST.
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

            // 2. CRITICAL: Create the user in YOUR database using Supabase's UUID.
            var newUser = new UserModel
            {
                Id = Guid.Parse(session.User.Id), // Use the ID from Supabase
                Email = request.Email,
                Username = request.Username,
                FullName = request.FullName,
                Role = "user" // Assign a default role
            };

            await _userDao.AddAsync(newUser);

            return session;
        }

        public async Task<Session> LoginAsync(LoginDTO request)
        {
            // Simply sign in using Supabase. If it fails, it will throw an exception.
            var session = await _supabaseClient.Auth.SignIn(request.Email, request.Password);
            return session;
        }
    }
}