using Elepla.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Interfaces
{
    public interface IPlanbookInCollectionRepository
    {
        Task<IEnumerable<PlanbookInCollection>> GetAllByPlanbookId(string planbookId);
        Task<PlanbookInCollection?> GetByCollectionIdAndPlanbookId(string collectionId, string planbookId);
        Task<IEnumerable<PlanbookInCollection>> GetAllByCollectionId(string collectionId);
        Task<IEnumerable<PlanbookInCollection>> GetAllByCollectionId(IEnumerable<string> collectionIds);
        Task AddAsync(PlanbookInCollection planbookInCollection);
        void DeleteRange(IEnumerable<PlanbookInCollection> planbookInCollections);
    }
}
