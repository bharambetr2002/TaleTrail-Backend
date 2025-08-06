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
        /// Validates a JWT and extracts its claims, properly mapping Supabase claims to .NET standard claims.
        /// </summary>
        /// <param name="token">The JWT string.</param>
        /// <returns>A list of claims from the token.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown if the token is invalid or expired.</exception>
        public IEnumerable<Claim> GetClaimsFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                // First, read the token to extract claims without validation
                var jsonToken = tokenHandler.ReadJwtToken(token);

                var claims = new List<Claim>();

                // Map Supabase claims to .NET standard claims
                if (jsonToken.Claims.FirstOrDefault(x => x.Type == "sub")?.Value is string userId)
                {
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
                }

                if (jsonToken.Claims.FirstOrDefault(x => x.Type == "role")?.Value is string role)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                // Add email if present
                if (jsonToken.Claims.FirstOrDefault(x => x.Type == "email")?.Value is string email)
                {
                    claims.Add(new Claim(ClaimTypes.Email, email));
                }

                // Add username if present in user_metadata
                if (jsonToken.Claims.FirstOrDefault(x => x.Type == "user_metadata")?.Value is string userMetadata)
                {
                    // You might need to parse the JSON in user_metadata to extract username
                    // For now, we'll skip this unless needed
                }

                // Now validate the token signature and expiration
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_supabaseJwtSecret)),
                    ValidateIssuer = false, // Supabase tokens do not have a standard issuer
                    ValidateAudience = false, // Supabase tokens do not have a standard audience
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                // This will throw an exception if the token is invalid or expired
                tokenHandler.ValidateToken(token, validationParameters, out _);

                return claims;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token validation failed.");
                throw new UnauthorizedAccessException("Invalid or expired token.", ex);
            }
        }
    }
}