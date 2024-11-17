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
        Task<IEnumerable<QuestionInExam>> GetByExamIdAsync(string examId);
        Task CreateRangeQuestionInExamAsync(IEnumerable<QuestionInExam> questionInExams);
        void UpdateRangeQuestionInExamAsync(IEnumerable<QuestionInExam> questionInExams);
        void DeleteRangeQuestionInExamAsync(IEnumerable<QuestionInExam> questionInExams);
    }
}
