using AutoMapper;
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
	}
}
