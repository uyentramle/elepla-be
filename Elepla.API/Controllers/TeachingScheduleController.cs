using Elepla.Service.Interfaces;
using Elepla.Service.Models.ViewModels.TeachingScheduleModels;
using Elepla.Service.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Elepla.API.Controllers
{
    public class TeachingScheduleController : BaseController
    {
        private readonly ITeachingScheduleService _teachingScheduleService;
        private readonly IGoogleCalendarService _googleCalendarService;

        public TeachingScheduleController(ITeachingScheduleService teachingScheduleService, IGoogleCalendarService googleCalendarService)
        {
            _teachingScheduleService = teachingScheduleService;
            _googleCalendarService = googleCalendarService;
        }

        #region Get All Teaching Schedules
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllTeachingSchedulesAsync(string? keyword, int pageIndex = 0, int pageSize = 10)
        {
            var response = await _teachingScheduleService.GetAllTeachingSchedulesAsync(keyword, pageIndex, pageSize);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion

        #region Get Teaching Schedule by ID
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetTeachingScheduleByIdAsync(string scheduleId)
        {
            var response = await _teachingScheduleService.GetTeachingScheduleByIdAsync(scheduleId);
            if (response.Success)
            {
                return Ok(response);
            }
            return NotFound(response);
        }
        #endregion

        #region Get Teaching Schedules by UserId
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetTeachingSchedulesByUserIdAsync(string userId, int pageIndex = 0, int pageSize = 10)
        {
            var response = await _teachingScheduleService.GetTeachingSchedulesByUserIdAsync(userId, pageIndex, pageSize);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion

        #region Add New Teaching Schedule
        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> AddTeachingScheduleAsync(CreateTeachingScheduleDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _teachingScheduleService.AddTeachingScheduleAsync(model);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion

        #region Update Teaching Schedule
        [HttpPut]
        //[Authorize]
        public async Task<IActionResult> UpdateTeachingScheduleAsync(UpdateTeachingScheduleDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _teachingScheduleService.UpdateTeachingScheduleAsync(model);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion

        #region Delete Teaching Schedule
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteTeachingScheduleAsync(string scheduleId)
        {
            var response = await _teachingScheduleService.DeleteTeachingScheduleAsync(scheduleId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion

        #region Get Google Authorization URL
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> GetGoogleAuthUrls([FromBody] List<string> scheduleIds)
        {
            var response = await _teachingScheduleService.GetGoogleAuthUrlsAsync(scheduleIds);
            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
        #endregion

        #region Add Teaching Schedule to Google Calendar
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddMultipleTeachingSchedulesToGoogleCalendar([FromBody] AddMultipleSchedulesRequest request)
        {
            if (request.ScheduleIds == null || request.ScheduleIds.Count == 0)
            {
                return BadRequest(new { message = "No schedule IDs provided." });
            }

            var response = await _teachingScheduleService.AddMultipleTeachingSchedulesToGoogleCalendarAsync(request.ScheduleIds, request.AuthorizationCode);
            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
        #endregion
    }
}
