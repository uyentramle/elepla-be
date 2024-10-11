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

		#region Get All Article Not Trashed
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
		#endregion

		#region Get All Articles by Category Id
		public async Task<ResponseModel> GetAllArticlesByCategoryIdAsync(string categoryId, int pageIndex, int pageSize)
		{
			var articles = await _unitOfWork.ArticleRepository.GetAsync(
				filter: r => r.ArticleCategories.Any(c => c.CategoryId == categoryId),
				pageIndex: pageIndex,
				pageSize: pageSize
			);

			var articleDtos = _mapper.Map<Pagination<ViewListArticleDTO>>(articles);

			foreach (var item in articleDtos.Items)
			{
				Image? image = await _unitOfWork.ImageRepository.GetByIdAsync(item.ArticleId);
				if (image != null)
					item.Thumb = image.ImageUrl;
			}

			return new SuccessResponseModel<object>
			{
				Success = true,
				Message = "Articles retrieved successfully.",
				Data = articleDtos
			};
		}

		#endregion

		#region Get Article By Id
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
		#endregion

		#region Create Article
		public async Task<ResponseModel> CreateArticleAsync(CreateArticleDTO model)
		{
			try
			{
				var article = _mapper.Map<Article>(model);
				if (model.Slug != null)
				{
					article.Url = _urlService.RemoveDiacritics(model.Slug).Replace(" ", "-").ToLower();
				}
				else
				{
					article.Url = _urlService.RemoveDiacritics(model.Title).Replace(" ", "-").ToLower();
				}

				if (model.Content != null)
				{
					article.Excerpt = model.Content.Substring(0, 100);
				}

				if (model.Status == null)
				{
					article.Status = "Draft";
				}
				else if (model.Status == "Trashed")
				{
					article.Status = "Trashed";
					_unitOfWork.ArticleRepository.SoftRemove(article);
					await _unitOfWork.SaveChangeAsync();
				}

				await _unitOfWork.ArticleRepository.AddAsync(article);
				await _unitOfWork.SaveChangeAsync();

				if (model.Categories != null)
				{
					foreach (var categoryId in model.Categories)
					{
						var articleCategory = new ArticleCategory
						{
							ArticleId = article.ArticleId,
							CategoryId = categoryId.ToString()
						};
						article.ArticleCategories.Add(articleCategory);
						await _unitOfWork.SaveChangeAsync();
					}
				}

				if (model.Thumb != null)
				{
					var thumb = new Image
					{
						ImageId = Guid.NewGuid().ToString(),
						ImageUrl = model.Thumb,
						Type = "Article Thumb"
					};
					await _unitOfWork.ImageRepository.AddAsync(thumb);
					await _unitOfWork.SaveChangeAsync();

					var articleImageThumb = new ArticleImage
					{
						ArticleId = article.ArticleId,
						ImageId = thumb.ImageId
					};
					article.ArticleImages.Add(articleImageThumb);
					await _unitOfWork.SaveChangeAsync();
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
		#endregion

		#region Update Article
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

				var mapper = _mapper.Map(model, article);
				if (model.Slug != null)
				{
					article.Url = _urlService.RemoveDiacritics(model.Slug).Replace(" ", "-").ToLower();
				}
				else
				{
					article.Url = _urlService.RemoveDiacritics(model.Title).Replace(" ", "-").ToLower();
				}

				if (model.Content != null)
				{
					article.Excerpt = model.Content.Substring(0, 100);
				}

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

				_unitOfWork.ArticleRepository.Update(mapper);
				await _unitOfWork.SaveChangeAsync();

				if (model.Categories != null)
				{
					var articleCategories = new List<ArticleCategory>();
					foreach (var categoryId in model.Categories)
					{
						var articleCategory = new ArticleCategory
						{
							ArticleId = article.ArticleId,
							CategoryId = categoryId.ToString()
						};
						articleCategories.Add(articleCategory);
						await _unitOfWork.SaveChangeAsync();
					}
				}

				if (model.Thumb != null)
				{
					var thumb = new Image
					{
						ImageId = Guid.NewGuid().ToString(),
						ImageUrl = model.Thumb,
						Type = "Article Thumb"
					};
					await _unitOfWork.ImageRepository.AddAsync(thumb);
					await _unitOfWork.SaveChangeAsync();

					var articleImageThumb = new ArticleImage
					{
						ArticleId = article.ArticleId,
						ImageId = thumb.ImageId
					};
					article.ArticleImages.Add(articleImageThumb);
					await _unitOfWork.SaveChangeAsync();
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
		#endregion

		#region Delete Article
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
		#endregion
	}
}
