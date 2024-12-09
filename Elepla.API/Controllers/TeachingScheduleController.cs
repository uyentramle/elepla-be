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

        [HttpPost("AuthorizeGoogle")]
        public async Task<IActionResult> AuthorizeGoogleAsync()
        {
            try
            {
                var tokenPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Tokens");
                await _googleCalendarService.InitializeServiceFromCredentialFileAsync(tokenPath);

                return Ok(new
                {
                    Success = true,
                    Message = "Google Calendar service initialized successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Failed to initialize Google Calendar service.",
                    Error = ex.Message
                });
            }
        }

        #region Import To Google Calendar
        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> ImportTeachingScheduleAsync(string scheduleId, string calendarId)
        {
            try
            {
                var tokenPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Tokens");
                await _googleCalendarService.InitializeServiceFromCredentialFileAsync(tokenPath);

                var response = await _teachingScheduleService.ImportToGoogleCalendarAsync(scheduleId, calendarId, null);
                if (response.Success)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Failed to import teaching schedule to Google Calendar.",
                    Error = ex.Message
                });
            }
        }
        #endregion

        #region List Calendars
        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> ListCalendarsAsync()
        {
            var calendars = await _googleCalendarService.GetCalendarListAsync();

            if (calendars.Any())
            {
                return Ok(new
                {
                    Success = true,
                    Message = "Calendars retrieved successfully.",
                    Data = calendars.Select(c => new
                    {
                        c.Id,
                        c.Summary,
                        c.Description,
                        c.TimeZone
                    })
                });
            }

            return BadRequest(new
            {
                Success = false,
                Message = "No calendars found."
            });
        }
        #endregion

        [HttpPost("CreateCalendar")]
        public async Task<IActionResult> CreateCalendarAsync()
        {
            try
            {
                var calendarId = await _teachingScheduleService.CreateCalendarAsync();
                return Ok(new
                {
                    Success = true,
                    Message = "Google Calendar created successfully.",
                    CalendarId = calendarId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "An error occurred while creating the calendar.",
                    Error = ex.Message
                });
            }
        }
    }
}
