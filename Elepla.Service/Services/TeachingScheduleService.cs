using AutoMapper;
using DocumentFormat.OpenXml.Drawing;
using Elepla.Domain.Entities;
using Elepla.Repository.Common;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.TeachingScheduleModels;
using Elepla.Service.Utils;
using Google.Apis.Calendar.v3.Data;
using iText.Kernel.Pdf.Canvas.Parser.ClipperLib;
using System;
using System.Globalization;


namespace Elepla.Service.Services
{
    public class TeachingScheduleService : ITeachingScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IGoogleCalendarService _googleCalendarService;

        public TeachingScheduleService(IUnitOfWork unitOfWork, IMapper mapper, IGoogleCalendarService googleCalendarService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _googleCalendarService = googleCalendarService;
        }

        public async Task<ResponseModel> GetAllTeachingSchedulesAsync(string? keyword, int pageIndex, int pageSize)
        {
            var schedules = await _unitOfWork.TeachingScheduleRepository.GetAsync(
                                filter: s => !s.IsDeleted && (string.IsNullOrEmpty(keyword) || s.ClassName.Contains(keyword)),
                                orderBy: s => s.OrderBy(s => s.Date),
                                includeProperties: "Teacher,Planbook",
                                pageIndex: pageIndex,
                                pageSize: pageSize
            );

            var scheduleDtos = _mapper.Map<Pagination<ViewTeachingScheduleDTO>>(schedules);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Teaching schedules retrieved successfully.",
                Data = scheduleDtos
            };
        }

        public async Task<ResponseModel> GetTeachingScheduleByIdAsync(string scheduleId)
        {
            var schedule = await _unitOfWork.TeachingScheduleRepository.GetByIdAsync(scheduleId, includeProperties: "Teacher,Planbook");

            if (schedule == null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "Teaching schedule not found."
                };
            }

            var result = _mapper.Map<ViewTeachingScheduleDTO>(schedule);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Teaching schedule retrieved successfully.",
                Data = result
            };
        }

        public async Task<ResponseModel> GetTeachingSchedulesByUserIdAsync(string userId, int pageIndex, int pageSize)
        {
            var schedules = await _unitOfWork.TeachingScheduleRepository.GetAsync(
                                filter: s => s.TeacherId == userId && !s.IsDeleted,
                                orderBy: s => s.OrderBy(s => s.Date),
                                includeProperties: "Teacher,Planbook",
                                pageIndex: pageIndex,
                                pageSize: pageSize
            );

            var scheduleDtos = _mapper.Map<Pagination<ViewTeachingScheduleDTO>>(schedules);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Teaching schedules for the user retrieved successfully.",
                Data = scheduleDtos
            };
        }


        public async Task<ResponseModel> AddTeachingScheduleAsync(CreateTeachingScheduleDTO model)
        {
            try
            {
                // Map the model to the TeachingSchedule entity
                var schedule = _mapper.Map<TeachingSchedule>(model);
                schedule.ScheduleId = Guid.NewGuid().ToString();

                await _unitOfWork.TeachingScheduleRepository.AddAsync(schedule);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Teaching schedule created successfully.",
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while creating the teaching schedule.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseModel> UpdateTeachingScheduleAsync(UpdateTeachingScheduleDTO model)
        {
            try
            {
                var schedule = await _unitOfWork.TeachingScheduleRepository.GetByIdAsync(model.ScheduleId);
                if (schedule == null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Teaching schedule not found."
                    };
                }

                if (schedule.IsDeleted == true)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Cannot modify a deleted teaching schedule."
                    };
                }

                // Map the updated model to the schedule entity
                _mapper.Map(model, schedule);

                _unitOfWork.TeachingScheduleRepository.Update(schedule);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Teaching schedule updated successfully.",
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while updating the teaching schedule.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseModel> DeleteTeachingScheduleAsync(string scheduleId)
        {
            try
            {
                // Fetch the teaching schedule by ID
                var schedule = await _unitOfWork.TeachingScheduleRepository.GetByIdAsync(scheduleId);

                // Check if the schedule exists
                if (schedule == null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Teaching schedule not found."
                    };
                }

                _unitOfWork.TeachingScheduleRepository.Delete(schedule);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Teaching schedule deleted permanently."
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the teaching schedule.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseModel> GetGoogleAuthUrlAsync(string scheduleId)
        {
            try
            {
                // Lấy thông tin lịch từ database
                var schedule = await _unitOfWork.TeachingScheduleRepository.GetByIdAsync(scheduleId);
                if (schedule == null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Teaching schedule not found."
                    };
                }

                // Tạo URL xác thực Google
                string authUrl = _googleCalendarService.GenerateAuthorizationUrl(scheduleId);

                return new SuccessResponseModel<object>
                {
                    Success = true,
                    Message = "Authorization URL generated successfully.",
                    Data = authUrl
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while generating the authorization URL.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseModel> AddTeachingScheduleToGoogleCalendarAsync(string scheduleId, string authorizationCode)
        {
            try
            {
                // Lấy thông tin lịch từ database
                var schedule = await _unitOfWork.TeachingScheduleRepository.GetByIdAsync(scheduleId, includeProperties: "Teacher,Planbook");
                if (schedule == null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Teaching schedule not found."
                    };
                }

                // Lấy thông tin giáo viên từ UserId trong schedule
                var teacher = schedule.Teacher;
                if (teacher == null || string.IsNullOrEmpty(teacher.GoogleEmail))
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Teacher does not have a linked Google account. Please connect a Google account before adding to calendar."
                    };
                }

                // Parse StartTime và EndTime
                if (!DateTime.TryParse(schedule.StartTime, out DateTime startTime) ||
                    !DateTime.TryParse(schedule.EndTime, out DateTime endTime))
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Invalid time format for StartTime or EndTime."
                    };
                }

                // Combine Date with StartTime and EndTime (Assume schedule.Date is in local time)
                DateTime startLocalTime = schedule.Date.Add(startTime.TimeOfDay);
                DateTime endLocalTime = schedule.Date.Add(endTime.TimeOfDay);

                // Convert local times to UTC for Google Calendar
                TimeZoneInfo localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
                DateTime startUtc = TimeZoneInfo.ConvertTimeToUtc(startLocalTime, localTimeZone);
                DateTime endUtc = TimeZoneInfo.ConvertTimeToUtc(endLocalTime, localTimeZone);

                // Map teaching schedule to Google Calendar event
                var calendarEvent = new Event
                {
                    Summary = schedule.Title,
                    Description = schedule.Description,
                    Start = new EventDateTime
                    {
                        DateTime = startUtc,
                        TimeZone = "Asia/Ho_Chi_Minh" // Ensure the correct time zone is sent
                    },
                    End = new EventDateTime
                    {
                        DateTime = endUtc,
                        TimeZone = "Asia/Ho_Chi_Minh"
                    },
                    Location = schedule.ClassName
                };

                // Thêm sự kiện vào Google Calendar sau xác thực
                var createdEvent = await _googleCalendarService.AddEventToCalendarAfterAuthorizationAsync(authorizationCode, calendarEvent);

                return new SuccessResponseModel<object>
                {
                    Success = true,
                    Message = "Teaching schedule successfully added to Google Calendar.",
                    Data = createdEvent.HtmlLink
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while adding the teaching schedule to Google Calendar.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}