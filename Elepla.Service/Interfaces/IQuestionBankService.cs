using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.QuestionBankViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Interfaces
{
	public interface IQuestionBankService
	{
		Task<ResponseModel> GetAllQuestionBankAsync(string? keyword, int pageIndex, int pageSize);
		Task<ResponseModel> GetQuestionBankByIdAsync(string id);
		Task<ResponseModel> GetQuestionsByChapterIdAsync(string chapterId, int pageIndex, int pageSize);
		Task<ResponseModel> GetQuestionsByLessonIdAsync(string lessonId, int pageIndex, int pageSize);
		Task<ResponseModel> CreateQuestionAsync(CreateQuestionDTO model);
		Task<ResponseModel> UpdateQuestionAsync(UpdateQuestionDTO model);
		Task<ResponseModel> DeleteQuestionAsync(string id);
		Task<ResponseModel> GetAllQuestionByUserId(string userId, int pageIndex, int pageSize);
		Task<ResponseModel> GetQuestionByUserIdAndChapterId(string userId, string chapterId, int pageIndex, int pageSize);
		Task<ResponseModel> GetQuestionByUserIdAndLessonId(string userId, string lessonId, int pageIndex, int pageSize);
    }
}
