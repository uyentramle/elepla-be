using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.ExamViewModels;
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

        #region Get Exams by User ID
        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> GetExamsByUserIdAsync(string userId)
        {
            var response = await _examService.GetExamsByUserIdAsync(userId);
            if (response.Success)
            {
                return Ok(response);
            }
            return NotFound(response);
        }
        #endregion

        #region Get Exam by ID
        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> GetExamByIdAsync(string examId)
        {
            var response = await _examService.GetExamByIdAsync(examId);
            if (response.Success)
            {
                return Ok(response);
            }
            return NotFound(response);
        }
        #endregion

        #region Create Exam
        [HttpPost]
        //[Authorize]
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
        //[Authorize]
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
        //[Authorize]
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
        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> ExportExamToWord(string examId)
        {
            var response = await _examService.ExportExamToWordAsync(examId);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            var successResponse = response as SuccessResponseModel<string>; // Cast to access Data
            var filePath = successResponse?.Data ?? string.Empty;
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var fileName = Path.GetFileName(filePath);

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
        }

        #endregion

        #region Export Exam to PDF
        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> ExportExamToPdf(string examId)
        {
            var response = await _examService.ExportExamToPdfAsync(examId);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            var successResponse = response as SuccessResponseModel<string>; // Cast to access Data
            var filePath = successResponse?.Data ?? string.Empty;
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var fileName = Path.GetFileName(filePath);

            return File(fileBytes, "application/pdf", fileName);
        }

        #endregion


    }
}
