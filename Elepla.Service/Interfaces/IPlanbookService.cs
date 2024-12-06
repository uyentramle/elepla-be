using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.PlanbookCollectionViewModels;
using Elepla.Service.Models.ViewModels.PlanbookViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Interfaces
{
    public interface IPlanbookService
    {
		Task<ResponseModel> GetAllPlanbooksAsync(int pageIndex, int pageSize);
		Task<ResponseModel> GetPlanbookByIdAsync(string planbookId);
		Task<ResponseModel> GetPlanbookByCollectionIdAsync(string collectionId, int pageIndex, int pageSize);
		Task<ResponseModel> GetPlanbookByLessonIdAsync(string lessonId, int pageIndex, int pageSize);
		Task<ResponseModel> CreatePlanbookAsync(CreatePlanbookDTO model);
		Task<ResponseModel> UpdatePlanbookAsync(UpdatePlanbookDTO model);
		Task<ResponseModel> DeletePlanbookAsync(string planbookId);
        Task<ResponseModel> SoftRemovePlanbookAsync(string planbookId);
        Task<ResponseModel> GetPlanbookFromTemplateAsync(string lessonId);
		Task<ResponseModel> GetPlanbookUsingAIAsync(string lessonId);
		Task<ResponseModel> ClonePlanbookAsync(ClonePlanbookDTO model);
		Task<ResponseModel> SavePlanbookAsync(SavePlanbookDTO model);
        Task<ResponseModel> ExportPlanbookToWordAsync(string planbookId);
        Task<ResponseModel> ExportPlanbookToPdfAsync(string planbookId);
		Task<ResponseModel> GetAllPlanbookTemplatesAsync(int pageIndex, int pageSize);
    }
}
