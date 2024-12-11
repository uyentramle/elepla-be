using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.FeedbackViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Interfaces
{
    public interface IFeedbackService
    {
        Task<ResponseModel> GetFeedbackByPlanbookIdAsync(string planbookId, int pageIndex, int pageSize);
        Task<ResponseModel> SubmitFeedbackAsync(CreateFeedbackDTO model);
        Task<ResponseModel> UpdateFeedbackAsync(UpdateFeedbackDTO model);
        Task<ResponseModel> HardDeleteFeedbackAsync(string feedbackId);
        Task<ResponseModel> FlagFeedbackAsync(string feedbackId);
        Task<ResponseModel> GetFlaggedFeedbackAsync(int pageIndex, int pageSize);
        Task<ResponseModel> GetSystemFeedbackAsync(int pageIndex, int pageSize);
        Task<ResponseModel> GetPlanbookFeedbackAsync(int pageIndex, int pageSize);
        Task<ResponseModel> FlagCommentAsync(string feedbackId);
    }
}