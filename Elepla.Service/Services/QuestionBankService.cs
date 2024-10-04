using AutoMapper;
using Elepla.Repository.Common;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.QuestionBankViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Services
{
	public class QuestionBankService : IQuestionBankService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public QuestionBankService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<ResponseModel> GetAllQuestionBankAsync(int pageIndex, int pageSize)
		{
			var questions = await _unitOfWork.QuestionBankRepository.GetAsync(
				filter: r => r.IsDeleted.Equals(false),
				pageIndex: pageIndex,
				pageSize: pageSize
				);
			var questionDtos = _mapper.Map<Pagination<ViewListQuestionBankDTO>>(questions);

			return new SuccessResponseModel<object>
			{
				Success = true,
				Message = "Question retrieved successfully.",
				Data = questionDtos
			};
		}
	}
}
