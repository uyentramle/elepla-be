using Elepla.Domain.Entities;
using Elepla.Repository.Data;
using Elepla.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Repositories
{
    public class PlanbookInCollectionRepository : IPlanbookInCollectionRepository
    {
        private readonly AppDbContext _dbContext;

        public PlanbookInCollectionRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<PlanbookInCollection>> GetAllByPlanbookId(string planbookId)
        {
            return await _dbContext.PlanbookInCollections.Where(p => p.PlanbookId.Equals(planbookId)).ToListAsync();
        }

        public async Task<PlanbookInCollection?> GetByCollectionIdAndPlanbookId(string collectionId, string planbookId)
        {
            return await _dbContext.PlanbookInCollections.FirstOrDefaultAsync(p => p.CollectionId.Equals(collectionId) && p.PlanbookId.Equals(planbookId));
        }

        public async Task<IEnumerable<PlanbookInCollection>> GetAllByCollectionId(string collectionId)
        {
            return await _dbContext.PlanbookInCollections.Where(p => p.CollectionId.Equals(collectionId)).ToListAsync();
        }

        public async Task<IEnumerable<PlanbookInCollection>> GetAllByCollectionId(IEnumerable<string> collectionIds)
        {
            return await _dbContext.PlanbookInCollections
                                   .Where(p => collectionIds.Contains(p.CollectionId))
                                   .ToListAsync();
        }

        public async Task AddAsync(PlanbookInCollection planbookInCollection)
        {
            await _dbContext.PlanbookInCollections.AddAsync(planbookInCollection);
        }

        public void DeleteRange(IEnumerable<PlanbookInCollection> planbookInCollections)
        {
            _dbContext.PlanbookInCollections.RemoveRange(planbookInCollections);
        }
    }
}
