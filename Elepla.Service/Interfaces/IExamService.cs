using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.ExamViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Interfaces
{
    public interface IExamService
    {
        Task<ResponseModel> GetExamsByUserIdAsync(string userId);
        Task<ResponseModel> GetExamByIdAsync(string examId);
        Task<ResponseModel> CreateExamAsync(CreateExamDTO model);
        Task<ResponseModel> UpdateExamAsync(UpdateExamDTO model);
        Task<ResponseModel> DeleteExamAsync(string examId);
        Task<ResponseModel> ExportExamToWordAsync(string examId);
        Task<ResponseModel> ExportExamToPdfAsync(string examId);
        Task<ResponseModel> ExportExamToWordNoColorAsync(string examId);
        Task<ResponseModel> ExportExamToPdfNoColorAsync(string examId);
    }
}
