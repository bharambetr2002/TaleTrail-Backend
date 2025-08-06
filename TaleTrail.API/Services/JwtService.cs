using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TaleTrail.API.Services
{
    public class JwtService
    {
        private readonly ILogger<JwtService> _logger;
        private readonly string _supabaseJwtSecret;

        public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
        {
            _logger = logger;
            _supabaseJwtSecret = configuration["Supabase:JwtSecret"]
                ?? throw new InvalidOperationException("Supabase:JwtSecret is missing from configuration.");
        }

        /// <summary>
        /// Validates a JWT and extracts its claims.
        /// </summary>
        /// <param name="token">The JWT string.</param>
        /// <returns>A list of claims from the token.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown if the token is invalid or expired.</exception>
        public IEnumerable<Claim> GetClaimsFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_supabaseJwtSecret)),
                    ValidateIssuer = false, // Supabase tokens do not have a standard issuer
                    ValidateAudience = false, // Supabase tokens do not have a standard audience
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                // This will throw an exception if the token is invalid
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

                return principal.Claims;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token validation failed.");
                throw new UnauthorizedAccessException("Invalid or expired token.", ex);
            }
        }
    }
}
