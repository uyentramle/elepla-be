using AutoMapper;
using Elepla.Repository.Common;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.PlanbookCollectionViewModels;
using Elepla.Service.Models.ViewModels.PlanbookViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Services
{
	public class PlanbookCollectionService : IPlanbookCollectionService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public PlanbookCollectionService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		#region Get Planbook Collections By Teacher Id
		public async Task<ResponseModel> GetPlanbookCollectionsByTeacherIdAsync(string teacherId, int pageIndex, int pageSize)
		{
			var collections = await _unitOfWork.PlanbookCollectionRepository.GetAsync(
								filter: r => r.TeacherId == teacherId && r.IsDeleted == false,
								pageIndex: pageIndex,
								pageSize: pageSize
							 );

			var mapper = _mapper.Map<Pagination<ViewListPlanbookCollectionDTO>>(collections);

			// Get planbook count for each collection
			//	foreach (var collection in mapper.Items)
			//	{
			//		var planbooks = await _unitOfWork.PlanbookRepository.GetAsync(
			//							filter: r => r.CollectionId == collection.CollectionId
			//									&& r.IsDeleted == false
			//									);
			//		collection.PlanbookCount = planbooks.Items.Count;
			//	}

			var collectionIds = mapper.Items.Select(c => c.CollectionId).ToList();
			var planbooks = await _unitOfWork.PlanbookRepository.GetAsync(
								filter: r => collectionIds.Contains(r.CollectionId) && r.IsDeleted == false
							 );

			var planbookCountsByCollection = planbooks.Items
											.GroupBy(p => p.CollectionId)
											.ToDictionary(g => g.Key, g => g.Count());

			// Assign planbook counts to the respective collections
			foreach (var collection in mapper.Items)
			{
				if (planbookCountsByCollection.TryGetValue(collection.CollectionId, out var planbookCount))
				{
					collection.PlanbookCount = planbookCount;
				}
				else
				{
					collection.PlanbookCount = 0;
				}
			}

			return new SuccessResponseModel<object>
			{
				Message = "Planbook Collections retrieved successfully",
				Success = true,
				Data = mapper
			};
		}
		#endregion

		#region Get Planbook Collection By Id
		public async Task<ResponseModel> GetCollectionByIdAsync(string collectionId)
		{
			var collection = await _unitOfWork.PlanbookCollectionRepository.GetByIdAsync(collectionId);
			if (collection == null)
			{
				return new ResponseModel
				{
					Message = "Planbook Collection not found",
					Success = false
				};
			}

			var mapper = _mapper.Map<ViewDetailPlanbookCollectionDTO>(collection);

			var planbooks = await _unitOfWork.PlanbookRepository.GetAsync(
								filter: r => r.CollectionId == collectionId
										&& r.IsDeleted == false
										);
			mapper.Planbooks = _mapper.Map<List<ViewListPlanbookDTO>>(planbooks.Items);
			mapper.PlanbookCount = planbooks.Items.Count;

			return new SuccessResponseModel<object>
			{
				Message = "Planbook Collection retrieved successfully",
				Success = true,
				Data = mapper
			};
		}
		#endregion
	}
}
