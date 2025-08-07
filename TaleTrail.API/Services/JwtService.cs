using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TaleTrail.API.Services
{
    public class JwtService
    {
        private readonly ILogger<JwtService> _logger;
        private readonly string _supabaseJwtSecret;

        public JwtService(ILogger<JwtService> logger)
        {
            _logger = logger;
            _supabaseJwtSecret = Environment.GetEnvironmentVariable("SUPABASE_JWT_SECRET")
                ?? throw new InvalidOperationException("SUPABASE_JWT_SECRET is missing from environment variables.");
        }

        /// <summary>
        /// Validates a Supabase JWT and extracts claims
        /// </summary>
        public IEnumerable<Claim> GetClaimsFromToken(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    throw new UnauthorizedAccessException("Token is null or empty.");

                // Remove Bearer prefix if present
                if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    token = token.Substring(7);

                var handler = new JwtSecurityTokenHandler();

                if (!handler.CanReadToken(token))
                {
                    _logger.LogWarning("Invalid token format");
                    throw new UnauthorizedAccessException("Token format is invalid.");
                }

                // Read token without verification first
                var jwt = handler.ReadJwtToken(token);
                var claims = new List<Claim>();

                // Extract standard claims
                var userId = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                if (!string.IsNullOrEmpty(userId))
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));

                var email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                if (!string.IsNullOrEmpty(email))
                    claims.Add(new Claim(ClaimTypes.Email, email));

                // Extract role (default to 'user' if not found)
                var role = jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value ?? "user";
                claims.Add(new Claim(ClaimTypes.Role, role));

                // Now validate the token signature and expiration
                var validationParams = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_supabaseJwtSecret)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                handler.ValidateToken(token, validationParams, out _);

                return claims;
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning("Token validation failed: {Error}", ex.Message);
                throw new UnauthorizedAccessException("Token validation failed.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during token validation");
                throw new UnauthorizedAccessException("Invalid token.", ex);
            }
        }

        /// <summary>
        /// Checks if JWT has valid format
        /// </summary>
        public bool IsValidJwtFormat(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return false;

            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                token = token.Substring(7);

            return token.Split('.').Length == 3;
        }
    }
}