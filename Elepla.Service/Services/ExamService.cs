using AutoMapper;
using Elepla.Domain.Entities;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.ExamViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Services
{
    public class ExamService : IExamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExamService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ResponseModel> GetExamsByUserIdAsync(string userId)
        {
            try
            {
                var paginationResult = await _unitOfWork.ExamRepository.GetAsync(
                    filter: e => e.UserId == userId,
                    includeProperties: "QuestionInExams.Question.Answers"
                );
                var exams = paginationResult.Items;

                if (!exams.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "No exams found for the specified user."
                    };
                }
                var examDtos = exams.Select(exam => new ViewExamDTO
                {
                    ExamId = exam.ExamId,
                    Title = exam.Title,
                    Time = exam.Time,
                    UserId = exam.UserId,
                    Questions = exam.QuestionInExams.Select(q => new QuestionDetailDTO
                    {
                        QuestionId = q.Question.QuestionId,
                        Question = q.Question.Question,
                        Type = q.Question.Type,
                        Answers = q.Question.Answers.Select(a => new AnswerDTO
                        {
                            AnswerId = a.AnswerId,
                            AnswerText = a.AnswerText,
                            IsCorrect = a.IsCorrect
                        }).ToList()
                    }).ToList()
                }).ToList();

                return new SuccessResponseModel<List<ViewExamDTO>>
                {
                    Success = true,
                    Message = "Exams retrieved successfully.",
                    Data = examDtos
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving exams.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }



        public async Task<ResponseModel> GetExamByIdAsync(string examId)
        {
            var exam = await _unitOfWork.ExamRepository.GetByIdAsync(examId, includeProperties: "QuestionInExams.Question.Answers");
            if (exam == null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "Exam not found."
                };
            }

            var examDto = new ViewExamDTO
            {
                ExamId = exam.ExamId,
                Title = exam.Title,
                Time = exam.Time,
                UserId = exam.UserId,
                Questions = exam.QuestionInExams.Select(q => new QuestionDetailDTO
                {
                    QuestionId = q.Question.QuestionId,
                    Question = q.Question.Question,
                    Type = q.Question.Type,
                    Answers = q.Question.Answers.Select(a => new AnswerDTO
                    {
                        AnswerId = a.AnswerId,
                        AnswerText = a.AnswerText,
                        IsCorrect = a.IsCorrect
                    }).ToList()
                }).ToList()
            };

            return new SuccessResponseModel<ViewExamDTO>
            {
                Success = true,
                Message = "Exam retrieved successfully.",
                Data = examDto
            };
        }


        public async Task<ResponseModel> CreateExamAsync(CreateExamDTO model)
        {
            try
            {
                var exam = new Exam
                {
                    ExamId = Guid.NewGuid().ToString(),
                    Title = model.Title,
                    Time = model.Time,
                    UserId = model.UserId
                };

                await _unitOfWork.ExamRepository.AddAsync(exam);

                // Add questions to the exam
                foreach (var questionId in model.QuestionIds)
                {
                    var questionInExam = new QuestionInExam
                    {
                        QuestionInExamId = Guid.NewGuid().ToString(),
                        ExamId = exam.ExamId,
                        QuestionId = questionId
                    };
                    await _unitOfWork.QuestionInExamRepository.AddAsync(questionInExam);
                }

                await _unitOfWork.SaveChangeAsync();

                return new SuccessResponseModel<string>
                {
                    Success = true,
                    Message = "Exam created successfully.",
                    Data = exam.ExamId
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while creating the exam.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseModel> UpdateExamAsync(UpdateExamDTO model)
        {
            try
            {
                var exam = await _unitOfWork.ExamRepository.GetByIdAsync(model.ExamId);
                if (exam == null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Exam not found."
                    };
                }

                exam.Title = model.Title ?? exam.Title;
                exam.Time = model.Time ?? exam.Time;

                // Validate that all QuestionIds exist in the QuestionBank
                var validQuestionIds = (await _unitOfWork.QuestionBankRepository
                    .GetAllAsync(q => model.QuestionIds.Contains(q.QuestionId)))
                    .Select(q => q.QuestionId)
                    .ToList();

                var invalidQuestionIds = model.QuestionIds.Except(validQuestionIds).ToList();
                if (invalidQuestionIds.Any())
                {
                    return new ErrorResponseModel<List<string>>
                    {
                        Success = false,
                        Message = "Some question IDs do not exist in the QuestionBank.",
                        Errors = invalidQuestionIds
                    };
                }

                // Update questions
                var existingQuestions = await _unitOfWork.QuestionInExamRepository.GetAsync(q => q.ExamId == exam.ExamId);
                _unitOfWork.QuestionInExamRepository.DeleteRange(existingQuestions);

                foreach (var questionId in model.QuestionIds)
                {
                    var questionInExam = new QuestionInExam
                    {
                        QuestionInExamId = Guid.NewGuid().ToString(),
                        ExamId = exam.ExamId,
                        QuestionId = questionId
                    };
                    await _unitOfWork.QuestionInExamRepository.AddAsync(questionInExam);
                }

                _unitOfWork.ExamRepository.Update(exam);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Exam updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while updating the exam.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseModel> DeleteExamAsync(string examId)
        {
            try
            {
                var exam = await _unitOfWork.ExamRepository.GetByIdAsync(examId);
                if (exam == null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Exam not found."
                    };
                }

                // Delete related questions
                var questionsInExam = await _unitOfWork.QuestionInExamRepository.GetAsync(q => q.ExamId == examId);
                _unitOfWork.QuestionInExamRepository.DeleteRange(questionsInExam);

                // Delete exam
                _unitOfWork.ExamRepository.Delete(exam);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Exam deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the exam.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

    }
}
