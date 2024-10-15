using AutoMapper;
using Elepla.Domain.Entities;
using Elepla.Repository.Common;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.ActivityViewModels;
using Elepla.Service.Models.ViewModels.PlanbookViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Services
{
	public class PlanbookService : IPlanbookService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public PlanbookService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		#region Get All Planbooks
		public async Task<ResponseModel> GetAllPlanbooksAsync(int pageIndex, int pageSize)
		{
			var planbooks = await _unitOfWork.PlanbookRepository.GetAsync(
							filter: r => r.IsDeleted == false,
							pageIndex: pageIndex,
							pageSize: pageSize
							);
			var mappers = _mapper.Map<Pagination<ViewListPlanbookDTO>>(planbooks);
			foreach (var item in mappers.Items)
			{
				var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(item.LessonId);
				if (lesson != null)
				{
					item.LessonName = lesson.Name;
				}
			}

			return new SuccessResponseModel<object>
			{
				Success = true,
				Message = "Planbooks retrieved successfully.",
				Data = mappers
			};
		}
		#endregion

		#region Get Planbook By Id
		public async Task<ResponseModel> GetPlanbookByIdAsync(string planbookId)
		{
			var planbook = await _unitOfWork.PlanbookRepository.GetByIdAsync(planbookId);
			if (planbook == null)
			{
				return new ResponseModel
				{
					Success = false,
					Message = "Planbook not found."
				};
			}

			var mapper = _mapper.Map<ViewDetailPlanbookDTO>(planbook);

			var collection = await _unitOfWork.PlanbookCollectionRepository.GetByIdAsync(planbook.CollectionId);
			if (collection != null)
			{
				mapper.CollectionName = collection.CollectionName;
			}

			var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(planbook.LessonId);
			if (lesson != null)
			{
				mapper.LessonName = lesson.Name;
			}

			var activities = await _unitOfWork.ActivityRepository.GetByPlanbookIdAsync(planbookId);
			mapper.Activities = _mapper.Map<List<ViewListActivityDTO>>(activities);

			return new SuccessResponseModel<object>
			{
				Success = true,
				Message = "Planbook retrieved successfully.",
				Data = mapper
			};
		}
		#endregion

		#region Get Planbook By Collection Id
		public async Task<ResponseModel> GetPlanbookByCollectionIdAsync(string collectionId, int pageIndex, int pageSize)
		{
			var planbooks = await _unitOfWork.PlanbookRepository.GetAsync(
							filter: r => r.CollectionId == collectionId && r.IsDeleted == false,
							pageIndex: pageIndex,
							pageSize: pageSize
							);
			var mappers = _mapper.Map<Pagination<ViewListPlanbookDTO>>(planbooks);
			foreach (var item in mappers.Items)
			{
				var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(item.LessonId);
				if (lesson != null)
				{
					item.LessonName = lesson.Name;
				}
			}

			return new SuccessResponseModel<object>
			{
				Success = true,
				Message = "Planbooks retrieved successfully.",
				Data = mappers
			};
		}
		#endregion

		#region Get Planbook By Lesson Id
		public async Task<ResponseModel> GetPlanbookByLessonIdAsync(string lessonId, int pageIndex, int pageSize)
		{
			var planbooks = await _unitOfWork.PlanbookRepository.GetAsync(
							filter: r => r.LessonId == lessonId && r.IsDeleted == false,
							pageIndex: pageIndex,
							pageSize: pageSize
							);
			var mappers = _mapper.Map<Pagination<ViewListPlanbookDTO>>(planbooks);
			foreach (var item in mappers.Items)
			{
				var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(item.LessonId);
				if (lesson != null)
				{
					item.LessonName = lesson.Name;
				}
			}

			return new SuccessResponseModel<object>
			{
				Success = true,
				Message = "Planbooks retrieved successfully.",
				Data = mappers
			};
		}
		#endregion

		// ham nay chua lam xong
		#region Get Planbook By User Id 
		public async Task<ResponseModel> GetPlanbookByUserIdAsync(string userId, int pageIndex, int pageSize)
		{
			var planbooks = await _unitOfWork.PlanbookRepository.GetAsync(
							//filter: r => r.UserId == userId && r.IsDeleted == false, 
							pageIndex: pageIndex,
							pageSize: pageSize
							);
			var mappers = _mapper.Map<Pagination<ViewListPlanbookDTO>>(planbooks);
			foreach (var item in mappers.Items)
			{
				var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(item.LessonId);
				if (lesson != null)
				{
					item.LessonName = lesson.Name;
				}
			}

			return new SuccessResponseModel<object>
			{
				Success = true,
				Message = "Planbooks retrieved successfully.",
				Data = mappers
			};
		}
		#endregion

		#region Create Planbook
		public async Task<ResponseModel> CreatePlanbookAsync(CreatePlanbookDTO model)
		{
			try
			{
				var planbook = _mapper.Map<Planbook>(model);
				await _unitOfWork.PlanbookRepository.AddAsync(planbook);
				await _unitOfWork.SaveChangeAsync();

				if (model.Activities != null && model.Activities.Any())
				{
					var activities = _mapper.Map<List<Activity>>(model.Activities);
					foreach (var activity in activities)
					{
						activity.PlanbookId = planbook.PlanbookId;
						await _unitOfWork.ActivityRepository.CreateActivityAsync(activity);
					}
					await _unitOfWork.SaveChangeAsync();
				}

				return new ResponseModel
				{
					Success = true,
					Message = "Planbook created successfully."
				};
			}
			catch (Exception ex)
			{
				return new ErrorResponseModel<string>
				{
					Success = false,
					Message = "An error occurred while creating the planbook.",
					Errors = new List<string> { ex.Message }
				};
			}
		}
		#endregion

		#region Update Planbook
		public async Task<ResponseModel> UpdatePlanbookAsync(UpdatePlanbookDTO model)
		{
			try
			{
				var planbook = await _unitOfWork.PlanbookRepository.GetByIdAsync(model.PlanbookId);
				if (planbook == null)
				{
					return new ResponseModel
					{
						Success = false,
						Message = "Planbook not found."
					};
				}

				var mapper = _mapper.Map(model, planbook);
				_unitOfWork.PlanbookRepository.Update(planbook);
				await _unitOfWork.SaveChangeAsync();

				// chua co activities

				return new ResponseModel
				{
					Success = true,
					Message = "Planbook updated successfully."
				};
			}
			catch (Exception ex)
			{
				return new ErrorResponseModel<string>
				{
					Success = false,
					Message = "An error occurred while updating the planbook.",
					Errors = new List<string> { ex.Message }
				};
			}
		}
		#endregion

		#region Soft Remove Planbook
		public async Task<ResponseModel> SoftRemovePlanbookAsync(string planbookId)
		{
			try
			{
				var planbook = await _unitOfWork.PlanbookRepository.GetByIdAsync(planbookId);
				if (planbook == null)
				{
					return new ResponseModel
					{
						Success = false,
						Message = "Planbook not found."
					};
				}

				_unitOfWork.PlanbookRepository.SoftRemove(planbook);
				await _unitOfWork.SaveChangeAsync();

				return new ResponseModel
				{
					Success = true,
					Message = "Planbook deleted successfully."
				};
			}
			catch (Exception ex)
			{
				return new ErrorResponseModel<string>
				{
					Success = false,
					Message = "An error occurred while deleting the planbook.",
					Errors = new List<string> { ex.Message }
				};
			}
		}
		#endregion
	}
}
