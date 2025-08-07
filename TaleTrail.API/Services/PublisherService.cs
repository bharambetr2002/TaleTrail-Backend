using TaleTrail.API.Model;
using TaleTrail.API.DAO;

namespace TaleTrail.API.Services;

public class PublisherService
{
    private readonly PublisherDao _publisherDao;

    public PublisherService(PublisherDao publisherDao)
    {
        _publisherDao = publisherDao;
    }

    public async Task<List<Publisher>> GetAllPublishersAsync()
    {
        return await _publisherDao.GetAllAsync();
    }

    public async Task<Publisher?> GetPublisherByIdAsync(Guid id)
    {
        return await _publisherDao.GetByIdAsync(id);
    }
}