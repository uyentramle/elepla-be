using AutoMapper;
using Elepla.Domain.Entities;
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

		public async Task<ResponseModel> GetQuestionBankByIdAsync(string id)
		{
			var question = await _unitOfWork.QuestionBankRepository.GetByIdAsync(id);
			if (question == null)
			{
				return new ResponseModel
				{
					Success = false,
					Message = "Question not found."
				};
			}

			var questionDto = _mapper.Map<ViewListQuestionBankDTO>(question);

			return new SuccessResponseModel<object>
			{
				Success = true,
				Message = "Question retrieved successfully.",
				Data = questionDto
			};
		}

		public async Task<ResponseModel> CreateQuestionAsync(CreateQuestionDTO model)
		{
			try
			{
				var question = _mapper.Map<QuestionBank>(model);
				await _unitOfWork.QuestionBankRepository.AddAsync(question);
				await _unitOfWork.SaveChangeAsync();

				return new ResponseModel
				{
					Success = true,
					Message = "Question create successfully."
				};
			}
			catch (Exception ex)
			{
				return new ErrorResponseModel<object>
				{
					Success = false,
					Message = ex.Message
				};
			}
		}

		public async Task<ResponseModel> UpdateQuestionAsync(UpdateQuestionDTO model)
		{
			try
			{
				var question = await _unitOfWork.QuestionBankRepository.GetByIdAsync(model.QuestionId);
				if (question == null)
				{
					return new ResponseModel
					{
						Success = false,
						Message = "Question not found."
					};
				}
				if (question.IsDeleted == true)
				{
					return new ResponseModel
					{
						Success = false,
						Message = "Can't modify question is deleted."
					};
				}

				_mapper.Map(model, question);
				_unitOfWork.QuestionBankRepository.Update(question);
				await _unitOfWork.SaveChangeAsync();

				return new ResponseModel
				{
					Success = true,
					Message = "Question updated successfully."
				};
			}
			catch (Exception ex)
			{
				return new ErrorResponseModel<object>
				{
					Success = false,
					Message = ex.Message
				};
			}
		}

		public async Task<ResponseModel> DeleteQuestionAsync(string id)
		{
			try
			{
				var question = await _unitOfWork.QuestionBankRepository.GetByIdAsync(id);
				if (question == null)
				{
					return new ResponseModel
					{
						Success = false,
						Message = "Question not found."
					};
				}
				if (question.IsDeleted == true)
				{
					return new ResponseModel
					{
						Success = false,
						Message = "Can't delete question is deleted."
					};
				}

				_unitOfWork.QuestionBankRepository.SoftRemove(question);
				await _unitOfWork.SaveChangeAsync();

				return new ResponseModel
				{
					Success = true,
					Message = "Question deleted successfully."
				};
			}
			catch (Exception ex)
			{
				return new ErrorResponseModel<object>
				{
					Success = false,
					Message = ex.Message
				};
			}
		}
	}
}
