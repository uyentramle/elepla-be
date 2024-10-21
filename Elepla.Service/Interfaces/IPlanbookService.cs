using Elepla.Service.Models.ResponseModels;
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
        Task<ResponseModel> GenerateLessonObjectivesAsync(string lessonId);

    }
}
