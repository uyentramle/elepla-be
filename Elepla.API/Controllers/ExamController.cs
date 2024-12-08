using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.ExamViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Elepla.API.Controllers
{
    public class ExamController : BaseController
    {
        private readonly IExamService _examService;

        public ExamController(IExamService examService)
        {
            _examService = examService;
        }

        #region Get Exams by User Id
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetExamsByUserIdAsync(string userId)
        {
            var response = await _examService.GetExamsByUserIdAsync(userId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion

        #region Get Exam by Id
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetExamByIdAsync(string examId)
        {
            var response = await _examService.GetExamByIdAsync(examId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion

        #region Create Exam
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateExamAsync(CreateExamDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _examService.CreateExamAsync(model);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion

        #region Update Exam
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateExamAsync(UpdateExamDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _examService.UpdateExamAsync(model);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion

        #region Delete Exam
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteExamAsync(string examId)
        {
            var response = await _examService.DeleteExamAsync(examId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion

        #region Export Exam to Word
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ExportExamToWordAsync(string examId)
        {
            var response = await _examService.ExportExamToWordAsync(examId) as SuccessResponseModel<byte[]>;

            if (response == null || !response.Success)
            {
                return StatusCode(500, response?.Message ?? "Failed to generate Word document.");
            }

            var wordData = response.Data;

            // Ensure the response has valid Word data
            if (wordData == null || wordData.Length == 0)
            {
                return StatusCode(500, "Failed to generate Word data.");
            }

            return File(wordData, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Exam_{examId}.docx");
        }
        #endregion

        #region Export Exam to PDF
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ExportExamToPdfAsync(string examId)
        {
            var response = await _examService.ExportExamToPdfAsync(examId) as SuccessResponseModel<byte[]>;

            if (response == null || !response.Success)
            {
                return StatusCode(500, response?.Message ?? "Failed to generate PDF.");
            }

            var pdfData = response.Data;

            // Ensure the response has valid PDF data
            if (pdfData == null || pdfData.Length == 0)
            {
                return StatusCode(500, "Failed to generate PDF data.");
            }

            return File(pdfData, "application/pdf", $"Exam_{examId}.pdf");
        }
        #endregion

        #region Export Exam to Word without Color
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ExportExamToWordNoColorAsync(string examId)
        {
            var response = await _examService.ExportExamToWordNoColorAsync(examId) as SuccessResponseModel<byte[]>;

            if (response == null || !response.Success)
            {
                return StatusCode(500, response?.Message ?? "Failed to generate Word document.");
            }

            var wordData = response.Data;

            if (wordData == null || wordData.Length == 0)
            {
                return StatusCode(500, "Failed to generate Word data.");
            }

            return File(wordData, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Exam_{examId}.docx");
        }
        #endregion

        #region Export Exam to PDF without Color
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ExportExamToPdfNoColorAsync(string examId)
        {
            var response = await _examService.ExportExamToPdfNoColorAsync(examId) as SuccessResponseModel<byte[]>;

            if (response == null || !response.Success)
            {
                return StatusCode(500, response?.Message ?? "Failed to generate PDF.");
            }

            var pdfData = response.Data;

            if (pdfData == null || pdfData.Length == 0)
            {
                return StatusCode(500, "Failed to generate PDF data.");
            }

            return File(pdfData, "application/pdf", $"Exam_{examId}.pdf");
        }
        #endregion
    }
}
