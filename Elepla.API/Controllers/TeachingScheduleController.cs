using Elepla.Service.Interfaces;
using Elepla.Service.Models.ViewModels.TeachingScheduleModels;
using Elepla.Service.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        #region Add Teaching Schedule To Google Calendar
        [HttpPost]
        public async Task<IActionResult> AddTeachingScheduleToGoogleCalendar(string scheduleId)
        {
            var response = await _teachingScheduleService.AddTeachingScheduleToGoogleCalendarAsync(scheduleId);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
        #endregion
    }
}
