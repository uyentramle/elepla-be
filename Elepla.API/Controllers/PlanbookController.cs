using Elepla.Service.Interfaces;
using Elepla.Service.Models.ViewModels.PlanbookViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elepla.API.Controllers
{
	public class PlanbookController : BaseController
	{
		private readonly IPlanbookService _planbookService;

		public PlanbookController(IPlanbookService planbookService)
		{
			_planbookService = planbookService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllPlanbooksAsync(int pageIndex = 0, int pageSize = 10)
		{
			var response = await _planbookService.GetAllPlanbooksAsync(pageIndex, pageSize);
			return Ok(response);
		}

		[HttpGet]
		public async Task<IActionResult> GetPlanbookByIdAsync(string planbookId)
		{
			var response = await _planbookService.GetPlanbookByIdAsync(planbookId);
			return Ok(response);
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> GetPlanbookByCollectionIdAsync(string collectionId, int pageIndex = 0, int pageSize = 10)
		{
			var response = await _planbookService.GetPlanbookByCollectionIdAsync(collectionId, pageIndex, pageSize);
			return Ok(response);
		}

		[HttpGet]
		public async Task<IActionResult> GetPlanbookByLessonIdAsync(string lessonId, int pageIndex = 0, int pageSize = 10)
		{
			var response = await _planbookService.GetPlanbookByLessonIdAsync(lessonId, pageIndex, pageSize);
			return Ok(response);
		}

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> CreatePlanbookAsync(CreatePlanbookDTO model)
		{
			if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _planbookService.CreatePlanbookAsync(model);
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

		[HttpPut]
		[Authorize]
		public async Task<IActionResult> UpdatePlanbookAsync(UpdatePlanbookDTO model)
		{
			if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _planbookService.UpdatePlanbookAsync(model);
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

		[HttpDelete]
		[Authorize]
		public async Task<IActionResult> DeletePlanbookAsync(string planbookId)
        {
            var response = await _planbookService.DeletePlanbookAsync(planbookId);
			if (response != null)
			{
                return Ok(response);
            }
            return BadRequest(response);
        }

        //[HttpDelete]
		//[Authorize]
		//public async Task<IActionResult> SoftRemovePlanbookAsync(string planbookId)
		//{
		//	var response = await _planbookService.SoftRemovePlanbookAsync(planbookId);
		//	return Ok(response);
		//}

        [HttpPost]
		[Authorize]
		public async Task<IActionResult> CreatePlanbookFromTemplateAsync(string lessonId)
        {
            var response = await _planbookService.GetPlanbookFromTemplateAsync(lessonId);
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> CreatePlanbookUsingAIAsync(string lessonId)
		{
            var response = await _planbookService.GetPlanbookUsingAIAsync(lessonId);
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost]
		[Authorize]
		public async Task<IActionResult> ClonePlanbookAsync(ClonePlanbookDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _planbookService.ClonePlanbookAsync(model);
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ExportPlanbookToWord(string planbookId)
        {
            var response = await _planbookService.ExportPlanbookToWordAsync(planbookId);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            var wordBase64String = response.Message;
            if (string.IsNullOrEmpty(wordBase64String))
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Word document content not found."
                });
            }

            try
            {
                var wordBytes = Convert.FromBase64String(wordBase64String);
                var fileName = $"{planbookId}_Planbook.docx";
                return File(wordBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
            }
            catch (FormatException)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "The Word document content is not a valid Base64 string."
                });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ExportPlanbookToPdf(string planbookId)
        {
            var response = await _planbookService.ExportPlanbookToPdfAsync(planbookId);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            var pdfBase64String = response.Message;
            if (string.IsNullOrEmpty(pdfBase64String))
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "PDF content not found."
                });
            }

            try
            {
                var pdfBytes = Convert.FromBase64String(pdfBase64String);
                var fileName = $"{planbookId}_Planbook.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (FormatException)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "The PDF content is not a valid Base64 string."
                });
            }
        }
    }
}
