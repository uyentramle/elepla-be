using AutoMapper;
using Elepla.Repository.Common;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.ActivityViewModels;
using Elepla.Service.Models.ViewModels.PlanbookViewModels;
using Elepla.Service.Utils;
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
        private readonly IOpenAiService _openAiService;

        public PlanbookService(IUnitOfWork unitOfWork, IMapper mapper, IOpenAiService openAiService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _openAiService = openAiService; 
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


        #region Generate Lesson Objectives
        public async Task<ResponseModel> GenerateLessonObjectivesAsync(string lessonId)
        {
            var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(lessonId);

            if (lesson == null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "Lesson not found."
                };
            }

            if (lesson.Chapter == null || lesson.Chapter.SubjectInCurriculum == null ||
                lesson.Chapter.SubjectInCurriculum.Subject == null || lesson.Chapter.SubjectInCurriculum.Grade == null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "Lesson's related data (Chapter, Subject, or Grade) is incomplete."
                };
            }

            // Construct prompts
            var knowledgePrompt = $"Hãy tạo một mục tiêu kiến thức cho bài học có tiêu đề '{lesson.Name}' thuộc môn {lesson.Chapter.SubjectInCurriculum.Subject.Name} cho lớp {lesson.Chapter.SubjectInCurriculum.Grade.Name} bằng tiếng Việt.";
            var skillsPrompt = $"Hãy tạo một mục tiêu kỹ năng cho bài học có tiêu đề '{lesson.Name}' thuộc môn {lesson.Chapter.SubjectInCurriculum.Subject.Name} cho lớp {lesson.Chapter.SubjectInCurriculum.Grade.Name} bằng tiếng Việt.";
            var qualitiesPrompt = $"Hãy tạo một mục tiêu phẩm chất cho bài học có tiêu đề '{lesson.Name}' thuộc môn {lesson.Chapter.SubjectInCurriculum.Subject.Name} cho lớp {lesson.Chapter.SubjectInCurriculum.Grade.Name} bằng tiếng Việt.";
            var teachingToolsPrompt = $"Hãy tạo danh sách các công cụ giảng dạy cho bài học có tiêu đề '{lesson.Name}' thuộc môn {lesson.Chapter.SubjectInCurriculum.Subject.Name} cho lớp {lesson.Chapter.SubjectInCurriculum.Grade.Name} bằng tiếng Việt.";

            // Call OpenAI for each objective
            var knowledgeObjective = await _openAiService.GeneratePlanbookField(knowledgePrompt);
            var skillsObjective = await _openAiService.GeneratePlanbookField(skillsPrompt);
            var qualitiesObjective = await _openAiService.GeneratePlanbookField(qualitiesPrompt);
            var teachingTools = await _openAiService.GeneratePlanbookField(teachingToolsPrompt);

            // Create a combined result
            var objectives = new
            {
                KnowledgeObjective = knowledgeObjective,
                SkillsObjective = skillsObjective,
                QualitiesObjective = qualitiesObjective,
                TeachingTools = teachingTools
            };

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Các mục tiêu bài học đã được tạo thành công.",
                Data = objectives
            };
        }
        #endregion

    }
}
