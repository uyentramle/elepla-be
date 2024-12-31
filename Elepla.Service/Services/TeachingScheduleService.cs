using AutoMapper;
using DocumentFormat.OpenXml.Drawing;
using Elepla.Domain.Entities;
using Elepla.Repository.Common;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.TeachingScheduleModels;
using Elepla.Service.Utils;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using iText.Kernel.Pdf.Canvas.Parser.ClipperLib;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Globalization;
using System.Net;


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

        public async Task<ResponseModel> GetGoogleAuthUrlsAsync(List<string> scheduleIds)
        {
            try
            {
                if (scheduleIds == null || scheduleIds.Count == 0)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "No schedule IDs provided."
                    };
                }

                // Gọi GoogleCalendarService để tạo URL xác thực duy nhất
                string authUrl = _googleCalendarService.GenerateAuthorizationUrl(scheduleIds);

                return new SuccessResponseModel<string>
                {
                    Success = true,
                    Message = "Authorization URL generated successfully.",
                    Data = authUrl // Trả về một URL duy nhất
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



        public async Task<ResponseModel> AddMultipleTeachingSchedulesToGoogleCalendarAsync(List<string> scheduleIds, string authorizationCode)
        {
            var results = new List<string>();
            var errors = new List<string>();

            // Lấy Refresh Token (nếu chưa lưu, sử dụng Authorization Code)
            string refreshToken;
            try
            {
                refreshToken = await _googleCalendarService.GetRefreshTokenFromAuthorizationCodeAsync(authorizationCode);
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "Could not retrieve Refresh Token.",
                    Errors = new List<string> { ex.Message }
                };
            }

            foreach (var scheduleId in scheduleIds)
            {
                try
                {
                    var schedule = await _unitOfWork.TeachingScheduleRepository.GetByIdAsync(scheduleId, includeProperties: "Teacher,Planbook");
                    if (schedule == null)
                    {
                        errors.Add($"Schedule ID {scheduleId} not found.");
                        continue;
                    }

                    var teacher = schedule.Teacher;
                    if (teacher == null || string.IsNullOrEmpty(teacher.GoogleEmail))
                    {
                        errors.Add($"Teacher for schedule ID {scheduleId} does not have a linked Google account.");
                        continue;
                    }

                    if (!DateTime.TryParse(schedule.StartTime, out DateTime startTime) || !DateTime.TryParse(schedule.EndTime, out DateTime endTime))
                    {
                        errors.Add($"Invalid time format for schedule ID {scheduleId}.");
                        continue;
                    }

                    DateTime startLocalTime = schedule.Date.Add(startTime.TimeOfDay);
                    DateTime endLocalTime = schedule.Date.Add(endTime.TimeOfDay);

                    TimeZoneInfo localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
                    DateTime startUtc = TimeZoneInfo.ConvertTimeToUtc(startLocalTime, localTimeZone);
                    DateTime endUtc = TimeZoneInfo.ConvertTimeToUtc(endLocalTime, localTimeZone);

                    var calendarEvent = new Event
                    {
                        Summary = schedule.Title,
                        Description = schedule.Description,
                        Start = new EventDateTime
                        {
                            DateTime = startUtc,
                            TimeZone = "Asia/Ho_Chi_Minh"
                        },
                        End = new EventDateTime
                        {
                            DateTime = endUtc,
                            TimeZone = "Asia/Ho_Chi_Minh"
                        },
                        Location = schedule.ClassName
                    };

                    // Lấy Access Token từ Refresh Token
                    string accessToken = await _googleCalendarService.GetAccessTokenFromRefreshTokenAsync(refreshToken);

                    // Tạo sự kiện trên Google Calendar
                    var calendarService = new CalendarService(new BaseClientService.Initializer
                    {
                        HttpClientInitializer = GoogleCredential.FromAccessToken(accessToken),
                        ApplicationName = "Elepla"
                    });

                    var insertRequest = calendarService.Events.Insert(calendarEvent, "primary");
                    var createdEvent = await insertRequest.ExecuteAsync();

                    results.Add($"Schedule ID {scheduleId} added successfully: {createdEvent.HtmlLink}");
                }
                catch (Exception ex)
                {
                    errors.Add($"Error for schedule ID {scheduleId}: {ex.Message}");
                }
            }

            if (errors.Count > 0)
            {
                return new ErrorResponseModel<List<string>>
                {
                    Success = false,
                    Message = "Some schedules could not be added.",
                    Errors = errors
                };
            }

            return new SuccessResponseModel<List<string>>
            {
                Success = true,
                Message = "All schedules added successfully.",
                Data = results
            };
        }

    }
}