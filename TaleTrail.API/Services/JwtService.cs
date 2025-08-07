using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;

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
        /// Validates a Supabase JWT and extracts claims: sub, email, and role.
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
                    _logger.LogWarning("Token format is invalid: {Token}", token[..Math.Min(20, token.Length)] + "...");
                    throw new UnauthorizedAccessException("Token format is invalid.");
                }

                // Read token (without verifying yet)
                var jwt = handler.ReadJwtToken(token);
                var claims = new List<Claim>();

                // Extract sub (userId)
                var userId = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                if (!string.IsNullOrEmpty(userId))
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));

                // Extract email
                var email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                if (!string.IsNullOrEmpty(email))
                    claims.Add(new Claim(ClaimTypes.Email, email));

                // Extract role (if missing, default to user)
                var role = jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value ?? "user";
                claims.Add(new Claim(ClaimTypes.Role, role));

                // Signature + expiry validation
                var validationParams = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_supabaseJwtSecret)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                // Validates signature + expiration (throws if fails)
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
                throw new UnauthorizedAccessException("Invalid Supabase token.", ex);
            }
        }

        /// <summary>
        /// Checks if the JWT has a valid format (3 parts separated by dots).
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
