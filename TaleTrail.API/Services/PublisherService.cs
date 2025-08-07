using TaleTrail.API.DAO;
using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaleTrail.API.Services
{
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

        public async Task<Publisher> CreatePublisherAsync(PublisherDto publisherDto)
        {
            var publisher = new Publisher { Name = publisherDto.Name };
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
            if (existingPublisher == null) return null;

            existingPublisher.Name = publisherDto.Name;
            return await _publisherDao.UpdateAsync(existingPublisher);
        }

        public async Task<bool> DeletePublisherAsync(Guid id)
        {
            var existingPublisher = await _publisherDao.GetByIdAsync(id);
            if (existingPublisher == null) return false;

            await _publisherDao.DeleteAsync(id);
            return true;
        }
    }
}