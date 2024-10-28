using Elepla.Domain.Entities;
using Elepla.Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Interfaces
{
	public interface IAnswerRepository
	{
		Task<List<Answer>> GetByQuestionIdAsync(string questionId);
		Task<Answer> GetByQuestionIdAndTextAsync(string questionId, string answerText);
		Task<Answer> CreateAnswerAsync(Answer answer);
		Task<Answer> UpdateAnswerAsync(Answer answer);
		Task<bool> DeleteAnswerAsync(Answer answer);
	}
}
