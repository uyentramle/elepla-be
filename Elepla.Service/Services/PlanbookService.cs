using AutoMapper;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using Elepla.Repository.Common;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.ActivityViewModels;
using Elepla.Service.Models.ViewModels.PlanbookViewModels;
using DocumentFormat.OpenXml.Wordprocessing;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;

namespace Elepla.Service.Services
{
    public class PlanbookService : IPlanbookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PlanbookService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

		#region Get All Planbooks
        public async Task<ResponseModel> GetAllPlanbooksAsync(int pageIndex, int pageSize)
		{
			var planbooks = await _unitOfWork.PlanbookRepository.GetAsync(
							filter: r => r.IsDeleted == false,
							pageIndex: pageIndex,
							pageSize: pageSize
							);
			var mappers = _mapper.Map<Pagination<ViewListPlanbookDTO>>(planbooks);
			foreach (var item in mappers.Items)
			{
				var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(item.LessonId);
				if (lesson != null)
				{
					item.LessonName = lesson.Name;
				}
			}

			return new SuccessResponseModel<object>
			{
				Success = true,
				Message = "Planbooks retrieved successfully.",
				Data = mappers
			};
		}
		#endregion

		#region Get Planbook By Id
		public async Task<ResponseModel> GetPlanbookByIdAsync(string planbookId)
		{
			var planbook = await _unitOfWork.PlanbookRepository.GetByIdAsync(planbookId);
			if (planbook == null)
			{
				return new ResponseModel
				{
					Success = false,
					Message = "Planbook not found."
				};
			}

			var mapper = _mapper.Map<ViewDetailPlanbookDTO>(planbook);

			var collection = await _unitOfWork.PlanbookCollectionRepository.GetByIdAsync(planbook.CollectionId);
			if (collection != null)
			{
				mapper.CollectionName = collection.CollectionName;
			}

			var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(planbook.LessonId);
			if (lesson != null)
			{
				mapper.LessonName = lesson.Name;
			}

			var activities = await _unitOfWork.ActivityRepository.GetByPlanbookIdAsync(planbookId);
			mapper.Activities = _mapper.Map<List<ViewListActivityDTO>>(activities);

			return new SuccessResponseModel<object>
			{
				Success = true,
				Message = "Planbook retrieved successfully.",
				Data = mapper
			};
		}
        #endregion

        #region Export Planbook to Word
        public async Task<ResponseModel> ExportPlanbookToWordAsync(string planbookId)
        {
            try
            {
                var planbook = await _unitOfWork.PlanbookRepository.GetByIdAsync(planbookId, includeProperties: "Activities,Feedbacks,TeachingSchedules");

                if (planbook == null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Planbook not found."
                    };
                }

                using (var memoryStream = new MemoryStream())
                {
                    using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document, true))
                    {
                        var mainPart = wordDocument.AddMainDocumentPart();
                        mainPart.Document = new Document();
                        var body = mainPart.Document.AppendChild(new Body());

                        // Add title
                        var titleParagraph = new Paragraph(new Run(new Text("Kế Hoạch Giảng Dạy")))
                        {
                            ParagraphProperties = new ParagraphProperties
                            {
                                Justification = new Justification { Val = JustificationValues.Center }
                            }
                        };
                        titleParagraph.ParagraphProperties.AppendChild(new RunProperties(new Bold(), new FontSize { Val = "28" }));
                        body.AppendChild(titleParagraph);

                        // Add a blank line
                        body.AppendChild(new Paragraph(new Run(new Text("\n"))));

                        // Add planbook details
                        body.AppendChild(CreateParagraph($"Tiêu Đề: {planbook.Title}"));
                        body.AppendChild(CreateParagraph($"Tên Trường: {planbook.SchoolName}"));
                        body.AppendChild(CreateParagraph($"Giáo Viên: {planbook.TeacherName}"));
                        body.AppendChild(CreateParagraph($"Môn Học: {planbook.Subject}"));
                        body.AppendChild(CreateParagraph($"Lớp Học: {planbook.ClassName}"));
                        body.AppendChild(CreateParagraph($"Số Tiết: {planbook.DurationInPeriods}"));
                        body.AppendChild(CreateParagraph($"Mục Tiêu Kiến Thức: {planbook.KnowledgeObjective}"));
                        body.AppendChild(CreateParagraph($"Mục Tiêu Kỹ Năng: {planbook.SkillsObjective}"));
                        body.AppendChild(CreateParagraph($"Mục Tiêu Phẩm Chất: {planbook.QualitiesObjective}"));
                        body.AppendChild(CreateParagraph($"Công Cụ Giảng Dạy: {planbook.TeachingTools}"));
                        body.AppendChild(CreateParagraph($"Ghi Chú: {planbook.Notes}"));

                        // Add teaching schedules
                        body.AppendChild(new Paragraph(new Run(new Text("Thời Khóa Biểu:"))));
                        foreach (var schedule in planbook.TeachingSchedules)
                        {
                            body.AppendChild(CreateParagraph($"- {schedule.Date.ToShortDateString()} từ {schedule.StartTime} đến {schedule.EndTime}"));
                        }

                        // Save the document
                        mainPart.Document.Save();
                    }

                    // Convert the memory stream content to a byte array
                    var wordBytes = memoryStream.ToArray();

                    return new ResponseModel
                    {
                        Success = true,
                        Message = Convert.ToBase64String(wordBytes)
                    };
                }
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while exporting the Planbook to Word.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        // Helper method to create a paragraph
        private Paragraph CreateParagraph(string text)
        {
            return new Paragraph(new Run(new Text(text)));
        }

        #endregion



        #region Export Planbook to PDF
        public async Task<ResponseModel> ExportPlanbookToPdfAsync(string planbookId)
        {
            try
            {
                var planbook = await _unitOfWork.PlanbookRepository.GetByIdAsync(planbookId, includeProperties: "Activities,Feedbacks,TeachingSchedules");

                if (planbook == null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Planbook not found."
                    };
                }

                using (var memoryStream = new MemoryStream())
                using (var writer = new iText.Kernel.Pdf.PdfWriter(memoryStream))
                using (var pdf = new iText.Kernel.Pdf.PdfDocument(writer))
                using (var document = new iText.Layout.Document(pdf))
                {
                    var fontPath = Path.Combine("fonts", "DejaVuSans.ttf");
                    if (!System.IO.File.Exists(fontPath))
                    {
                        return new ResponseModel
                        {
                            Success = false,
                            Message = "Font file not found."
                        };
                    }

                    var fontProgram = iText.Kernel.Font.PdfFontFactory.CreateFont(fontPath, iText.IO.Font.PdfEncodings.IDENTITY_H, iText.Kernel.Font.PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
                    document.SetFont(fontProgram);

                    // Add a large "Planbook" title at the top
                    var title = new iText.Layout.Element.Paragraph("Kế Hoạch Giảng Dạy")
                                    .SetFontSize(24)
                                    .SetBold()
                                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                    document.Add(title);

                    document.Add(new iText.Layout.Element.Paragraph("\n"));

                    // Add content to the PDF
                    document.Add(new iText.Layout.Element.Paragraph($"Tiêu Đề: {planbook.Title}"));
                    document.Add(new iText.Layout.Element.Paragraph($"Tên Trường: {planbook.SchoolName}"));
                    document.Add(new iText.Layout.Element.Paragraph($"Giáo Viên: {planbook.TeacherName}"));
                    document.Add(new iText.Layout.Element.Paragraph($"Môn Học: {planbook.Subject}"));
                    document.Add(new iText.Layout.Element.Paragraph($"Lớp Học: {planbook.ClassName}"));
                    document.Add(new iText.Layout.Element.Paragraph($"Số Tiết: {planbook.DurationInPeriods}"));
                    document.Add(new iText.Layout.Element.Paragraph($"Mục Tiêu Kiến Thức: {planbook.KnowledgeObjective}"));
                    document.Add(new iText.Layout.Element.Paragraph($"Mục Tiêu Kỹ Năng: {planbook.SkillsObjective}"));
                    document.Add(new iText.Layout.Element.Paragraph($"Mục Tiêu Phẩm Chất: {planbook.QualitiesObjective}"));
                    document.Add(new iText.Layout.Element.Paragraph($"Công Cụ Giảng Dạy: {planbook.TeachingTools}"));
                    document.Add(new iText.Layout.Element.Paragraph($"Ghi Chú: {planbook.Notes}"));

                    document.Add(new iText.Layout.Element.Paragraph("Thời Khóa Biểu:"));
                    foreach (var schedule in planbook.TeachingSchedules)
                    {
                        document.Add(new iText.Layout.Element.Paragraph($"- {schedule.Date.ToShortDateString()} từ {schedule.StartTime} đến {schedule.EndTime}"));
                    }

                    document.Close();

                    // Convert the memory stream content to a base64 string
                    var pdfBase64String = Convert.ToBase64String(memoryStream.ToArray());

                    return new ResponseModel
                    {
                        Success = true,
                        Message = pdfBase64String
                    };
                }
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while exporting the Planbook to PDF.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        #endregion
    }

}
