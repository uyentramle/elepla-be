using Elepla.Service.Interfaces;
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

        #region Export Planbook to PDF
        [HttpGet]
        public async Task<IActionResult> ExportPlanbookToPdf(string planbookId)
        {
            var response = await _planbookService.ExportPlanbookToPdfAsync(planbookId);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            // Assume that the response's Message contains the PDF content encoded in base64
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
                // Convert the base64 string to a byte array
                var pdfBytes = Convert.FromBase64String(pdfBase64String);

                var fileName = $"{planbookId}_Planbook.pdf";

                // Return the file as a downloadable PDF
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
        #endregion

    }
}
