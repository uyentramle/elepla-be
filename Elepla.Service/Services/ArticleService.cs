using AutoMapper;
using Elepla.Repository.Common;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.ArticleViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
		private readonly IUrlService _urlService;

		public ArticleService(IUnitOfWork unitOfWork, IMapper mapper, IUrlService urlService)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_urlService = urlService;
		}

		public async Task<ResponseModel> GetAllArticleAsync(int pageIndex, int pageSize)
		{
			var articles = await _unitOfWork.ArticleRepository.GetAsync(
				filter: r => r.IsDeleted == false,
				pageIndex: pageIndex,
				pageSize: pageSize
				);
			var articleDtos = _mapper.Map<Pagination<ViewListArticleDTO>>(articles);

			return new SuccessResponseModel<object>
			{
				Success = true,
				Message = "Articles retrieved successfully.",
				Data = articleDtos
			};
		}

		public async Task<ResponseModel> GetArticleByIdAsync(string id)
		{
			var article = await _unitOfWork.ArticleRepository.GetByIdAsync(id);

			if (article == null)
			{
				return new ErrorResponseModel<object>
				{
					Success = false,
					Message = "Article not found."
				};
			}

			var result = _mapper.Map<ViewListArticleDTO>(article);

			return new SuccessResponseModel<object>
			{
				Success = true,
				Message = "Article retrieved successfully.",
				Data = result
			};
		}
	}
}
