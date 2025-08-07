using Microsoft.Extensions.Diagnostics.HealthChecks;
using TaleTrail.API.Services;
using TaleTrail.API.Model; // Fixed namespace

namespace TaleTrail.API.Extensions;

/// <summary>
/// Health check for Supabase database connectivity
/// </summary>
public class SupabaseHealthCheck : IHealthCheck
{
    private readonly SupabaseService _supabaseService;
    private readonly ILogger<SupabaseHealthCheck> _logger;

    public SupabaseHealthCheck(SupabaseService supabaseService, ILogger<SupabaseHealthCheck> logger)
    {
        _supabaseService = supabaseService;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Try to perform a simple query to check database connectivity
            var response = await _supabaseService.Supabase
                .From<Author>()
                .Limit(1)
                .Get(cancellationToken);

            if (response != null)
            {
                _logger.LogDebug("Supabase health check passed");
                return HealthCheckResult.Healthy("Supabase is responsive");
            }
            else
            {
                _logger.LogWarning("Supabase health check failed - null response");
                return HealthCheckResult.Unhealthy("Supabase returned null response");
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Supabase health check was cancelled");
            return HealthCheckResult.Unhealthy("Supabase health check was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Supabase health check failed with exception");
            return HealthCheckResult.Unhealthy($"Supabase is not responsive: {ex.Message}");
        }
    }
}