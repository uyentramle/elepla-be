using AutoMapper;
using Elepla.Domain.Entities;
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

			var result = _mapper.Map<ViewDetailArticleDTO>(article);

			return new SuccessResponseModel<object>
			{
				Success = true,
				Message = "Article retrieved successfully.",
				Data = result
			};
		}

		public async Task<ResponseModel> CreateArticleAsync(CreateArticleDTO model)
		{
			try
			{
				var article = _mapper.Map<Article>(model);
				article.Url = _urlService.RemoveDiacritics(model.Title).Replace(" ", "-").ToLower();

				switch (model.Status)
				{
					case "Published":
						article.Status = "Published";
						break;
					case "Draft":
						article.Status = "Draft";
						break;
					case "InActive":
						article.Status = "InActive";
						break;
					case "Trashed":
						article.Status = "Trashed";
						break;
					default:
						article.Status = "Draft";
						break;
				}

				return new ResponseModel
				{
					Success = true,
					Message = "Article created successfully."
				};
			}
			catch (Exception ex)
			{
				return new ResponseModel
				{
					Success = false,
					Message = ex.Message
				};
			}
		}

		public async Task<ResponseModel> UpdateArticleAsync(UpdateArticleDTO model)
		{
			try
			{
				var article = await _unitOfWork.ArticleRepository.GetByIdAsync(model.ArticleId);

				if (article == null)
				{
					return new ResponseModel
					{
						Success = false,
						Message = "Article not found."
					};
				}

				article = _mapper.Map(model, article);
				article.Url = _urlService.RemoveDiacritics(model.Title).Replace(" ", "-").ToLower();

				switch (model.Status)
				{
					case "Published":
						article.Status = "Published";
						break;
					case "Draft":
						article.Status = "Draft";
						break;
					case "InActive":
						article.Status = "InActive";
						break;
					case "Trashed":
						article.Status = "Trashed";
						_unitOfWork.ArticleRepository.SoftRemove(article);
						await _unitOfWork.SaveChangeAsync();
						break;
					default:
						article.Status = "Draft";
						break;
				}

				return new ResponseModel
				{
					Success = true,
					Message = "Article updated successfully."
				};
			}
			catch (Exception ex)
			{
				return new ResponseModel
				{
					Success = false,
					Message = ex.Message
				};
			}
		}

		public async Task<ResponseModel> DeleteArticleAsync(string id)
		{
			try
			{
				var article = await _unitOfWork.ArticleRepository.GetByIdAsync(id);

				if (article == null)
				{
					return new ResponseModel
					{
						Success = false,
						Message = "Article not found."
					};
				}

				if (article.IsDeleted == true)
				{
					return new ResponseModel
					{
						Success = false,
						Message = "Can't delete article is deleted."
					};
				}

				article.Status = "Trashed";
				_unitOfWork.ArticleRepository.SoftRemove(article);
				await _unitOfWork.SaveChangeAsync();

				return new ResponseModel
				{
					Success = true,
					Message = "Article deleted successfully."
				};
			}
			catch (Exception ex)
			{
				return new ResponseModel
				{
					Success = false,
					Message = ex.Message
				};
			}
		}
	}
}
