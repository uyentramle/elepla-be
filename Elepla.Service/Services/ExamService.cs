using AutoMapper;
using Elepla.Domain.Entities;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.ExamViewModels;
using PdfDocument = iText.Kernel.Pdf.PdfDocument;
using PdfWriter = iText.Kernel.Pdf.PdfWriter;
using Document = iText.Layout.Document;
using Paragraph = iText.Layout.Element.Paragraph;
using OpenXmlWord = DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.IO.Font;

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

        public async Task<ResponseModel> ExportExamToWordAsync(string examId)
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

            using (var memoryStream = new MemoryStream())
            {
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document))
                {
                    var mainPart = wordDoc.AddMainDocumentPart();
                    mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
                    var body = mainPart.Document.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Body());

                    // Add title and time
                    var titleParagraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                        new DocumentFormat.OpenXml.Wordprocessing.Run(
                            new DocumentFormat.OpenXml.Wordprocessing.Text($"Bài thi: {exam.Title}")
                        )
                        {
                            RunProperties = new DocumentFormat.OpenXml.Wordprocessing.RunProperties { Bold = new DocumentFormat.OpenXml.Wordprocessing.Bold() }
                        }
                    );
                    body.AppendChild(titleParagraph);

                    var timeParagraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                        new DocumentFormat.OpenXml.Wordprocessing.Run(
                            new DocumentFormat.OpenXml.Wordprocessing.Text($"Thời gian: {exam.Time} phút")
                        )
                    );
                    body.AppendChild(timeParagraph);
                    body.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text("")))); // Empty line

                    // Add questions and answers
                    foreach (var questionInExam in exam.QuestionInExams)
                    {
                        var question = questionInExam.Question;
                        var questionParagraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                            new DocumentFormat.OpenXml.Wordprocessing.Run(
                                new DocumentFormat.OpenXml.Wordprocessing.Text($"{question.Question}")
                            )
                            {
                                RunProperties = new DocumentFormat.OpenXml.Wordprocessing.RunProperties { Bold = new DocumentFormat.OpenXml.Wordprocessing.Bold() }
                            }
                        );
                        body.AppendChild(questionParagraph);

                        // Label answers with letters A, B, C, D
                        var answerLabels = new[] { "A", "B", "C", "D" };
                        for (int j = 0; j < question.Answers.Count && j < answerLabels.Length; j++)
                        {
                            var answer = question.Answers.ElementAt(j);
                            var answerText = $"{answerLabels[j]}. {answer.AnswerText}";

                            // Create RunProperties for each answer
                            var answerRunProperties = new DocumentFormat.OpenXml.Wordprocessing.RunProperties();
                            if (string.Equals(answer.IsCorrect, "true", StringComparison.OrdinalIgnoreCase))
                            {
                                // Set color to red for correct answers
                                answerRunProperties.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Color { Val = "FF0000" });
                            }

                            var answerRun = new DocumentFormat.OpenXml.Wordprocessing.Run
                            {
                                RunProperties = answerRunProperties
                            };
                            answerRun.Append(new DocumentFormat.OpenXml.Wordprocessing.Text(answerText));

                            var answerParagraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(answerRun);
                            body.AppendChild(answerParagraph);
                        }

                        // Add empty line between questions
                        body.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text(""))));
                    }
                }

                // Return the Word document as a byte array
                return new SuccessResponseModel<byte[]>
                {
                    Success = true,
                    Message = "Exam exported to Word successfully.",
                    Data = memoryStream.ToArray() // Convert memory stream to byte array
                };
            }
        }


        public async Task<ResponseModel> ExportExamToPdfAsync(string examId)
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

            // Path to your DejaVuSans.ttf font file
            var fontPath = Path.Combine("font", "DejaVuSans.ttf");

            // Using a memory stream instead of saving the file on disk
            using (var memoryStream = new MemoryStream())
            {
                using (var pdfWriter = new PdfWriter(memoryStream))
                {
                    using (var pdfDoc = new PdfDocument(pdfWriter))
                    {
                        var document = new Document(pdfDoc);

                        // Load DejaVuSans font that supports Vietnamese characters
                        var vietnameseFont = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);
                        document.SetFont(vietnameseFont);

                        // Add title and time
                        document.Add(new Paragraph($"Bài thi: {exam.Title}")
                            .SetFontSize(18)
                            .SetBold()
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));
                        document.Add(new Paragraph($"Thời gian: {exam.Time} phút").SetFontSize(14));
                        document.Add(new Paragraph("\n"));

                        // Add questions and answers
                        foreach (var questionInExam in exam.QuestionInExams)
                        {
                            var question = questionInExam.Question;
                            document.Add(new Paragraph($"{question.Question}")
                                .SetFontSize(14)
                                .SetBold());

                            // Label answers with letters A, B, C, D
                            var answerLabels = new[] { "A", "B", "C", "D" };
                            for (int j = 0; j < question.Answers.Count && j < answerLabels.Length; j++)
                            {
                                var answer = question.Answers.ElementAt(j);
                                var paragraph = new Paragraph($"{answerLabels[j]}. {answer.AnswerText}").SetFontSize(12);

                                // Set text color to red if answer is correct
                                if (string.Equals(answer.IsCorrect, "true", StringComparison.OrdinalIgnoreCase))
                                {
                                    paragraph.SetFontColor(iText.Kernel.Colors.ColorConstants.RED);
                                }

                                document.Add(paragraph);
                            }

                            document.Add(new Paragraph("\n")); // Empty line between questions
                        }
                    }
                }

                // Return the PDF as a byte array
                return new SuccessResponseModel<byte[]>
                {
                    Success = true,
                    Message = "Exam exported to PDF successfully.",
                    Data = memoryStream.ToArray() // Convert memory stream to byte array
                };
            }
        }

    }
}
