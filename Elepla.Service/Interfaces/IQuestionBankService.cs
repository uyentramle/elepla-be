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
		Task<ResponseModel> GetAllQuestionBankAsync(int pageIndex, int pageSize);
		Task<ResponseModel> GetQuestionBankByIdAsync(string id);
		Task<ResponseModel> CreateQuestionAsync(CreateQuestionDTO model);
	}
}
