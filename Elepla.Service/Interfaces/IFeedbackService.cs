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
    }
}
