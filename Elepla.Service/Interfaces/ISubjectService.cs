using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.SubjectViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Interfaces
{
    public interface ISubjectService
    {
        Task<ResponseModel> GetAllSubjectAsync(string? keyword, int pageIndex, int pageSize);
        Task<ResponseModel> GetSubjectByIdAsync(string subjectId);
        Task<ResponseModel> CreateSubjectAsync(CreateSubjectDTO model);
        Task<ResponseModel> UpdateSubjectAsync(UpdateSubjectDTO model);
        Task<ResponseModel> DeleteSubjectAsync(string subjectId);
    }
}
