using Elepla.Repository.Data;
using Elepla.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Repositories
{
    public class ArticleCategoryRepository : IArticleCategoryRepository
    {
        private readonly AppDbContext _dbContext;

        public ArticleCategoryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
