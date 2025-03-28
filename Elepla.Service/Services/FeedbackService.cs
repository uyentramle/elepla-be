﻿using AutoMapper;
using Elepla.Domain.Entities;
using Elepla.Repository.Common;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.FeedbackViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FeedbackService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Get feedback for a specific Planbook
        public async Task<ResponseModel> GetFeedbackByPlanbookIdAsync(string planbookId, int pageIndex, int pageSize)
        {
            var feedbacks = await _unitOfWork.FeedbackRepository.GetAsync(
                filter: f => f.PlanbookId == planbookId && !f.IsDeleted,
                orderBy: f => f.OrderByDescending(f => f.CreatedAt),
                includeProperties: "Teacher.Avatar,Planbook",
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var feedbackDtos = _mapper.Map<Pagination<ViewFeedbackDTO>>(feedbacks);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Feedback retrieved successfully.",
                Data = feedbackDtos
            };
        }

        // Submit new feedback for a Planbook
        public async Task<ResponseModel> SubmitFeedbackAsync(CreateFeedbackDTO model)
        {
            try
            {
                var feedback = _mapper.Map<Feedback>(model);
                feedback.FeedbackId = Guid.NewGuid().ToString();

                await _unitOfWork.FeedbackRepository.AddAsync(feedback);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Feedback submitted successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while submitting feedback.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        // Update existing feedback
        public async Task<ResponseModel> UpdateFeedbackAsync(UpdateFeedbackDTO model)
        {
            try
            {
                var feedback = await _unitOfWork.FeedbackRepository.GetByIdAsync(model.FeedbackId);
                if (feedback == null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Feedback not found."
                    };
                }

                _mapper.Map(model, feedback);
                _unitOfWork.FeedbackRepository.Update(feedback);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Feedback updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while updating feedback.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        // Hard delete feedback by Id
        public async Task<ResponseModel> HardDeleteFeedbackAsync(string feedbackId)
        {
            try
            {
                var feedback = await _unitOfWork.FeedbackRepository.GetByIdAsync(feedbackId);
                if (feedback == null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Feedback not found."
                    };
                }

                _unitOfWork.FeedbackRepository.Delete(feedback);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Feedback deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting feedback.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        // Flag a feedback
        public async Task<ResponseModel> FlagFeedbackAsync(string feedbackId)
        {
            var feedback = await _unitOfWork.FeedbackRepository.GetByIdAsync(feedbackId);
            if (feedback == null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "Feedback not found."
                };
            }

            // Toggle the flag status
            feedback.IsFlagged = !feedback.IsFlagged;
            _unitOfWork.FeedbackRepository.Update(feedback);
            await _unitOfWork.SaveChangeAsync();

            return new ResponseModel
            {
                Success = true,
                Message = feedback.IsFlagged ? "Feedback flagged successfully." : "Feedback unflagged successfully."
            };
        }

        // Get all flagged feedbacks
        public async Task<ResponseModel> GetFlaggedFeedbackAsync(int pageIndex, int pageSize)
        {
            var flaggedFeedbacks = await _unitOfWork.FeedbackRepository.GetAsync(
                filter: f => f.IsFlagged && !f.IsDeleted,
                includeProperties: "Teacher.Avatar,Planbook",
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var feedbackDtos = _mapper.Map<Pagination<ViewFeedbackDTO>>(flaggedFeedbacks);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Flagged feedback retrieved successfully.",
                Data = feedbackDtos
            };
        }

		// Get all system feedbacks
		public async Task<ResponseModel> GetSystemFeedbackAsync(int pageIndex, int pageSize)
		{
			var systemFeedbacks = await _unitOfWork.FeedbackRepository.GetAsync(
						            filter: f => f.Type == "System" && !f.IsDeleted,
									includeProperties: "Teacher.Avatar,Planbook",
									pageIndex: pageIndex,
									pageSize: pageSize
									);

			var feedbackDtos = _mapper.Map<Pagination<ViewFeedbackDTO>>(systemFeedbacks);

			return new SuccessResponseModel<object>
			{
				Success = true,
				Message = "System feedback retrieved successfully.",
				Data = feedbackDtos
			};
		}

		// Get all planbook feedbacks
		public async Task<ResponseModel> GetPlanbookFeedbackAsync(int pageIndex, int pageSize)
		{
			var planbookFeedbacks = await _unitOfWork.FeedbackRepository.GetAsync(
									filter: f => f.Type == "Planbook" && !f.IsDeleted,
				                    includeProperties: "Teacher.Avatar,Planbook",
									pageIndex: pageIndex,
									pageSize: pageSize
                                    );

			var feedbackDtos = _mapper.Map<Pagination<ViewFeedbackDTO>>(planbookFeedbacks);

			return new SuccessResponseModel<object>
			{
				Success = true,
				Message = "Planbook feedback retrieved successfully.",
				Data = feedbackDtos
			};
		}

        // Flag a feedback
        public async Task<ResponseModel> FlagCommentAsync(string feedbackId)
        {
            try
            {
                var feedback = await _unitOfWork.FeedbackRepository.GetByIdAsync(feedbackId);
                if (feedback is null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Feedback not found."
                    };
                }

                feedback.FlagCount++;

                _unitOfWork.FeedbackRepository.Update(feedback);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Feedback flagged successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while flagging feedback.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}
