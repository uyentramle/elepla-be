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
		Task<ResponseModel> GetAllCategory(int pageIndex, int pageSize);
		Task<ResponseModel> GetCategoryById(string id);
		Task<ResponseModel> CreateCategory(CreateCategoryDTO model);
		Task<ResponseModel> UpdateCategory(UpdateCategoryDTO model);
		Task<ResponseModel> DeleteCategory(string id);
	}
}
