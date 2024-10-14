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
    public class ArticleImageRepository : IArticleImageRepository
    {
        private readonly AppDbContext _dbContext;

        public ArticleImageRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

		public async Task<ArticleImage?> GetByArticleIdAsync(string articleId)
		{
			return await _dbContext.ArticleImages
				.Where(ai => ai.ArticleId == articleId)
				.FirstOrDefaultAsync();
		}
	}
}
