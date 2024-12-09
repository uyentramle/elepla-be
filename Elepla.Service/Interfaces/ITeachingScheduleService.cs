using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.TeachingScheduleModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Interfaces
{
    public interface ITeachingScheduleService
    {
        Task<ResponseModel> GetAllTeachingSchedulesAsync(string? keyword, int pageIndex, int pageSize);

        Task<ResponseModel> GetTeachingScheduleByIdAsync(string scheduleId);

        Task<ResponseModel> GetTeachingSchedulesByUserIdAsync(string userId, int pageIndex, int pageSize);

        Task<ResponseModel> AddTeachingScheduleAsync(CreateTeachingScheduleDTO model);

        Task<ResponseModel> UpdateTeachingScheduleAsync(UpdateTeachingScheduleDTO model);

        Task<ResponseModel> DeleteTeachingScheduleAsync(string scheduleId);

        Task<ResponseModel> ImportToGoogleCalendarAsync(string scheduleId, string calendarId, string accessToken);
        Task<string> CreateCalendarAsync();
    }
}
