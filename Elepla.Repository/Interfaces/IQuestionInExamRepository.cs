using Elepla.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Interfaces
{
    public interface IQuestionInExamRepository
    {
        Task AddAsync(QuestionInExam entity);
        Task<IEnumerable<QuestionInExam>> GetAsync(System.Linq.Expressions.Expression<System.Func<QuestionInExam, bool>> filter);
        void DeleteRange(IEnumerable<QuestionInExam> entities);
    }
}
