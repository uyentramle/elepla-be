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
	public class QuestionBankRepository : GenericRepository<QuestionBank>, IQuestionBankRepository
	{
		public QuestionBankRepository(AppDbContext dbContext, ITimeService timeService, IClaimsService claimsService) : base(dbContext, timeService, claimsService)
		{
		}

		public async Task<QuestionBank> GetByQuestionIdAsync(string id)
		{
			return await _dbContext.QuestionBanks
				.Include(q => q.Chapter)
				.Include(q => q.Lesson)
				.Include(q => q.Answers)
				.FirstOrDefaultAsync(q => q.QuestionId == id);
		}
	}
}
