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
    public class QuestionInExamRepository : IQuestionInExamRepository
    {
        private readonly AppDbContext _dbContext;

        public QuestionInExamRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Add an entity
        public async Task AddAsync(QuestionInExam entity)
        {
            await _dbContext.QuestionInExams.AddAsync(entity);
        }

        // Get entities based on a filter
        public async Task<IEnumerable<QuestionInExam>> GetAsync(System.Linq.Expressions.Expression<System.Func<QuestionInExam, bool>> filter)
        {
            return await _dbContext.QuestionInExams.Where(filter).ToListAsync();
        }

        // Delete a range of entities
        public void DeleteRange(IEnumerable<QuestionInExam> entities)
        {
            _dbContext.QuestionInExams.RemoveRange(entities);
        }
    }
}
