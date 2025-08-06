using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Exceptions;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Services
{
    public class PublisherService
    {
        private readonly SupabaseService _supabase;
        private readonly ILogger<PublisherService> _logger;

        public PublisherService(SupabaseService supabase, ILogger<PublisherService> logger)
        {
            _supabase = supabase;
            _logger = logger;
        }

        public async Task<List<Publisher>> GetAllPublishersAsync()
        {
            try
            {
                var response = await _supabase.Client.From<Publisher>()
                    .Order("name", Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                return response.Models ?? new List<Publisher>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all publishers");
                throw new AppException($"Failed to get publishers: {ex.Message}", ex);
            }
        }

        public async Task<Publisher?> GetPublisherByIdAsync(Guid id)
        {
            try
            {
                var response = await _supabase.Client.From<Publisher>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                    .Get();

                return response.Models?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get publisher {PublisherId}", id);
                throw new AppException($"Failed to get publisher: {ex.Message}", ex);
            }
        }

        public async Task<Publisher> CreatePublisherAsync(PublisherDto publisherDto)
        {
            ValidationHelper.ValidateModel(publisherDto);

            var publisher = new Publisher
            {
                Id = Guid.NewGuid(),
                Name = publisherDto.Name
            };

            try
            {
                var response = await _supabase.Client.From<Publisher>().Insert(publisher);
                var createdPublisher = response.Models?.FirstOrDefault();

                if (createdPublisher == null)
                    throw new AppException("Failed to create publisher - no data returned");

                _logger.LogInformation("Publisher created successfully with ID {PublisherId}", createdPublisher.Id);
                return createdPublisher;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create publisher with name {Name}", publisherDto.Name);
                throw;
            }
        }
    }
}