using AutoMapper;
using Elepla.Repository.Common;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.ActivityViewModels;
using Elepla.Service.Models.ViewModels.PlanbookViewModels;
using iText.Bouncycastleconnector;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        // Method to export a Planbook to PDF
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
                using (var writer = new PdfWriter(memoryStream))
                using (var pdf = new PdfDocument(writer))
                using (var document = new Document(pdf))
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

                    var fontProgram = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
                    document.SetFont(fontProgram);

                    // Add a large "Planbook" title at the top
                    var title = new Paragraph("Kế Hoạch Giảng Dạy")
                                    .SetFontSize(24)
                                    .SetBold()
                                    .SetTextAlignment(TextAlignment.CENTER);
                    document.Add(title);

                    document.Add(new Paragraph("\n"));

                    // Add content to the PDF
                    document.Add(new Paragraph($"Tiêu Đề: {planbook.Title}"));
                    document.Add(new Paragraph($"Tên Trường: {planbook.SchoolName}"));
                    document.Add(new Paragraph($"Giáo Viên: {planbook.TeacherName}"));
                    document.Add(new Paragraph($"Môn Học: {planbook.Subject}"));
                    document.Add(new Paragraph($"Lớp Học: {planbook.ClassName}"));
                    document.Add(new Paragraph($"Số Tiết: {planbook.DurationInPeriods}"));
                    document.Add(new Paragraph($"Mục Tiêu Kiến Thức: {planbook.KnowledgeObjective}"));
                    document.Add(new Paragraph($"Mục Tiêu Kỹ Năng: {planbook.SkillsObjective}"));
                    document.Add(new Paragraph($"Mục Tiêu Phẩm Chất: {planbook.QualitiesObjective}"));
                    document.Add(new Paragraph($"Công Cụ Giảng Dạy: {planbook.TeachingTools}"));
                    document.Add(new Paragraph($"Ghi Chú: {planbook.Notes}"));

                    document.Add(new Paragraph("Thời Khóa Biểu:"));
                    foreach (var schedule in planbook.TeachingSchedules)
                    {
                        document.Add(new Paragraph($"- {schedule.Date.ToShortDateString()} từ {schedule.StartTime} đến {schedule.EndTime}"));
                    }

                    document.Close();

                    // Convert the PDF content to a base64 string
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



    }

}
