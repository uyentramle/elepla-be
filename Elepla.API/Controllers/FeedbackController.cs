using Elepla.Service.Interfaces;
using Elepla.Service.Models.ViewModels.FeedbackViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Elepla.API.Controllers
{
    public class FeedbackController : BaseController
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        #region Get Feedback by Planbook ID
        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> GetFeedbackByPlanbookIdAsync(string planbookId, int pageIndex = 0, int pageSize = 10)
        {
            var response = await _feedbackService.GetFeedbackByPlanbookIdAsync(planbookId, pageIndex, pageSize);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion

        #region Submit Feedback
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SubmitFeedbackAsync(CreateFeedbackDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _feedbackService.SubmitFeedbackAsync(model);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion

        #region Update Feedback
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateFeedbackAsync(UpdateFeedbackDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _feedbackService.UpdateFeedbackAsync(model);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion

        #region Delete Feedback (Hard Delete)
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> HardDeleteFeedbackAsync(string feedbackId)
        {
            var response = await _feedbackService.HardDeleteFeedbackAsync(feedbackId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion


        #region Flag feedback
        [HttpPost]
        //[Authorize(Roles = "AcademyStaff")]
        public async Task<IActionResult> FlagFeedbackAsync(string feedbackId)
        {
            var response = await _feedbackService.FlagFeedbackAsync(feedbackId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion


        #region View flagged feedback with pagination
        [HttpGet]
        [Authorize(Roles = "AcademyStaff")]
        public async Task<IActionResult> GetFlaggedFeedbackAsync(int pageIndex = 0, int pageSize = 10)
        {
            var response = await _feedbackService.GetFlaggedFeedbackAsync(pageIndex, pageSize);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion

        #region View system feedback 
        [HttpGet]
        public async Task<IActionResult> GetSystemFeedbackAsync(int pageIndex = 0, int pageSize = 10)
		{
			var response = await _feedbackService.GetSystemFeedbackAsync(pageIndex, pageSize);
			if (response.Success)
			{
				return Ok(response);
			}
			return BadRequest(response);
		}
		#endregion

		#region View planbook feedbacks
		[HttpGet]
		public async Task<IActionResult> GetPlanbookFeedbackAsync(int pageIndex = 0, int pageSize = 10)
		{
			var response = await _feedbackService.GetPlanbookFeedbackAsync(pageIndex, pageSize);
			if (response.Success)
			{
				return Ok(response);
			}
			return BadRequest(response);
		}
		#endregion
	}
}
