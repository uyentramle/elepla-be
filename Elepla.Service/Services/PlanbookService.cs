using AutoMapper;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using Elepla.Domain.Entities;
using Elepla.Repository.Common;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.ActivityViewModels;
using Elepla.Service.Models.ViewModels.PlanbookViewModels;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using Word = DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Wordprocessing;
using Elepla.Service.Models.ViewModels.PlanbookCollectionViewModels;
using Elepla.Service.Models.ViewModels.AccountViewModels;

namespace Elepla.Service.Services
{
    public class PlanbookService : IPlanbookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IOpenAIService _openAIService;

        public PlanbookService(IUnitOfWork unitOfWork, IMapper mapper, IOpenAIService openAIService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _openAIService = openAIService;
        }

        #region Get All Planbooks
        public async Task<ResponseModel> GetAllPlanbooksAsync(int pageIndex, int pageSize)
        {
            var planbooks = await _unitOfWork.PlanbookRepository.GetAsync(
                            filter: r => r.IsDeleted == false && r.IsPublic && !r.IsDefault,
                            includeProperties: "Lesson.Chapter.SubjectInCurriculum.Subject,Lesson.Chapter.SubjectInCurriculum.Curriculum,Lesson.Chapter.SubjectInCurriculum.Grade,Feedbacks",
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

                // Tính số lượng comment và đánh giá trung bình
                var feedbacks = planbooks.Items
                    .FirstOrDefault(p => p.PlanbookId == item.PlanbookId)?
                    .Feedbacks
                    .Where(f => !f.IsDeleted) // Lọc những feedback hợp lệ
                    .ToList();

                if (feedbacks != null && feedbacks.Any())
                {
                    item.CommentCount = feedbacks.Count;
                    var totalRate = feedbacks.Where(f => f.Rate.HasValue).Sum(f => f.Rate.Value);
                    item.AverageRate = (float)Math.Round(totalRate / (double)item.CommentCount, 1);
                }
                else
                {
                    item.CommentCount = 0;
                    item.AverageRate = 0;
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
            var planbook = await _unitOfWork.PlanbookRepository.GetByIdAsync(id: planbookId, includeProperties: "Feedbacks");
            if (planbook == null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "Planbook not found."
                };
            }

            var mapper = _mapper.Map<ViewDetailsPlanbookDTO>(planbook);

            //var collection = await _unitOfWork.PlanbookCollectionRepository.GetByIdAsync(planbook.CollectionId);
            //if (collection != null)
            //{
            //    mapper.CollectionName = collection.CollectionName;
            //}

            var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(planbook.LessonId);
            if (lesson != null)
            {
                mapper.LessonName = lesson.Name;
            }

            // Tính toán CommentCount và AverageRate
            var feedbacks = planbook.Feedbacks.Where(f => !f.IsDeleted).ToList();
            if (feedbacks.Any())
            {
                mapper.CommentCount = feedbacks.Count;
                var totalRate = feedbacks.Where(f => f.Rate.HasValue).Sum(f => f.Rate.Value);
                mapper.AverageRate = (float)Math.Round(totalRate / (double)mapper.CommentCount, 1);
            }
            else
            {
                mapper.CommentCount = 0;
                mapper.AverageRate = 0;
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
            //var planbooks = await _unitOfWork.PlanbookRepository.GetAsync(
            //                filter: r => r.CollectionId == collectionId && r.IsDeleted == false,
            //                pageIndex: pageIndex,
            //                pageSize: pageSize
            //                );
            //var mappers = _mapper.Map<Pagination<ViewListPlanbookDTO>>(planbooks);
            //foreach (var item in mappers.Items)
            //{
            //    var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(item.LessonId);
            //    if (lesson != null)
            //    {
            //        item.LessonName = lesson.Name;
            //    }
            //}
            var planbooks = await _unitOfWork.PlanbookRepository.GetAsync(
                    filter: r => r.PlanbookInCollections.Any(pc => pc.CollectionId == collectionId) && !r.IsDeleted,
                    includeProperties: "Lesson,PlanbookInCollections.PlanbookCollection",
                    pageIndex: pageIndex,
                    pageSize: pageSize);

            var mappers = _mapper.Map<Pagination<ViewListPlanbookDTO>>(planbooks);

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
        //		var planbookDto = _mapper.Map<Planbook>(model);
        //		await _unitOfWork.PlanbookRepository.AddAsync(planbookDto);
        //		await _unitOfWork.SaveChangeAsync();

        //		if (model.Activities != null && model.Activities.Any())
        //		{
        //			var activities = _mapper.Map<List<Activity>>(model.Activities);
        //			foreach (var activity in activities)
        //			{
        //				activity.PlanbookId = planbookDto.PlanbookId;
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
        //			Message = "An error occurred while creating the planbookDto.",
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
                
                if (model.CollectionId is not null)
                {
                    // Kiểm tra collection có tồn tại không
                    var existingCollection = await _unitOfWork.PlanbookCollectionRepository.GetByIdAsync(model.CollectionId);

                    if (existingCollection is null)
                    {
                        return new ResponseModel
                        {
                            Success = false,
                            Message = "Collection not found."
                        };
                    }

                    // Lấy gói dịch vụ của người dùng đang sử dụng
                    var userPackage = await _unitOfWork.UserPackageRepository.GetActiveUserPackageAsync(existingCollection.TeacherId);

                    if (userPackage is null)
                    {
                        return new ResponseModel
                        {
                            Success = false,
                            Message = "User package not found."
                        };
                    }

                    var createdPlanbookCount = await _unitOfWork.PlanbookRepository.CountPlanbookByUserId(existingCollection.TeacherId);

                    if (createdPlanbookCount >= userPackage.MaxPlanbooks)
                    {
                        return new ResponseModel
                        {
                            Success = false,
                            Message = "You have reached the maximum number of planbooks allowed by your package."
                        };
                    }
                }

                // Tạo planbookDto
                var planbook = _mapper.Map<Planbook>(model);

                await _unitOfWork.PlanbookRepository.AddAsync(planbook);
                //await _unitOfWork.SaveChangeAsync();

                // Nếu có CollectionId thì tạo bản ghi trong PlanbookInCollection
                if (model.CollectionId is not null)
                {
                    var planbookInCollection = new PlanbookInCollection
                    {
                        PlanbookInCollectionId = Guid.NewGuid().ToString(),
                        PlanbookId = planbook.PlanbookId,
                        CollectionId = model.CollectionId
                    };

                    await _unitOfWork.PlanbookInCollectionRepository.AddAsync(planbookInCollection);
                }

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
                // Kiểm tra planbookDto có tồn tại không
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

                    // Cập nhật activity trong planbookDto sẽ có 3 trường hợp:
                    // 1. Cập nhật thông tin cho các activity đã tồn tại
                    // 2. Thêm mới các activity
                    // 3. Có thể xóa activity đã tồn tại

                    // Tạo 1 list chứa các hoạt động cần cập nhật và một list chứa các hoạt động mới cần thêm
                    var activitiesToUpdate = new List<Activity>();
                    var newActivities = new List<Activity>();

                    // Tạo một HashSet chứa các UserPackageId của các hoạt động cần giữ lại
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
                        _unitOfWork.ActivityRepository.DeleteRangeActivity(activitiesToDelete);
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
                if (planbook is null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Planbook not found."
                    };
                }

                // Kiểm tra nếu có liên kết trong bảng PlanbookInCollection, xóa các bản ghi liên quan
                var planbookInCollections = await _unitOfWork.PlanbookInCollectionRepository.GetAllByPlanbookId(planbookId);

                if (planbookInCollections.Any())
                {
                    // Xóa các liên kết trong bảng PlanbookInCollection
                    _unitOfWork.PlanbookInCollectionRepository.DeleteRange(planbookInCollections);
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
            // Lấy danh sách các planbookDto mặc định cho bài học
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

            // Chọn một planbookDto ngẫu nhiên từ danh sách
            var random = new Random();
            var templatePlanbook = defaultPlanbooks.ElementAt(random.Next(defaultPlanbooks.Count()));

            // Map thông tin planbookDto và các hoạt động từ planbookDto mẫu
            var planbookDto = _mapper.Map<CreatePlanbookDTO>(templatePlanbook);
            planbookDto.IsDefault = false;
            planbookDto.IsPublic = false;
            var activities = templatePlanbook.Activities.OrderBy(a => a.Index).ToList(); // Sắp xếp theo Index
            planbookDto.Activities = _mapper.Map<List<CreateActivityForPlanbookDTO>>(activities);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Planbook template retrieved successfully.",
                Data = planbookDto
            };
        }
        #endregion

        #region Create Planbook Using AI
        //public async Task<ResponseModel> GetPlanbookUsingAIAsync(string lessonId)
        //{
        //    try
        //    {
        //        var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(
        //                                    id: lessonId,
        //                                    filter: l => !l.IsDeleted,
        //                                    includeProperties: "Chapter,Chapter.SubjectInCurriculum,Chapter.SubjectInCurriculum.Subject,Chapter.SubjectInCurriculum.Curriculum,Chapter.SubjectInCurriculum.Grade");

        //        if (lesson is null)
        //        {
        //            return new ResponseModel
        //            {
        //                Success = false,
        //                Message = "Lesson not found."
        //            };
        //        }

        //        var chapter = lesson.Chapter;
        //        var subject = chapter.SubjectInCurriculum.Subject.Name + " - " + chapter.SubjectInCurriculum.Grade.Name + " - " + chapter.SubjectInCurriculum.Curriculum.Name;

        //        // Tạo các prompt cho AI để lấy các thông tin cho kế hoạch giảng dạy
        //        var titlePrompt = $"Hãy tạo tiêu đề thích hợp cho kế hoạch giảng dạy của bài học {lesson.Name}, chỉ cần một tiêu đề";
        //        var knowledgeObjectivePrompt = $"Hãy tạo mục tiêu kiến thức cho bài học {lesson.Name} thuộc chương {chapter.Name} của môn học {subject}";
        //        var skillsObjectivePrompt = $"Hãy tạo mục tiêu kỹ năng cho bài học {lesson.Name} thuộc chương {chapter.Name} của môn học {subject}";
        //        var qualitiesObjectivePrompt = $"Hãy tạo mục tiêu phẩm chất cho bài học {lesson.Name} thuộc chương {chapter.Name} của môn học {subject}";
        //        var teachingToolsPrompt = $"Hãy tạo danh sách thiết bị dạy học cho bài học {lesson.Name} thuộc chương {chapter.Name} của môn học {subject}";
        //        var notesPrompt = $"Hãy tạo ghi chú cho bài học {lesson.Name} thuộc chương {chapter.Name} của môn học {subject}";

        //        // Gọi OpenAI Service để lấy các thông tin từ các prompt
        //        var title = await _openAIService.GeneratePlanbookFieldAsync(titlePrompt);
        //        var knowledgeObjective = await _openAIService.GeneratePlanbookFieldAsync(knowledgeObjectivePrompt);
        //        var skillsObjective = await _openAIService.GeneratePlanbookFieldAsync(skillsObjectivePrompt);
        //        var qualitiesObjective = await _openAIService.GeneratePlanbookFieldAsync(qualitiesObjectivePrompt);
        //        var teachingTools = await _openAIService.GeneratePlanbookFieldAsync(teachingToolsPrompt);
        //        var notes = await _openAIService.GeneratePlanbookFieldAsync(notesPrompt);

        //        // Tạo đối tượng CreatePlanbookDTO và điền thông tin vào
        //        var planbookDto = new CreatePlanbookDTO
        //        {
        //            Title = title,
        //            SchoolName = "",  // Giáo viên có thể tự điền
        //            TeacherName = "",  // Giáo viên có thể tự điền
        //            Subject = subject,
        //            ClassName = "",  // Giáo viên có thể tự điền
        //            DurationInPeriods = 1,  // Tiết học, mặc định là 1
        //            KnowledgeObjective = knowledgeObjective,
        //            SkillsObjective = skillsObjective,
        //            QualitiesObjective = qualitiesObjective,
        //            TeachingTools = teachingTools,
        //            Notes = notes,
        //            IsDefault = false,
        //            CollectionId = null,  // Khi call API này, FE sẽ tự truyền CollectionId
        //            LessonId = lessonId,
        //            Activities = new List<CreateActivityForPlanbookDTO>(), // Các hoạt động sẽ được tạo sau
        //        };

        //        // Tiến hành tạo các hoạt động cho bài học
        //        var activityPrompts = new List<string>
        //        {
        //            "Hoạt động 1: Xác định vấn đề",
        //            "Hoạt động 2: Hình thành kiến thức mới",
        //            "Hoạt động 3: Luyện tập",
        //            "Hoạt động 4: Vận dụng"
        //        };

        //        foreach (var activityPrompt in activityPrompts)
        //        {
        //            //var activityTitle = await _openAIService.GeneratePlanbookFieldAsync($"Hãy tạo nội dung cho {activityPrompt} trong bài học {lesson.Name}");
        //            var activityObjective = await _openAIService.GeneratePlanbookFieldAsync($"Hãy tạo mục tiêu cho {activityPrompt} trong bài học {lesson.Name}");
        //            var activityContent = await _openAIService.GeneratePlanbookFieldAsync($"Hãy tạo nội dung chi tiết cho {activityPrompt} trong bài học {lesson.Name}");
        //            var activityProduct = await _openAIService.GeneratePlanbookFieldAsync($"Hãy tạo sản phẩm cho {activityPrompt} trong bài học {lesson.Name}");
        //            var activityImplementation = await _openAIService.GeneratePlanbookFieldAsync($"Hãy tạo hướng dẫn tổ chức thực hiện cho {activityPrompt} trong bài học {lesson.Name}");

        //            planbookDto.Activities.Add(new CreateActivityForPlanbookDTO
        //            {
        //                Title = activityPrompt,
        //                Objective = activityObjective,
        //                Content = activityContent,
        //                Product = activityProduct,
        //                Implementation = activityImplementation
        //            });
        //        }

        //        return new SuccessResponseModel<object>
        //        {
        //            Success = true,
        //            Message = "Planbook created successfully using AI.",
        //            Data = planbookDto
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ErrorResponseModel<object>
        //        {
        //            Success = false,
        //            Message = "An error occurred while creating the planbookDto using AI.",
        //            Errors = new List<string> { ex.Message }
        //        };
        //    }
        //}

        // Tối ưu tôc độ trả về của OpenAI Service bằng cách gửi nhiều prompt cùng lúc
        public async Task<ResponseModel> GetPlanbookUsingAIAsync(string lessonId)
        {
            try
            {
                var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(
                                id: lessonId,
                                filter: l => !l.IsDeleted,
                                includeProperties: "Chapter,Chapter.SubjectInCurriculum,Chapter.SubjectInCurriculum.Subject,Chapter.SubjectInCurriculum.Curriculum,Chapter.SubjectInCurriculum.Grade");

                if (lesson is null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Lesson not found."
                    };
                }

                var chapter = lesson.Chapter;
                var subject = chapter.SubjectInCurriculum.Subject.Name + " - " + chapter.SubjectInCurriculum.Grade.Name + " - " + chapter.SubjectInCurriculum.Curriculum.Name;

                var rule = "Bỏ chữ in đậm, in nghiêng, chỉ trả về chữ văn bản bình thường, bỏ khoảng trống giữa các dòng. Chỉ cần trả về kết quả không cần lặp lại yêu cầu.";

                // Tạo các prompt cho AI để lấy các thông tin cho kế hoạch giảng dạy
                var prompts = new List<string>
                {
                    $"Hãy tạo tiêu đề thích hợp cho kế hoạch giảng dạy của bài học {lesson.Name}, chỉ cần một tiêu đề",
                    $"Hãy tạo mục tiêu kiến thức cho bài học {lesson.Name} thuộc chương {chapter.Name} của môn học {subject}",
                    $"Hãy tạo mục tiêu kỹ năng cho bài học {lesson.Name} thuộc chương {chapter.Name} của môn học {subject}",
                    $"Hãy tạo mục tiêu phẩm chất cho bài học {lesson.Name} thuộc chương {chapter.Name} của môn học {subject}",
                    $"Hãy tạo danh sách thiết bị dạy học cho bài học {lesson.Name} thuộc chương {chapter.Name} của môn học {subject}",
                    $"Hãy tạo ghi chú cho bài học {lesson.Name} thuộc chương {chapter.Name} của môn học {subject}"
                };

                // Gửi các prompt cho OpenAI Service để lấy các thông tin cho kế hoạch giảng dạy
                var aiResponses = await Task.WhenAll(prompts.Select(prompt => _openAIService.GeneratePlanbookFieldAsync(rule + prompt)));

                // Map AI response cho các trường thông tin của planbookDto
                var planbookDto = new CreatePlanbookDTO
                {
                    Title = aiResponses[0],
                    SchoolName = "",  // Giáo viên có thể tự điền
                    TeacherName = "", // Giáo viên có thể tự điền
                    Subject = subject,
                    ClassName = "",   // Giáo viên có thể tự điền
                    DurationInPeriods = 1, // Tiết học, mặc định là 1
                    KnowledgeObjective = aiResponses[1],
                    SkillsObjective = aiResponses[2],
                    QualitiesObjective = aiResponses[3],
                    TeachingTools = aiResponses[4],
                    Notes = aiResponses[5],
                    IsDefault = false,
                    IsPublic = false,
                    CollectionId = null,  // Khi call API này, FE sẽ tự truyền CollectionId
                    LessonId = lessonId,
                    Activities = new List<CreateActivityForPlanbookDTO>() // Các hoạt động sẽ được tạo sau
                };

                // Tiến hành tạo các hoạt động cho bài học
                var activityPrompts = new List<string>
                {
                    "Xác định vấn đề",
                    "Hình thành kiến thức mới",
                    "Luyện tập",
                    "Vận dụng"
                };

                // Tạo một list các task để gửi các prompt cho AI và lấy kết quả cho các hoạt động
                var activityTasks = activityPrompts.Select(async activityPrompt =>
                {
                    var objective = await _openAIService.GeneratePlanbookFieldAsync(rule + $"Hãy tạo mục tiêu cho hoạt động {activityPrompt} trong bài học {lesson.Name}");
                    var content = await _openAIService.GeneratePlanbookFieldAsync(rule + $"Hãy tạo nội dung chi tiết cho hoạt động {activityPrompt} trong bài học {lesson.Name}");
                    var product = await _openAIService.GeneratePlanbookFieldAsync(rule + $"Hãy tạo sản phẩm cho hoạt động {activityPrompt} trong bài học {lesson.Name}");
                    var implementation = await _openAIService.GeneratePlanbookFieldAsync(rule + $"Hãy tạo hướng dẫn tổ chức thực hiện cho hoạt động {activityPrompt} trong bài học {lesson.Name}");

                    return new CreateActivityForPlanbookDTO
                    {
                        Title = activityPrompt,
                        Objective = objective,
                        Content = content,
                        Product = product,
                        Implementation = implementation
                    };
                }).ToList();

                // Chờ tất cả các task hoàn thành và lấy kết quả
                var activities = await Task.WhenAll(activityTasks);

                // Thêm các hoạt động vào planbookDto
                planbookDto.Activities.AddRange(activities);

                return new SuccessResponseModel<object>
                {
                    Success = true,
                    Message = "Planbook created successfully using AI.",
                    Data = planbookDto
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while creating the planbook using AI.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        #endregion

        #region Clone Planbook
        public async Task<ResponseModel> ClonePlanbookAsync(ClonePlanbookDTO model)
        {
            try
            {
                // Kiểm tra planbookDto có tồn tại không
                var planbook = await _unitOfWork.PlanbookRepository.GetByIdAsync(model.PlanbookId);
                if (planbook is null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Planbook not found."
                    };
                }

                // Kiểm tra collection có tồn tại không
                var existingCollection = await _unitOfWork.PlanbookCollectionRepository.GetByIdAsync(model.CollectionId);
                if (existingCollection is null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Collection not found."
                    };
                }

                // Lấy gói dịch vụ của người dùng đang sử dụng
                var userPackage = await _unitOfWork.UserPackageRepository.GetActiveUserPackageAsync(existingCollection.TeacherId);
                if (userPackage is null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "User package not found."
                    };
                }

                // Kiểm tra số lượng planbook đã tạo của người dùng
                var createdPlanbookCount = await _unitOfWork.PlanbookRepository.CountPlanbookByUserId(existingCollection.TeacherId);
                if (createdPlanbookCount >= userPackage.MaxPlanbooks)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "You have reached the maximum number of planbooks allowed by your package."
                    };
                }

                // Clone Planbook
                var clonePlanbook = new Planbook
                {
                    PlanbookId = Guid.NewGuid().ToString(),
                    Title = planbook.Title,
                    SchoolName = planbook.SchoolName,
                    TeacherName = planbook.TeacherName,
                    Subject = planbook.Subject,
                    ClassName = planbook.ClassName,
                    DurationInPeriods = planbook.DurationInPeriods,
                    KnowledgeObjective = planbook.KnowledgeObjective,
                    SkillsObjective = planbook.SkillsObjective,
                    QualitiesObjective = planbook.QualitiesObjective,
                    TeachingTools = planbook.TeachingTools,
                    Notes = planbook.Notes,
                    IsDefault = false,
                    IsPublic = false,
                    LessonId = planbook.LessonId,
                    Activities = new List<Activity>()
                };

                // Clone các hoạt động
                var activities = await _unitOfWork.ActivityRepository.GetByPlanbookIdAsync(model.PlanbookId);
                var cloneActivities = activities.Select(a => new Activity
                {
                    ActivityId = Guid.NewGuid().ToString(),
                    PlanbookId = clonePlanbook.PlanbookId,
                    Title = a.Title,
                    Objective = a.Objective,
                    Content = a.Content,
                    Product = a.Product,
                    Implementation = a.Implementation,
                    Index = a.Index
                }).ToList();

                // Tạo liên kết trong bảng PlanbookInCollection
                var planbookInCollection = new PlanbookInCollection
                {
                    PlanbookInCollectionId = Guid.NewGuid().ToString(),
                    PlanbookId = clonePlanbook.PlanbookId,
                    CollectionId = model.CollectionId
                };

                await _unitOfWork.PlanbookRepository.AddAsync(clonePlanbook);
                await _unitOfWork.ActivityRepository.CreateRangeActivityAsync(cloneActivities);
                await _unitOfWork.PlanbookInCollectionRepository.AddAsync(planbookInCollection);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Planbook cloned successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<string>
                {
                    Success = false,
                    Message = "An error occurred while cloning the planbook.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        #endregion

        #region Save Planbook
        public async Task<ResponseModel> SavePlanbookAsync(SavePlanbookDTO model)
        {
            try
            {
                var collection = await _unitOfWork.PlanbookCollectionRepository.GetByIdAsync(model.CollectionId);
                if (collection is null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Collection not found."
                    };
                }

                var planbook = await _unitOfWork.PlanbookRepository.GetByIdAsync(model.PlanbookId);
                if (planbook is null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Planbook not found."
                    };
                }

                // Kiểm tra xem planbook đã được lưu trong collection chưa
                var existingPlanbookInCollection = await _unitOfWork.PlanbookInCollectionRepository.GetByCollectionIdAndPlanbookId(model.CollectionId, model.PlanbookId);
                if (existingPlanbookInCollection != null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Planbook already saved in the collection."
                    };
                }

                // Tạo liên kết trong bảng PlanbookInCollection
                var planbookInCollection = new PlanbookInCollection
                {
                    PlanbookInCollectionId = Guid.NewGuid().ToString(),
                    PlanbookId = model.PlanbookId,
                    CollectionId = model.CollectionId
                };

                await _unitOfWork.PlanbookInCollectionRepository.AddAsync(planbookInCollection);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Planbook saved successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<string>
                {
                    Success = false,
                    Message = "An error occurred while saving the planbook.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseModel> UnsavePlanbookAsync(SavePlanbookDTO model)
        {
            try
            {
                var planbookInCollection = await _unitOfWork.PlanbookInCollectionRepository.GetByCollectionIdAndPlanbookId(model.CollectionId, model.PlanbookId);
                if (planbookInCollection is null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Planbook not found in the collection."
                    };
                }

                _unitOfWork.PlanbookInCollectionRepository.Delete(planbookInCollection);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Planbook unsaved successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<string>
                {
                    Success = false,
                    Message = "An error occurred while unsaving the planbook.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        #endregion

        #region Share Planbook
        public async Task<ResponseModel> SharePlanbookAsync(SharePlanbookDTO model)
        {
            try
            {
                var planbook = await _unitOfWork.PlanbookRepository.GetByIdAsync(
                                            id: model.PlanbookId,
                                            includeProperties: "PlanbookInCollections.PlanbookCollection");

                if (planbook is null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Planbook not found."
                    };
                }

                // Kiểm tra nếu SharedTo rỗng, xóa tất cả chia sẻ
                if (model.SharedTo is null || !model.SharedTo.Any())
                {
                    var sharesToDelete = await _unitOfWork.PlanbookShareRepository
                        .GetAllAsync(ps => ps.PlanbookId == model.PlanbookId);

                    // Xóa tất cả chia sẻ
                    _unitOfWork.PlanbookShareRepository.DeleteRange(sharesToDelete);
                    await _unitOfWork.SaveChangeAsync();

                    return new ResponseModel
                    {
                        Success = true,
                        Message = "Planbook sharing updated successfully."
                    };
                }

                // Lấy danh sách tất cả các userId từ danh sách SharedTo
                var userIds = model.SharedTo.Select(x => x.UserId).ToList();

                // Kiểm tra các userId tồn tại
                var users = await _unitOfWork.AccountRepository.GetByIdsAsync(userIds);

                if (users.Count != userIds.Count)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Some users not found."
                    };
                }

                var existingShares = await _unitOfWork.PlanbookShareRepository
                    .GetAllAsync(ps => ps.PlanbookId == model.PlanbookId);

                // Sử dụng dictionary để tra cứu nhanh
                var existingSharesDict = existingShares.ToDictionary(x => x.SharedTo);

                // Danh sách chia sẻ cần xóa
                var sharesToRemove = existingShares.Where(ps => !userIds.Contains(ps.SharedTo)).ToList();

                // Xóa những chia sẻ không còn trong danh sách SharedTo
                if (sharesToRemove.Any())
                {
                    _unitOfWork.PlanbookShareRepository.DeleteRange(sharesToRemove);
                }

                // Xử lý chia sẻ mới hoặc cập nhật
                var newShares = new List<PlanbookShare>();

                foreach (var userShare in model.SharedTo)
                {
                    var shareExists = existingSharesDict.TryGetValue(userShare.UserId, out var existingShare);

                    if (shareExists)
                    {
                        // Nếu chia sẻ đã tồn tại, cập nhật IsEdited
                        existingShare.IsEdited = userShare.IsEdited;
                        _unitOfWork.PlanbookShareRepository.Update(existingShare);
                    }
                    else
                    {
                        // Nếu chưa có chia sẻ, tạo mới
                        var planbookShare = new PlanbookShare
                        {
                            ShareId = Guid.NewGuid().ToString(),
                            PlanbookId = model.PlanbookId,
                            SharedBy = planbook.PlanbookInCollections.FirstOrDefault()?.PlanbookCollection?.TeacherId,
                            SharedTo = userShare.UserId,
                            IsEdited = userShare.IsEdited
                        };

                        newShares.Add(planbookShare);
                    }
                }

                // Thêm chia sẻ mới
                if (newShares.Any())
                {
                    await _unitOfWork.PlanbookShareRepository.AddRangeAsync(newShares);
                }

                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Planbook sharing updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<string>
                {
                    Success = false,
                    Message = "An error occurred while sharing the planbook.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseModel> GetUserSharedByPlanbookAsync(string planbookId)
        {
            try
            {
                // Lấy thông tin về Planbook từ PlanbookInCollections hoặc cách khác để xác định chủ sở hữu
                var planbook = await _unitOfWork.PlanbookRepository.GetByIdAsync(planbookId, includeProperties: "PlanbookInCollections.PlanbookCollection");

                if (planbook is null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Planbook not found."
                    };
                }

                // Lấy thông tin chủ sở hữu từ PlanbookInCollections
                var ownerUserId = planbook.PlanbookInCollections?.FirstOrDefault()?.PlanbookCollection?.TeacherId;

                // Lấy tất cả các bản ghi chia sẻ từ PlanbookShare tương ứng với PlanbookId
                var shares = await _unitOfWork.PlanbookShareRepository.GetAllAsync(ps => ps.PlanbookId == planbookId);

                // Lấy danh sách UserIds từ các bản ghi chia sẻ
                var userIds = shares?.Select(x => x.SharedTo).ToList() ?? new List<string>();

                // Lấy thông tin người dùng từ UserIds
                var users = userIds.Any() ? await _unitOfWork.AccountRepository.GetByIdsAsync(userIds)
                                            : new List<User>();

                // Tạo danh sách để chứa thông tin người dùng bao gồm cả chủ sở hữu
                var sharedBy = new List<ViewListUserPlanbookShareDTO>();

                // Thêm chủ sở hữu vào danh sách
                var owner = await _unitOfWork.AccountRepository.GetByIdAsync(id: ownerUserId, includeProperties: "Avatar");
                if (owner is not null)
                {
                    var ownerDto = _mapper.Map<ViewListUserPlanbookShareDTO>(owner);
                    ownerDto.IsEdited = true; // Chủ sở hữu không có quyền chỉnh sửa từ chia sẻ
                    ownerDto.IsOwner = true;
                    sharedBy.Add(ownerDto);
                }

                // Thêm các người dùng được chia sẻ vào danh sách
                foreach (var user in users)
                {
                    var sharedUserDto = _mapper.Map<ViewListUserPlanbookShareDTO>(user);
                    var shareRecord = shares?.FirstOrDefault(x => x.SharedTo == user.UserId);
                    if (shareRecord is not null)
                    {
                        sharedUserDto.IsEdited = shareRecord.IsEdited;
                    }
                    sharedBy.Add(sharedUserDto);
                }

                // Trả về kết quả với dữ liệu đã chuyển đổi
                return new SuccessResponseModel<object>
                {
                    Success = true,
                    Message = "Planbook shared by retrieved successfully.",
                    Data = sharedBy
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<string>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the planbook shared by.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseModel> GetUserToSharedPlanbookAsync(string planbookId)
        {
            try
            {
                var planbook = await _unitOfWork.PlanbookRepository.GetByIdAsync(planbookId, includeProperties: "PlanbookInCollections.PlanbookCollection");

                if (planbook is null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Planbook not found."
                    };
                }

                // Lấy tất cả người dùng từ hệ thống
                var allUsers = await _unitOfWork.AccountRepository.GetAllAsync(includeProperties: "Avatar");

                // Lấy thông tin chủ sở hữu từ PlanbookInCollections
                var ownerUserId = planbook?.PlanbookInCollections?.FirstOrDefault()?.PlanbookCollection?.TeacherId;

                // Lấy danh sách chia sẻ từ PlanbookShare
                var sharedUsers = await _unitOfWork.PlanbookShareRepository.GetAllAsync(ps => ps.PlanbookId == planbookId);

                // Danh sách UserIds đã được chia sẻ
                var sharedUserIds = sharedUsers.Select(ps => ps.SharedTo).ToHashSet();

                // Lọc người dùng chưa được chia sẻ
                var notSharedUsers = allUsers.Where(user => !sharedUserIds.Contains(user.UserId) && user.UserId != ownerUserId).ToList();

                // Sử dụng AutoMapper để chuyển đổi sang DTO
                var notSharedUserDtos = _mapper.Map<List<ViewListUserToPlanbookShareDTO>>(notSharedUsers);

                // Trả về kết quả
                return new SuccessResponseModel<object>
                {
                    Success = true,
                    Message = "Users retrieved successfully.",
                    Data = notSharedUserDtos
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving users not shared with the planbook.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseModel> GetSharedPlanbookByUserIdAsync(string userId)
        {
            try
            {
                // Lấy danh sách các bản ghi chia sẻ từ PlanbookShare dựa vào userId
                var sharedPlanbooks = await _unitOfWork.PlanbookShareRepository.GetAllAsync(
                                                    filter: ps => ps.SharedTo.Equals(userId),
                                                    includeProperties: "Planbook.PlanbookInCollections.PlanbookCollection,Planbook.Lesson.Chapter.SubjectInCurriculum.Subject,Planbook.Lesson.Chapter.SubjectInCurriculum.Curriculum,Planbook.Lesson.Chapter.SubjectInCurriculum.Grade");

                // Lấy danh sách Planbooks từ bản ghi chia sẻ
                var planbooks = sharedPlanbooks.Select(ps => ps.Planbook).Distinct().ToList();

                // Sử dụng AutoMapper để chuyển đổi sang DTO
                var planbookDtos = _mapper.Map<List<ViewListPlanbookDTO>>(planbooks);

                // Trả về danh sách DTO
                return new SuccessResponseModel<object>
                {
                    Success = true,
                    Message = "Shared planbooks retrieved successfully.",
                    Data = planbookDtos
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the planbooks shared with user.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        #endregion

        #region Export Planbook to Word
        public async Task<ResponseModel> ExportPlanbookToWordAsync(string planbookId)
        {
            try
            {
                var planbook = await _unitOfWork.PlanbookRepository.GetByIdAsync(id: planbookId, includeProperties: "Activities");
                if (planbook is null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Planbook not found."
                    };
                }

                using (var memoryStream = new MemoryStream())
                {
                    using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document, true))
                    {
                        var mainPart = wordDocument.AddMainDocumentPart();
                        mainPart.Document = new Document();
                        var body = mainPart.Document.AppendChild(new Body());

                        // Add SchoolName and TeacherName in one row (Header)
                        body.AppendChild(CreateTopTable(
                            "TRƯỜNG " + planbook.SchoolName?.ToUpper() ?? string.Empty,
                            "TỔ ...",
                            "HỌ VÀ TÊN GIÁO VIÊN",
                            planbook.TeacherName?.ToUpper() ?? string.Empty,
                            14 // Font size
                        ));

                        // Add Title and Other Details
                        body.AppendChild(CreateParagraph($"", JustificationValues.Center, fontSize: 14));
                        body.AppendChild(CreateParagraph($"TÊN BÀI DẠY: {planbook.Title?.ToUpper()}", JustificationValues.Center, bold: true, fontSize: 14));
                        body.AppendChild(CreateParagraph($"Môn học: {planbook.Subject}; lớp: {planbook.ClassName}", JustificationValues.Center, fontSize: 14));
                        body.AppendChild(CreateParagraph($"Thời gian thực hiện: ({planbook.DurationInPeriods} tiết)", JustificationValues.Center, fontSize: 14));
                        body.AppendChild(CreateParagraph($"", JustificationValues.Center, fontSize: 14));

                        // Add Objectives Section
                        body.AppendChild(CreateParagraph("I. MỤC TIÊU", JustificationValues.Left, bold: true, fontSize: 14));
                        body.AppendChild(CreateParagraph($"1. Về kiến thức:", JustificationValues.Left, bold: true, fontSize: 14));
                        body.AppendChild(CreateParagraph(planbook.KnowledgeObjective ?? string.Empty, JustificationValues.Left, fontSize: 14));
                        body.AppendChild(CreateParagraph($"2. Về năng lực:", JustificationValues.Left, bold: true, fontSize: 14));
                        body.AppendChild(CreateParagraph(planbook.SkillsObjective ?? string.Empty, JustificationValues.Left, fontSize: 14));
                        body.AppendChild(CreateParagraph($"3. Về phẩm chất:", JustificationValues.Left, bold: true, fontSize: 14));
                        body.AppendChild(CreateParagraph(planbook.QualitiesObjective ?? string.Empty, JustificationValues.Left, fontSize: 14));

                        // Add Teaching Tools Section
                        body.AppendChild(CreateParagraph("II. THIẾT BỊ DẠY HỌC VÀ HỌC LIỆU", JustificationValues.Left, bold: true, fontSize: 14));
                        body.AppendChild(CreateParagraph(planbook.TeachingTools ?? string.Empty, JustificationValues.Left, fontSize: 14));

                        // Add Activities Section
                        body.AppendChild(CreateParagraph("III. TIẾN TRÌNH DẠY HỌC", JustificationValues.Left, bold: true, fontSize: 14));
                        foreach (var activity in planbook.Activities.OrderBy(a => a.Index))
                        {
                            body.AppendChild(CreateParagraph(activity.Title, JustificationValues.Left, bold: true, fontSize: 14));
                            body.AppendChild(CreateParagraph($"a) Mục tiêu:", JustificationValues.Left, bold: true, fontSize: 14));
                            body.AppendChild(CreateParagraph(activity.Objective ?? string.Empty, JustificationValues.Left, fontSize: 14));
                            body.AppendChild(CreateParagraph($"b) Nội dung:", JustificationValues.Left, bold: true, fontSize: 14));
                            body.AppendChild(CreateParagraph(activity.Content ?? string.Empty, JustificationValues.Left, fontSize: 14));
                            body.AppendChild(CreateParagraph($"c) Sản phẩm:", JustificationValues.Left, bold: true, fontSize: 14));
                            body.AppendChild(CreateParagraph(activity.Product ?? string.Empty, JustificationValues.Left, fontSize: 14));
                            body.AppendChild(CreateParagraph($"d) Tổ chức thực hiện:", JustificationValues.Left, bold: true, fontSize: 14));
                            body.AppendChild(CreateParagraph(activity.Implementation ?? string.Empty, JustificationValues.Left, fontSize: 14));
                        }

                        body.AppendChild(CreateParagraph("Ghi chú:", JustificationValues.Left, bold: true, fontSize: 14));
                        body.AppendChild(CreateParagraph(planbook.Notes ?? string.Empty, JustificationValues.Left, fontSize: 14));

                        // Add Footer Table
                        body.AppendChild(CreateFooterTable(
                            "Duyệt của lãnh đạo tổ",     // Left cell text
                            "Người soạn",                // Label
                            planbook.TeacherName ?? string.Empty, // Teacher name
                            14                           // Font size
                        ));

                        mainPart.Document.Save();
                    }

                    return new ResponseModel
                    {
                        Success = true,
                        Message = Convert.ToBase64String(memoryStream.ToArray()) // Return file content as Base64
                    };
                }
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while exporting the Planbook to Word.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        // Helper Method for Word Paragraphs
        private Word.Paragraph CreateParagraph(string text, Word.JustificationValues justification, bool bold = false, int fontSize = 12)
        {
            var runProperties = new Word.RunProperties
            {
                RunFonts = new Word.RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                FontSize = new Word.FontSize { Val = (fontSize * 2).ToString() },
                Bold = bold ? new Word.Bold() : null
            };

            var paragraphProperties = new Word.ParagraphProperties
            {
                Justification = new Word.Justification { Val = justification }
            };

            var paragraph = new Word.Paragraph(paragraphProperties);

            var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                var run = new Word.Run(runProperties.CloneNode(true));
                run.AppendChild(new Word.Text(line) { Space = DocumentFormat.OpenXml.SpaceProcessingModeValues.Preserve });
                paragraph.AppendChild(run);

                if (line != lines.Last())
                {
                    paragraph.AppendChild(new Word.Break());
                }
            }

            return paragraph;
        }

        // Helper Method for Word Table
        private Word.Table CreateTopTable(string schoolName, string team, string teacherLabel, string teacherName, int fontSize)
        {
            var table = new Word.Table(
                new Word.TableProperties(
                    new Word.TableBorders(
                        new Word.TopBorder { Val = Word.BorderValues.None },
                        new Word.BottomBorder { Val = Word.BorderValues.None },
                        new Word.LeftBorder { Val = Word.BorderValues.None },
                        new Word.RightBorder { Val = Word.BorderValues.None },
                        new Word.InsideHorizontalBorder { Val = Word.BorderValues.None },
                        new Word.InsideVerticalBorder { Val = Word.BorderValues.None }
                    ),
                    new Word.TableWidth { Width = "100%", Type = Word.TableWidthUnitValues.Pct }
                )
            );

            var tableRow = new Word.TableRow();

            // Left Cell
            var leftCell = new Word.TableCell(
                new Word.Paragraph(
                    new Word.Run(
                        new Word.RunProperties(
                            new Word.Bold(),
                            new Word.RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                            new Word.FontSize { Val = (fontSize * 2).ToString() }
                        ),
                        new Word.Text(schoolName) // School Name
                    )
                ),
                new Word.Paragraph(
                    new Word.Run(
                        new Word.RunProperties(
                            new Word.Bold(),
                            new Word.RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                            new Word.FontSize { Val = (fontSize * 2).ToString() }
                        ),
                        new Word.Text(team) // Team
                    )
                )
            );
            leftCell.Append(new Word.TableCellProperties(
                new Word.TableCellWidth { Width = "50%", Type = Word.TableWidthUnitValues.Pct } // Chiếm 50% chiều rộng
            ));
            tableRow.Append(leftCell);

            // Right Cell with no extra spacing
            var rightCell = new Word.TableCell(
                new Word.Paragraph(
                    new Word.ParagraphProperties(
                        new Word.Justification { Val = Word.JustificationValues.Right } // Căn phải đoạn văn
                    ),
                    new Word.Run(
                        new Word.RunProperties(
                            new Word.Bold(),
                            new Word.RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                            new Word.FontSize { Val = (fontSize * 2).ToString() }
                        ),
                        new Word.Text(teacherLabel) // "HỌ VÀ TÊN GIÁO VIÊN"
                    )
                ),
                new Word.Paragraph(
                    new Word.ParagraphProperties(
                        new Word.Justification { Val = Word.JustificationValues.Right } // Căn phải đoạn văn
                    ),
                    new Word.Run(
                        new Word.RunProperties(
                            new Word.RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                            new Word.FontSize { Val = (fontSize * 2).ToString() }
                        ),
                        new Word.Text(teacherName) // Teacher's Name
                    )
                )
            );
            rightCell.Append(new Word.TableCellProperties(
                new Word.TableCellWidth { Width = "50%", Type = Word.TableWidthUnitValues.Pct }, // Chiếm 50% chiều rộng
                        new Word.TableCellVerticalAlignment { Val = Word.TableVerticalAlignmentValues.Center } // Căn giữa theo chiều dọc
            ));
            tableRow.Append(rightCell);

            table.Append(tableRow);
            return table;
        }

        private Word.Table CreateFooterTable(string leftText, string teacherLabel, string teacherName, int fontSize)
        {
            var table = new Word.Table(
                new Word.TableProperties(
                    new Word.TableBorders(
                        new Word.TopBorder { Val = Word.BorderValues.None },
                        new Word.BottomBorder { Val = Word.BorderValues.None },
                        new Word.LeftBorder { Val = Word.BorderValues.None },
                        new Word.RightBorder { Val = Word.BorderValues.None },
                        new Word.InsideHorizontalBorder { Val = Word.BorderValues.None },
                        new Word.InsideVerticalBorder { Val = Word.BorderValues.None }
                    ),
                    new Word.TableWidth { Width = "100%", Type = Word.TableWidthUnitValues.Pct }
                )
            );
            var tableRow = new Word.TableRow();

            // Left Cell
            var leftCell = new Word.TableCell(
            new Word.Paragraph(
                    new Word.Run(
                        new Word.RunProperties(
                            new Word.Bold(),
                            new Word.RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                            new Word.FontSize { Val = (fontSize * 2).ToString() }
                        ),
                        new Word.Text(leftText) // "Duyệt của lãnh đạo tổ"
                    )
                )
            );
            leftCell.AppendChild(new Word.TableCellProperties(
                new Word.TableCellWidth { Type = Word.TableWidthUnitValues.Pct, Width = "50%" },
                new Word.TableCellMargin(
                    new Word.TopMargin { Width = "200", Type = Word.TableWidthUnitValues.Dxa },
                    new Word.LeftMargin { Width = "200", Type = Word.TableWidthUnitValues.Dxa }
                )
            ));
            tableRow.Append(leftCell);

            // Right Cell with spacing between "Người soạn" and Teacher Name
            var rightCell = new Word.TableCell(
                new Word.Paragraph(
                    new Word.ParagraphProperties(new Word.Justification { Val = Word.JustificationValues.Right }),
                    new Word.Run(
                        new Word.RunProperties(
                            new Word.Bold(),
                            new Word.RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                            new Word.FontSize { Val = (fontSize * 2).ToString() }
                        ),
                        new Word.Text(teacherLabel) // "Người soạn"
                    )
                ),
                new Word.Paragraph(new Word.Run(new Word.Break())),
                new Word.Paragraph(
                    new Word.ParagraphProperties(new Word.Justification { Val = Word.JustificationValues.Right }),
                    new Word.Run(
                        new Word.RunProperties(
                            new Word.Bold(),
                            new Word.RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                            new Word.FontSize { Val = (fontSize * 2).ToString() }
                        ),
                        new Word.Text(teacherName) // Teacher's Name
                    )
                )
            );
            rightCell.AppendChild(new Word.TableCellProperties(
                new Word.TableCellWidth { Type = Word.TableWidthUnitValues.Pct, Width = "50%" },
                new Word.TableCellMargin(
                    new Word.TopMargin { Width = "200", Type = Word.TableWidthUnitValues.Dxa },
                    new Word.RightMargin { Width = "2200", Type = Word.TableWidthUnitValues.Dxa }
                )
            ));
            tableRow.Append(rightCell);
            table.Append(tableRow);
            return table;
        }
        #endregion

        #region Export Planbook to Pdf
        public async Task<ResponseModel> ExportPlanbookToPdfAsync(string planbookId)
        {
            try
            {
                var planbook = await _unitOfWork.PlanbookRepository.GetByIdAsync(id: planbookId, includeProperties: "Activities");
                if (planbook is null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Planbook not found."
                    };
                }

                using (var memoryStream = new MemoryStream())
                {
                    var writer = new PdfWriter(memoryStream);
                    using (var pdf = new PdfDocument(writer))
                    {
                        var document = new iText.Layout.Document(pdf);

                        // Load the font
                        var fontPath = Path.Combine(AppContext.BaseDirectory, "Resources", "Fonts", "SVNTimesNewRoman2.ttf");
                        if (!File.Exists(fontPath))
                            throw new FileNotFoundException($"Font file not found: {fontPath}");
                        var font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);

                        // Header Table
                        var headerTable = new iText.Layout.Element.Table(2).UseAllAvailableWidth();
                        headerTable.SetMarginBottom(10); // Adjust top margin for overall spacing
                        headerTable.AddCell(new iText.Layout.Element.Cell()
                            .Add(new iText.Layout.Element.Paragraph("TRƯỜNG " + planbook.SchoolName?.ToUpper())
                                .SetFont(font)
                                .SetBold()
                                .SetFontSize(14))
                            .Add(new iText.Layout.Element.Paragraph($"TỔ ...") //{planbook.Team?.ToUpper()}
                                .SetFont(font)
                                .SetBold()
                                .SetFontSize(14))
                            .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT));
                        headerTable.AddCell(new iText.Layout.Element.Cell()
                            .Add(new iText.Layout.Element.Paragraph("HỌ VÀ TÊN GIÁO VIÊN")
                                .SetFont(font)
                                .SetBold()
                                .SetFontSize(14))
                            .Add(new iText.Layout.Element.Paragraph(planbook.TeacherName?.ToUpper())
                                .SetFont(font)
                                .SetFontSize(14))
                            .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));
                        document.Add(headerTable);

                        // Title and Other Details
                        document.Add(new iText.Layout.Element.Paragraph($"TÊN BÀI DẠY: {planbook.Title?.ToUpper()}")
                            .SetFont(font)
                            .SetBold()
                            .SetFontSize(14)
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));
                        document.Add(new iText.Layout.Element.Paragraph($"Môn học: {planbook.Subject}; lớp: {planbook.ClassName}")
                            .SetFont(font)
                            .SetFontSize(14)
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));
                        document.Add(new iText.Layout.Element.Paragraph($"Thời gian thực hiện: ({planbook.DurationInPeriods} tiết)")
                            .SetFont(font)
                            .SetFontSize(14)
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                        // Objectives Section
                        document.Add(new iText.Layout.Element.Paragraph("I. MỤC TIÊU")
                            .SetFont(font)
                            .SetBold()
                            .SetFontSize(14));
                        document.Add(new iText.Layout.Element.Paragraph($"1. Về kiến thức:")
                            .SetFont(font)
                            .SetBold()
                            .SetFontSize(14));
                        document.Add(new iText.Layout.Element.Paragraph(planbook.KnowledgeObjective ?? string.Empty)
                            .SetFont(font)
                            .SetFontSize(14));
                        document.Add(new iText.Layout.Element.Paragraph($"2. Về năng lực:")
                            .SetFont(font)
                            .SetBold()
                            .SetFontSize(14));
                        document.Add(new iText.Layout.Element.Paragraph(planbook.SkillsObjective ?? string.Empty)
                            .SetFont(font)
                            .SetFontSize(14));
                        document.Add(new iText.Layout.Element.Paragraph($"3. Về phẩm chất:")
                            .SetFont(font)
                            .SetBold()
                            .SetFontSize(14));
                        document.Add(new iText.Layout.Element.Paragraph(planbook.QualitiesObjective ?? string.Empty)
                            .SetFont(font)
                            .SetFontSize(14));

                        // Teaching Tools Section
                        document.Add(new iText.Layout.Element.Paragraph("II. THIẾT BỊ DẠY HỌC VÀ HỌC LIỆU")
                            .SetFont(font)
                            .SetBold()
                            .SetFontSize(14));
                        document.Add(new iText.Layout.Element.Paragraph(planbook.TeachingTools ?? string.Empty)
                            .SetFont(font)
                            .SetFontSize(14));

                        // Activities Section
                        document.Add(new iText.Layout.Element.Paragraph("III. TIẾN TRÌNH DẠY HỌC")
                            .SetFont(font)
                            .SetBold()
                            .SetFontSize(14));
                        foreach (var activity in planbook.Activities.OrderBy(a => a.Index))
                        {
                            document.Add(new iText.Layout.Element.Paragraph(activity.Title)
                                .SetFont(font)
                                .SetBold()
                                .SetFontSize(14));
                            document.Add(new iText.Layout.Element.Paragraph($"a) Mục tiêu:")
                                .SetFont(font)
                                .SetBold()
                                .SetFontSize(14));
                            document.Add(new iText.Layout.Element.Paragraph(activity.Objective ?? string.Empty)
                                .SetFont(font)
                                .SetFontSize(14));
                            document.Add(new iText.Layout.Element.Paragraph($"b) Nội dung:")
                                .SetFont(font)
                                .SetBold()
                                .SetFontSize(14));
                            document.Add(new iText.Layout.Element.Paragraph(activity.Content ?? string.Empty)
                                .SetFont(font)
                                .SetFontSize(14));
                            document.Add(new iText.Layout.Element.Paragraph($"c) Sản phẩm:")
                                .SetFont(font)
                                .SetBold()
                                .SetFontSize(14));
                            document.Add(new iText.Layout.Element.Paragraph(activity.Product ?? string.Empty)
                                .SetFont(font)
                                .SetFontSize(14));
                            document.Add(new iText.Layout.Element.Paragraph($"d) Tổ chức thực hiện:")
                                .SetFont(font)
                                .SetBold()
                                .SetFontSize(14));
                            document.Add(new iText.Layout.Element.Paragraph(activity.Implementation ?? string.Empty)
                                .SetFont(font)
                                .SetFontSize(14));
                        }
                        document.Add(new iText.Layout.Element.Paragraph("Ghi chú:")
                           .SetFont(font)
                           .SetBold()
                           .SetFontSize(14));
                        document.Add(new iText.Layout.Element.Paragraph(planbook.Notes ?? string.Empty)
                            .SetFont(font)
                            .SetFontSize(14));

                        // Footer Table
                        var footerTable = new iText.Layout.Element.Table(2).UseAllAvailableWidth();
                        footerTable.SetMarginTop(20); // Adjust top margin for overall spacing

                        // Left Cell
                        footerTable.AddCell(
                            new iText.Layout.Element.Cell()
                                .Add(new iText.Layout.Element.Paragraph("Duyệt của lãnh đạo tổ")
                                    .SetFont(font)
                                    .SetBold()
                                    .SetFontSize(14))
                                .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                                .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                        );

                        // Right Cell
                        footerTable.AddCell(
                            new iText.Layout.Element.Cell()
                                .Add(new iText.Layout.Element.Paragraph("Người soạn")
                                    .SetFont(font)
                                    .SetBold()
                                    .SetFontSize(14))
                                .Add(new iText.Layout.Element.Paragraph("\n") // Add empty line
                                    .SetFont(font)
                                    .SetFontSize(14))
                                .Add(new iText.Layout.Element.Paragraph(planbook.TeacherName)
                                    .SetFont(font)
                                    .SetBold()
                                    .SetFontSize(14))
                                .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                                .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                        );

                        // Add Footer Table to Document
                        document.Add(footerTable);
                    }

                    return new ResponseModel
                    {
                        Success = true,
                        Message = Convert.ToBase64String(memoryStream.ToArray()) // Return file content as Base64
                    };
                }
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while exporting the Planbook to Pdf.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        #endregion

        #region Manage Planbook Templates
        public async Task<ResponseModel> GetAllPlanbookTemplatesAsync(int pageIndex, int pageSize)
        {
            try
            {
                var planbookTemplates = await _unitOfWork.PlanbookRepository.GetAsync(
                                                                        filter: p => p.IsDefault,
                                                                        orderBy: p => p.OrderBy(p => p.Title),
                                                                        includeProperties: "Lesson.Chapter.SubjectInCurriculum.Subject,Lesson.Chapter.SubjectInCurriculum.Curriculum,Lesson.Chapter.SubjectInCurriculum.Grade",
                                                                        pageIndex: pageIndex,
                                                                        pageSize: pageSize);

                var planbookTemplateDtos = _mapper.Map<Pagination<ViewListPlanbookTemplateDTO>>(planbookTemplates);

                //// Lặp qua từng item để gắn giá trị Subject đã xử lý
                //foreach (var planbook in planbookTemplates.Items)
                //{
                //    // Lấy thông tin Subject, Grade, Curriculum từ thực thể
                //    var subjectName = planbook.Lesson?.Chapter?.SubjectInCurriculum?.Subject?.Name ?? string.Empty;
                //    var gradeName = planbook.Lesson?.Chapter?.SubjectInCurriculum?.Grade?.Name ?? string.Empty;
                //    var curriculumName = planbook.Lesson?.Chapter?.SubjectInCurriculum?.Curriculum?.Name ?? string.Empty;

                //    // Lấy 2 từ đầu tiên từ Curriculum
                //    var curriculumWords = !string.IsNullOrWhiteSpace(curriculumName)
                //        ? curriculumName.Split(' ').Take(2)
                //        : new string[] { };

                //    // Tìm DTO tương ứng
                //    var dto = planbookTemplateDtos.Items.FirstOrDefault(p => p.PlanbookId == planbook.PlanbookId);
                //    if (dto != null)
                //    {
                //        // Gắn giá trị vào Subject trong DTO
                //        dto.Subject = $"{subjectName} - {gradeName} - {string.Join(" ", curriculumWords)}";
                //    }
                //}

                return new SuccessResponseModel<object>
                {
                    Success = true,
                    Message = "Planbook Templates retrieved successfully.",
                    Data = planbookTemplateDtos
                };

            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while getting all Planbook Templates.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        #endregion
    }
}
