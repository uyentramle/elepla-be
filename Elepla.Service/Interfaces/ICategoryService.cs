using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.CategoryViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Interfaces
{
	public interface ICategoryService
	{
		Task<ResponseModel> GetAllCategoryAsync(int pageIndex, int pageSize);
		Task<ResponseModel> GetCategoryByIdAsync(string id);
		Task<ResponseModel> CreateCategoryAsync(CreateCategoryDTO model);
		Task<ResponseModel> UpdateCategoryAsync(UpdateCategoryDTO model);
		Task<ResponseModel> DeleteCategoryAsync(string id);
	}
}
