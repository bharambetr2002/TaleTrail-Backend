using TaleTrail.API.Model;
using TaleTrail.API.Model.DTOs;
using TaleTrail.API.DAO;

namespace TaleTrail.API.Services;

public class PublisherService
{
    private readonly PublisherDao _publisherDao;

    public PublisherService(PublisherDao publisherDao)
    {
        _publisherDao = publisherDao;
    }

    public async Task<List<PublisherResponseDTO>> GetAllPublishersAsync()
    {
        var publishers = await _publisherDao.GetAllAsync();
        return publishers.Select(MapToPublisherResponseDTO).ToList();
    }

    public async Task<PublisherResponseDTO?> GetPublisherByIdAsync(Guid id)
    {
        var publisher = await _publisherDao.GetByIdAsync(id);
        return publisher == null ? null : MapToPublisherResponseDTO(publisher);
    }

    private static PublisherResponseDTO MapToPublisherResponseDTO(Publisher publisher)
    {
        return new PublisherResponseDTO
        {
            Id = publisher.Id,
            Name = publisher.Name,
            Description = publisher.Description,
            Address = publisher.Address,
            FoundedYear = publisher.FoundedYear,
            BookCount = 0 // TODO: Calculate book count if needed
        };
    }
}