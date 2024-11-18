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

        public async Task<IEnumerable<QuestionInExam>> GetByExamIdAsync(string examId)
        {
            return await _dbContext.QuestionInExams.Where(q => q.ExamId == examId).OrderBy(a => a.Index).ToListAsync();
        }

        public async Task CreateRangeQuestionInExamAsync(IEnumerable<QuestionInExam> questionInExams)
        {
            await _dbContext.QuestionInExams.AddRangeAsync(questionInExams);
        }

        public void UpdateRangeQuestionInExamAsync(IEnumerable<QuestionInExam> questionInExams)
        {
            _dbContext.QuestionInExams.UpdateRange(questionInExams);
        }

        // Delete a range of questionInExams
        public void DeleteRangeQuestionInExamAsync(IEnumerable<QuestionInExam> questionInExams)
        {
            _dbContext.QuestionInExams.RemoveRange(questionInExams);
        }
    }
}
