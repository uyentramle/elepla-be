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
							filter: r => !r.IsDeleted && r.LessonId == lessonId,
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
        //public async Task<ResponseModel> CreatePlanbookAsync(CreatePlanbookDTO model)
        //{
        //	try
        //	{
        //		var planbook = _mapper.Map<Planbook>(model);
        //		await _unitOfWork.PlanbookRepository.AddAsync(planbook);
        //		await _unitOfWork.SaveChangeAsync();

        //		if (model.Activities != null && model.Activities.Any())
        //		{
        //			var activities = _mapper.Map<List<Activity>>(model.Activities);
        //			foreach (var activity in activities)
        //			{
        //				activity.PlanbookId = planbook.PlanbookId;
        //				await _unitOfWork.ActivityRepository.CreateActivityAsync(activity);
        //			}
        //			await _unitOfWork.SaveChangeAsync();
        //		}

        //		return new ResponseModel
        //		{
        //			Success = true,
        //			Message = "Planbook created successfully."
        //		};
        //	}
        //	catch (Exception ex)
        //	{
        //		return new ErrorResponseModel<string>
        //		{
        //			Success = false,
        //			Message = "An error occurred while creating the planbook.",
        //			Errors = new List<string> { ex.Message }
        //		};
        //	}
        //}

        public async Task<ResponseModel> CreatePlanbookAsync(CreatePlanbookDTO model)
        {
            try
            {
                // Kiểm tra lesson có tồn tại không
                var existingLesson = await _unitOfWork.LessonRepository.GetByIdAsync(model.LessonId);

                if (existingLesson is null)
				{
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Lesson not found."
                    };
                }

                // Kiểm tra collection có tồn tại không
                if (!string.IsNullOrEmpty(model.CollectionId))
				{
                    var existingCollection = await _unitOfWork.PlanbookCollectionRepository.GetByIdAsync(model.CollectionId);

                    if (existingCollection is null)
                    {
                        return new ResponseModel
                        {
                            Success = false,
                            Message = "Collection not found."
                        };
                    }
                }

                // Tạo planbook
                var planbook = _mapper.Map<Planbook>(model);

                await _unitOfWork.PlanbookRepository.AddAsync(planbook);
                //await _unitOfWork.SaveChangeAsync();

                if (model.Activities != null && model.Activities.Any())
                {
                    var activities = _mapper.Map<List<Activity>>(model.Activities);
					int index = 1; // tự động tạo index cho activity, không cần truyền index trong DTO
                    
					foreach (var activity in activities)
                    {
                        activity.PlanbookId = planbook.PlanbookId;
                        activity.Index = index++;
                    }

                    await _unitOfWork.ActivityRepository.CreateRangeActivityAsync(activities);
                    //await _unitOfWork.SaveChangeAsync();
                }

                await _unitOfWork.SaveChangeAsync();

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
                // Kiểm tra planbook có tồn tại không
                var planbook = await _unitOfWork.PlanbookRepository.GetByIdAsync(model.PlanbookId);
				if (planbook is null)
				{
					return new ResponseModel
					{
						Success = false,
						Message = "Planbook not found."
					};
				}

                // Cập nhật Planbook
                _mapper.Map(model, planbook);
                _unitOfWork.PlanbookRepository.Update(planbook);

                if (model.Activities != null && model.Activities.Any())
                {
                    var existingActivities = await _unitOfWork.ActivityRepository.GetByPlanbookIdAsync(planbook.PlanbookId);

                    // Cập nhật activity trong planbook sẽ có 3 trường hợp:
                    // 1. Cập nhật thông tin cho các activity đã tồn tại
                    // 2. Thêm mới các activity
                    // 3. Có thể xóa activity đã tồn tại

                    // Tạo 1 list chứa các hoạt động cần cập nhật và một list chứa các hoạt động mới cần thêm
                    var activitiesToUpdate = new List<Activity>();
                    var newActivities = new List<Activity>();

                    // Tạo một HashSet chứa các Id của các hoạt động cần giữ lại
                    var activityIdsToKeep = model.Activities.Select(a => a.ActivityId).ToHashSet();

                    // Lọc ra các hoạt động không còn trong danh sách cần giữ
                    var activitiesToDelete = existingActivities
                        .Where(a => !activityIdsToKeep.Contains(a.ActivityId))
                        .ToList();

                    // Xác định index tiếp theo dựa trên hoạt động có Index cao nhất
                    int nextIndex = existingActivities.Any()
                        ? existingActivities.Max(a => a.Index) + 1
                        : 0;

                    foreach (var activityDto in model.Activities)
                    {
                        // Kiểm tra nếu hoạt động đã tồn tại
                        var existingActivity = existingActivities.FirstOrDefault(a => a.ActivityId == activityDto.ActivityId);
                        if (existingActivity != null)
                        {
                            // Map thông tin từ DTO vào hoạt động đã tồn tại và thêm vào list cần cập nhật
                            _mapper.Map(activityDto, existingActivity);
                            activitiesToUpdate.Add(existingActivity);
                        }
                        else
                        {
                            // Tạo hoạt động mới và gán Index tiếp theo
                            var newActivity = _mapper.Map<Activity>(activityDto);
                            newActivity.PlanbookId = planbook.PlanbookId;
                            newActivity.Index = nextIndex++;
                            newActivities.Add(newActivity);
                        }
                    }

                    // Xóa các hoạt động không còn trong danh sách
                    if (activitiesToDelete.Any())
                    {
                        _unitOfWork.ActivityRepository.DeleteRangeActivityAsync(activitiesToDelete);
                    }

                    // Cập nhật và thêm mới các hoạt động
                    if (activitiesToUpdate.Any())
                    {
                        _unitOfWork.ActivityRepository.UpdateRangeActivityAsync(activitiesToUpdate);
                    }
                    if (newActivities.Any())
                    {
                        await _unitOfWork.ActivityRepository.CreateRangeActivityAsync(newActivities);
                    }
                }

                // Lưu tất cả các thay đổi
                await _unitOfWork.SaveChangeAsync();

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

        #region Delete Planbook
        public async Task<ResponseModel> DeletePlanbookAsync(string planbookId)
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

                _unitOfWork.PlanbookRepository.Delete(planbook);
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

        #region Create Planbook From Template
        public async Task<ResponseModel> GetPlanbookFromTemplateAsync(string lessonId)
        {
            // Lấy danh sách các planbook mặc định cho bài học
            var defaultPlanbooks = await _unitOfWork.PlanbookRepository.GetAllAsync(
                                                filter: p => p.IsDefault && p.LessonId.Equals(lessonId),
                                                includeProperties: "PlanbookCollection,Lesson,Activities");

            if (defaultPlanbooks is null || !defaultPlanbooks.Any())
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "No default planbooks found for the specified lesson."
                };
            }

            // Chọn một planbook ngẫu nhiên từ danh sách
            var random = new Random();
            var templatePlanbook = defaultPlanbooks.ElementAt(random.Next(defaultPlanbooks.Count()));

            // Map thông tin planbook và các hoạt động từ planbook mẫu
            var planbookDto = _mapper.Map<ViewDetailsPlanbookDTO>(templatePlanbook);
            var activities = templatePlanbook.Activities.OrderBy(a => a.Index).ToList(); // Sắp xếp theo Index
            planbookDto.Activities = _mapper.Map<List<ViewListActivityDTO>>(activities);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Planbook template retrieved successfully.",
                Data = planbookDto
            };
        }
        #endregion
    }
}
