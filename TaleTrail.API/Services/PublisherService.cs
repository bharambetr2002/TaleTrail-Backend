using TaleTrail.API.DAO;
using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TaleTrail.API.Services
{
    public class PublisherService
    {
        private readonly PublisherDao _publisherDao;
        private readonly ILogger<PublisherService> _logger;

        public PublisherService(PublisherDao publisherDao, ILogger<PublisherService> logger)
        {
            _publisherDao = publisherDao;
            _logger = logger;
        }

        public async Task<List<Publisher>> GetAllPublishersAsync()
        {
            return await _publisherDao.GetAllAsync();
        }

        public async Task<Publisher?> GetPublisherByIdAsync(Guid id)
        {
            return await _publisherDao.GetByIdAsync(id);
        }

        public async Task<Publisher> CreatePublisherAsync(PublisherDto publisherDto)
        {
            var publisher = new Publisher
            {
                Name = publisherDto.Name
            };

            var createdPublisher = await _publisherDao.AddAsync(publisher);
            if (createdPublisher == null)
            {
                throw new AppException("Failed to create publisher.");
            }
            return createdPublisher;
        }

        public async Task<Publisher?> UpdatePublisherAsync(Guid id, PublisherDto publisherDto)
        {
            var existingPublisher = await _publisherDao.GetByIdAsync(id);
            if (existingPublisher == null)
            {
                return null; // Not found
            }

            existingPublisher.Name = publisherDto.Name;
            return await _publisherDao.UpdateAsync(existingPublisher);
        }

        public async Task<bool> DeletePublisherAsync(Guid id)
        {
            var existingPublisher = await _publisherDao.GetByIdAsync(id);
            if (existingPublisher == null)
            {
                return false; // Not found
            }

            await _publisherDao.DeleteAsync(id);
            return true;
        }
    }
}
