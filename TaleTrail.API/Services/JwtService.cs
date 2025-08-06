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
                ?? throw new InvalidOperationException("SUPABASE_JWT_SECRET environment variable is missing");
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_supabaseJwtSecret)),
                    ValidateIssuer = false, // Supabase handles this
                    ValidateAudience = false, // Supabase handles this
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token validation failed");
                throw new UnauthorizedAccessException("Invalid or expired token");
            }
        }

        public Guid GetUserIdFromToken(string token)
        {
            var principal = ValidateToken(token);
            var subClaim = principal.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(subClaim) || !Guid.TryParse(subClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID in token");
            }

            return userId;
        }

        public string? GetUserEmailFromToken(string token)
        {
            var principal = ValidateToken(token);
            return principal.FindFirst("email")?.Value;
        }
    }
}