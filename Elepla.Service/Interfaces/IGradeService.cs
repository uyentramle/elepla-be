using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.GradeViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Interfaces
{
    public interface IGradeService
    {
        Task<ResponseModel> GetAllGradeAsync(string? keyword, int pageIndex, int pageSize);
        Task<ResponseModel> GetGradeByIdAsync(string gradeId);
        Task<ResponseModel> CreateGradeAsync(CreateGradeDTO model);
        Task<ResponseModel> UpdateGradeAsync(UpdateGradeDTO model);
        Task<ResponseModel> DeleteGradeAsync(string gradeId);
    }
}
