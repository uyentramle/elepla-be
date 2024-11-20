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
using Elepla.Service.Models.ViewModels.AnswerViewModels;

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
                var exams = await _unitOfWork.ExamRepository.GetAllAsync(filter: e => e.UserId.Equals(userId));
                var examDtos = _mapper.Map<List<ViewListExamDTO>>(exams);

                return new SuccessResponseModel<object>
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
            try
            {
                var exam = await _unitOfWork.ExamRepository.GetByIdAsync(id: examId, includeProperties: "QuestionInExams.Question.Answers");
                if (exam is null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Exam not found."
                    };
                }

                var examDto = _mapper.Map<ViewDetailExamDTO>(exam);

                //examDto.Questions = exam.QuestionInExams
                //    .OrderBy(questionId => questionId.Index)
                //    .Select(questionId => new QuestionInExamDTO
                //    {
                //        QuestionId = questionId.QuestionId,
                //        Question = questionId.Question.Question,
                //        Type = questionId.Question.Type,
                //        Plum = questionId.Question.Plum,
                //        Index = questionId.Index.ToString(),
                //        Answers = questionId.Question.Answers.Select(a => new ViewListAnswerDTO
                //        {
                //            AnswerId = a.AnswerId,
                //            AnswerText = a.AnswerText,
                //            IsCorrect = a.IsCorrect
                //        }).ToList()
                //    })
                //    .ToList();

                return new SuccessResponseModel<object>
                {
                    Success = true,
                    Message = "Exam retrieved successfully.",
                    Data = examDto
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the exam.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseModel> CreateExamAsync(CreateExamDTO model)
        {
            try
            {
                var exam = _mapper.Map<Exam>(model);

                await _unitOfWork.ExamRepository.AddAsync(exam);

                var questionInExamList = model.QuestionIds.Select((questionId, index) => new QuestionInExam
                {
                    QuestionInExamId = Guid.NewGuid().ToString(),
                    ExamId = exam.ExamId,
                    QuestionId = questionId,
                    Index = index + 1
                });

                await _unitOfWork.QuestionInExamRepository.CreateRangeQuestionInExamAsync(questionInExamList);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Exam created successfully.",
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

                // Cập nhật thông tin bài kiểm tra
                _mapper.Map(model, exam);
                _unitOfWork.ExamRepository.Update(exam);

                // Cập nhật câu hỏi trong bài kiểm tra sẽ có 2 trường hợp:
                // 1. Thêm câu hỏi mới vào bài kiểm tra
                // 2. Cập nhật lại câu hỏi đã có trong bài kiểm tra tức là xóa câu hỏi
                // => Xóa tất cả câu hỏi trong bài kiểm tra và thêm lại câu hỏi cũ và mới

                // Tạo 1 list chứa các câu hỏi sẽ thêm vào bài kiểm tra từ model
                var questionInExamList = model.QuestionIds.Select((questionId, index) => new QuestionInExam
                {
                    QuestionInExamId = Guid.NewGuid().ToString(),
                    ExamId = exam.ExamId,
                    QuestionId = questionId,
                    Index = index + 1
                });

                var questionsInExam = await _unitOfWork.QuestionInExamRepository.GetByExamIdAsync(model.ExamId);
                _unitOfWork.QuestionInExamRepository.DeleteRangeQuestionInExamAsync(questionsInExam);
                await _unitOfWork.QuestionInExamRepository.CreateRangeQuestionInExamAsync(questionInExamList);
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
                var questionsInExam = await _unitOfWork.QuestionInExamRepository.GetByExamIdAsync(examId);
                _unitOfWork.QuestionInExamRepository.DeleteRangeQuestionInExamAsync(questionsInExam);

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
                            new DocumentFormat.OpenXml.Wordprocessing.Text($"Thời gian: {exam.Time}")
                        )
                    );
                    body.AppendChild(timeParagraph);
                    body.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text("")))); // Empty line

                    // Add questions and answers with numbering
                    int questionNumber = 1;
                    foreach (var questionInExam in exam.QuestionInExams.OrderBy(q => q.Index))
                    {
                        var question = questionInExam.Question;

                        // Add question number and text
                        var questionParagraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                            new DocumentFormat.OpenXml.Wordprocessing.Run(
                                new DocumentFormat.OpenXml.Wordprocessing.Text($"Câu {questionNumber}. {question.Question}")
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

                        questionNumber++;
                        body.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text("")))); // Empty line between questions
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
            var fontPath = Path.Combine(AppContext.BaseDirectory, "Resources", "Fonts", "DejaVuSans.ttf");

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
                        document.Add(new Paragraph($"Thời gian: {exam.Time}").SetFontSize(14));
                        document.Add(new Paragraph("\n"));

                        // Add questions and answers with numbering
                        int questionNumber = 1;
                        foreach (var questionInExam in exam.QuestionInExams.OrderBy(q => q.Index))
                        {
                            var question = questionInExam.Question;

                            // Add question number and text
                            document.Add(new Paragraph($"Câu {questionNumber}. {question.Question}")
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

                            questionNumber++;
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

        public async Task<ResponseModel> ExportExamToWordNoColorAsync(string examId)
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
                            new DocumentFormat.OpenXml.Wordprocessing.Text($"Thời gian: {exam.Time}")
                        )
                    );
                    body.AppendChild(timeParagraph);
                    body.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text("")))); // Empty line

                    // Add questions and answers with numbering
                    int questionNumber = 1;
                    foreach (var questionInExam in exam.QuestionInExams.OrderBy(q => q.Index))
                    {
                        var question = questionInExam.Question;

                        // Add question number and text
                        var questionParagraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                            new DocumentFormat.OpenXml.Wordprocessing.Run(
                                new DocumentFormat.OpenXml.Wordprocessing.Text($"Câu {questionNumber}. {question.Question}")
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

                            // No color styling applied here for correct answers
                            var answerRun = new DocumentFormat.OpenXml.Wordprocessing.Run(
                                new DocumentFormat.OpenXml.Wordprocessing.Text(answerText)
                            );

                            var answerParagraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(answerRun);
                            body.AppendChild(answerParagraph);
                        }

                        questionNumber++;
                        body.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text("")))); // Empty line between questions
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

        public async Task<ResponseModel> ExportExamToPdfNoColorAsync(string examId)
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
            var fontPath = Path.Combine(AppContext.BaseDirectory, "Resources", "Fonts", "DejaVuSans.ttf");

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
                        document.Add(new Paragraph($"Thời gian: {exam.Time}").SetFontSize(14));
                        document.Add(new Paragraph("\n"));

                        // Add questions and answers with numbering
                        int questionNumber = 1;
                        foreach (var questionInExam in exam.QuestionInExams.OrderBy(q => q.Index))
                        {
                            var question = questionInExam.Question;

                            // Add question number and text
                            document.Add(new Paragraph($"Câu {questionNumber}. {question.Question}")
                                .SetFontSize(14)
                                .SetBold());

                            // Label answers with letters A, B, C, D
                            var answerLabels = new[] { "A", "B", "C", "D" };
                            for (int j = 0; j < question.Answers.Count && j < answerLabels.Length; j++)
                            {
                                var answer = question.Answers.ElementAt(j);
                                var paragraph = new Paragraph($"{answerLabels[j]}. {answer.AnswerText}").SetFontSize(12);

                                // No color styling applied here for correct answers
                                document.Add(paragraph);
                            }

                            questionNumber++;
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
