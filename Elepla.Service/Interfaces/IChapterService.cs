using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.ChapterViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Interfaces
{
    public interface IChapterService
    {
        Task<ResponseModel> GetAllChapterAsync(string? keyword, int pageIndex, int pageSize);
        Task<ResponseModel> GetAllChapterBySubjectInCurriculumIdAsync(string subjectInCurriculumId);
        Task<ResponseModel> GetChapterByIdAsync(string chapterId);
        Task<ResponseModel> CreateChapterAsync(CreateChapterDTO model);
        Task<ResponseModel> UpdateChapterAsync(UpdateChapterDTO model);
        Task<ResponseModel> DeleteChapterAsync(string chapterId);
    }
}
